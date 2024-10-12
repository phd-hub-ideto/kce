using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Application.Portal.Models.Users.UserManagement;
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

[Authorize]
public sealed class UserManagementController(
    IHttpRequestProvider httpRequestProvider,
    IUserAccessService userAccessService,
    IUserActivationService userActivationService,
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IKCRouteBuilder kCRouteBuilder) : BaseController
{
    [HttpGet]
    [Route("users", Name = RouteNames.UserManagement.Users)]
    public ActionResult Users()
    {
        if (!HasAccess(AdministratorPermission.ManageUsers))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var users = userRepository.Query()
                        .Take(68)
                        .Select(u => UserModel.Create(u, Url))
                        .ToList();

        return View(users);
    }

    [HttpGet]
    [Route("create-user", Name = RouteNames.UserManagement.CreateUser)]
    public ActionResult CreateUser()
    {
        if (!HasAccess(AdministratorPermission.ManageUsers))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        return View();
    }

    [HttpPost]
    [Route("create-user", Name = RouteNames.UserManagement.AdminCreateUser)]
    public ActionResult AdminCreateUser([FromForm] CreateUserModel model)
    {
        if (!HasAccess(AdministratorPermission.ManageUsers))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        if (ModelState.IsValid)
        {
            using var scope = new TransactionScope();

            var returnUrl = httpRequestProvider.UrlReferrer.Host;

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

                var userRole = new UserRole
                {
                    UserId = user.Id.Value,
                    RoleId = (int)model.Role.Value,
                    SecurityEntityType = SecurityEntityType.Administrator
                };

                userRoleRepository.Upsert(userRole);

                scope.Complete();

                return Redirect(Url.Action<UserManagementController>(c => c.ViewUser(user.Id.Value)));
            }

            ViewBag.Message = response.Errors.First().GetBestDescription();
        }

        return View("CreateUser", model);
    }

    [HttpGet]
    [Route("user/{id:int}", Name = RouteNames.UserManagement.ViewUser)]
    public ActionResult ViewUser(int id)
    {
        if (!HasAccess(AdministratorPermission.ManageUsers))
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var user = userRepository.Query().SingleOrDefault(u => u.Id == id);

        if (user == null)
        {
            return Redirect(kCRouteBuilder.BuildUrl(RouteNames.Site.Home));
        }

        var model = ViewUserModel.Create(user);

        return View(model);
    }

    private void ActivateUser(UserAccountManager userAccountManager)
    {
        //TODO-LP: You may also consider subscribing them to your news letter and wrap this in a transaction

        userActivationService.Activate(userAccountManager.User);
    }
}