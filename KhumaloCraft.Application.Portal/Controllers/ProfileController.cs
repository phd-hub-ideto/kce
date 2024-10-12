using KhumaloCraft.Application.Portal.Models.Profile;
using KhumaloCraft.Application.Portal.Models.Profile.Password;
using KhumaloCraft.Application.Portal.Routing;
using KhumaloCraft.Application.Users;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Controllers;

[Authorize]
public sealed class ProfileController(
    IUserRepository userRepository,
    Domain.Users.IUserService userService,
    IPrincipalResolver principalResolver) : BaseController
{
    [HttpGet]
    [Route("profile", Name = RouteNames.Profile.Default)]
    public ActionResult MyProfile()
    {
        var user = principalResolver.ResolveCurrentUser();

        var model = new ProfileModel
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MobileNumber = user.MobileNumber
        };

        return View("Profile", model);
    }

    [HttpPost]
    [Route("profile", Name = RouteNames.Profile.ProfileUpdate)]
    public ActionResult UpdateProfile([FromForm] ProfileModel model)
    {
        if (ModelState.IsValid)
        {
            var user = userRepository.Query().Single(u => u.Id == principalResolver.GetRequiredUserId());

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.MobileNumber = model.MobileNumber;

            userService.UpdateUser(user);

            ViewBag.Message = "Details Updated Successfully!";
        }

        return View("Profile", model);
    }

    [HttpGet]
    [Route("change-password", Name = RouteNames.Profile.ChangePassword)]
    public ActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Route("change-password", Name = RouteNames.Profile.UpdatePassword)]
    public ActionResult UpdatePassword([FromForm] ChangePasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var user = userRepository.Query().Single(u => u.Id == principalResolver.GetRequiredUserId());

            var userAccountManager = new UserAccountManager(user);

            userAccountManager.ChangePassword(model.NewPassword, false);

            ViewBag.Message = "Password Updated Successfully!";
        }

        return View("ChangePassword", model);
    }
}