using KhumaloCraft.Domain.Users;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Application.Portal.Models.Users.UserManagement;

public sealed class ViewUserModel
{
    public string CreatedDate { get; set; }
    public string LastLogin { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string MobileNumber { get; set; }

    public static ViewUserModel Create(User user)
    {
        return new ViewUserModel
        {
            CreatedDate = FormattingHelper.FormatDateTime(user.CreationDate),
            LastLogin = user.LastLoginDate.HasValue ? FormattingHelper.FormatDateTime(user.LastLoginDate.Value) : "-",
            FullName = user.Fullname,
            Username = user.Username,
            MobileNumber = user.MobileNumber
        };
    }
}