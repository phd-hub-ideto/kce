using KhumaloCraft.Application.Authentication;
using KhumaloCraft.Application.Portal.Models.Account.Activation;
using KhumaloCraft.Application.Portal.Models.Account.Login;
using KhumaloCraft.Application.Portal.Models.Account.Register;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Application.Routing.Routes;
using KhumaloCraft.Application.Users;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Domain.Users.Access;
using KhumaloCraft.Helpers;
using KhumaloCraft.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace KhumaloCraft.Application.Portal.Controllers;

public sealed class AccountController(
    IPrincipalResolver principalResolver,
    IHttpRequestProvider httpRequestProvider,
    IFormsAuthenticator formsAuthenticator,
    IUserAccessService userAccessService,
    IUserActivationService userActivationService,
    IUserRepository userRepository,
    IResetPasswordHasher resetPasswordHasher,
    IKCRouteBuilder kCRouteBuilder) : BaseController
{
    [HttpGet]
    [Route("login", Name = RouteNames.Account.Login)]
    public ActionResult Login(string returnUrl = null)
    {
        var model = new LoginModel
        {
            ReturnUrl = returnUrl
        };

        if (principalResolver.IsAuthenticated())
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        model.ReturnUrl = httpRequestProvider.UrlReferrer?.PathAndQuery ?? httpRequestProvider.RequestUrl;

        return View(model);
    }

    [HttpPost]
    [Route("login", Name = RouteNames.Account.LoginUser)]
    public ActionResult LoginUser([FromForm] LoginModel model)
    {
        if (principalResolver.IsAuthenticated())
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (ModelState.IsValid)
        {
            var loginParameters = new LoginParameters
            {
                Username = model.Username,
                Password = model.Password,
                PersistCookie = model.RememberMe,
                LogUserLogin = true,
                LoginSource = LoginSource.Portal
            };

            var loginResult = userAccessService.Login(loginParameters);

            if (loginResult.Success)
            {
                return Redirect(model.ReturnUrl);
            }

            switch (loginResult.Error)
            {
                case LoginError.UserRequiresActivation:
                    ViewBag.Message = "An email was sent with details on how to activate your account.";
                    break;
                case LoginError.NotAuthenticated:
                case LoginError.AccountDoesNotExist:
                    ViewBag.Message = "Invalid Email or Password";
                    break;
                default:
                    throw new NotSupportedException($"Unsupported Login Error: {loginResult.Error}");
            }
        }

        return View("Login", model);
    }

    [HttpGet]
    [Route("register", Name = RouteNames.Account.Register)]
    public ActionResult Register()
    {
        if (principalResolver.IsAuthenticated())
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var model = new RegisterModel
        {
            ReturnUrl = httpRequestProvider.UrlReferrer?.PathAndQuery ?? httpRequestProvider.RequestUrl
        };

        return View(model);
    }

    [HttpPost]
    [Route("register", Name = RouteNames.Account.RegisterUser)]
    public ActionResult RegisterUser([FromForm] RegisterModel model)
    {
        if (principalResolver.IsAuthenticated())
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (ModelState.IsValid)
        {
            using var scope = new TransactionScope();

            var returnUrl = model.ReturnUrl ?? httpRequestProvider.UrlReferrer.PathAndQuery;

            var response = userAccessService.Register(
                new RegistrationParameters
                {
                    Username = model.Username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    MobileNumber = model.MobileNumber,
                    Password = model.Password,
                    PassThroughParameters = new PassThroughParameters
                    {
                        ReturnUrl = returnUrl
                    }
                });

            if (response.Success)
            {
                var user = userRepository.Query().SingleOrDefault(u => u.Id == response.UserId.Value);

                //TODO : Once Implemented the sending of emails for verification, stop activating immediately
                //Activate the User Account Immediately and redirect to home page
                var userAccountManager = new UserAccountManager(user);

                if (!userAccountManager.IsActivated())
                {
                    ActivateUser(userAccountManager);
                }

                var loginParameters = new LoginParameters
                {
                    Username = model.Username,
                    Password = model.Password,
                    PersistCookie = true,
                    LogUserLogin = true,
                    LoginSource = LoginSource.Portal
                };

                userAccessService.Login(loginParameters);

                scope.Complete();

                return Redirect(returnUrl);
            }

            ViewBag.Message = response.Errors.First().GetBestDescription();
        }

        return View("Register", model);
    }

    [AllowAnonymous]
    [Route(Constants.Pages.AccountActivationUrl, Name = RouteNames.Account.Activate)]
    public ActionResult ActivateUserAccount(ActivateUserAccountModel model)
    {
        if (principalResolver.IsAuthenticated())
        {
            return Redirect(Url.RouteUrl(RouteNames.Profile.Default));
        }

        var user = userRepository.Query().SingleOrDefault(u => u.Username == model.Username);

        if (user == null)
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var loginUrl = model.LoginUrl;

        if (string.IsNullOrWhiteSpace(loginUrl))
        {
            loginUrl = Url.RouteUrl(RouteNames.Account.Login, new { model.Username, model.ReturnUrl });
        }

        var userAccountManager = new UserAccountManager(model.Username);

        if (userAccountManager.IsActivated())
        {
            return Redirect(loginUrl);
        }

        string errorModel = null;

        if (!userAccountManager.IsRegistered())
        {
            errorModel = "Unknown username specified.";
        }

        if (!resetPasswordHasher.IsValidToken(model.Username, model.Token, model.ReturnUrl))
        {
            return Redirect(Url.RouteUrl(RouteNames.Account.Login, new { IsEmailLinkExpired = true }));
        }

        if (string.IsNullOrWhiteSpace(errorModel))
        {
            try
            {
                /* TODO-LP : Implement Change Password Page
                if (user.RequiresPassword)
                {
                    var changePasswordModel = new ChangePasswordModel()
                    {
                        LoginUrl = loginUrl,
                        Token = model.Token,
                        Username = user.Username,
                        ReturnUrl = model.ReturnUrl
                    };

                    changePasswordModel.SetTextForUserActivation();

                    return View("ChangePassword", changePasswordModel);
                }*/

                ActivateUser(userAccountManager);

                return Redirect(loginUrl);
            }
            catch (Exception)
            {
                errorModel = "An unknown system error occurred, please try again!";
            }
        }

        //TODO-LP : Add ActivateUserAccountError View
        return View("ActivateUserAccountError", errorModel);
    }

    [Route("logout", Name = RouteNames.Account.Logout)]
    public ActionResult Logout()
    {
        return Logout(formsAuthenticator, httpRequestProvider);
    }

    private void ActivateUser(UserAccountManager userAccountManager)
    {
        //TODO-LP: You may also consider subscribing them to your news letter and wrap this in a transaction

        userActivationService.Activate(userAccountManager.User);
    }
}