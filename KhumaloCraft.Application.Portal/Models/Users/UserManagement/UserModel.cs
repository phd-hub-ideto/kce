using KhumaloCraft.Application.Mvc;
using KhumaloCraft.Application.Portal.Controllers;
using KhumaloCraft.Domain.Users;
using KhumaloCraft.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Application.Portal.Models.Users.UserManagement;

public sealed class UserModel
{
    public int UserId { get; set; }
    public string CreatedDate { get; set; }
    public string LastLogin { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string MobileNumber { get; set; }
    public string ViewUrl { get; set; }

    public static UserModel Create(User user, IUrlHelper urlHelper)
    {
        return new UserModel
        {
            UserId = user.Id.Value,
            CreatedDate = FormattingHelper.FormatDateTime(user.CreationDate),
            LastLogin = user.LastLoginDate.HasValue ? FormattingHelper.FormatDateTime(user.LastLoginDate.Value) : "-",
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            MobileNumber = user.MobileNumber,
            ViewUrl = urlHelper.Action<UserManagementController>(c => c.ViewUser(user.Id.Value))
        };
    }
}