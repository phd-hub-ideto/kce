using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain;
using KhumaloCraft.Domain.Security;
using KhumaloCraft.Domain.Users;
using System.Transactions;

namespace KhumaloCraft.Application.Users;

[Singleton]
public class UserDeleter(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IResetPasswordHasher resetPasswordHasher,
    IPrincipalResolver principalResolver)
{
    public void TryDeleteUserAccount(User user, string token, out bool success)
    {
        success = false;

        if (user.Id != principalResolver.GetRequiredUserId())
        {
            return;
        }

        try
        {
            if (resetPasswordHasher.IsValidToken(user.Username, token))
            {
                Delete(user.Id.Value);

                success = true;
            }
        }
        catch
        {
            success = false;
        }
    }

    public void Delete(int userId)
    {
        var user = userRepository.Query().Single(u => u.Id == userId);

        CheckRegistrationStatus(user);

        using var scope = new TransactionScope();

        userRoleRepository.DeleteUserRolesByUserId(userId);

        user.Username = $"deleted{DateTime.Now.Ticks}@unknownuser.com";

        //check here and delete
        user.Reset();

        user.Deleted = true;

        userRepository.Upsert(user);

        scope.Complete();
    }

    private static void CheckRegistrationStatus(User user)
    {
        if (!user.IsRegistered)
        {
            throw Exceptions.UserIsNotRegistered(user.Username);
        }
    }

    public void Undelete(int userId)
    {
        var user = userRepository.Query().Single(u => u.Id == userId);

        CheckRegistrationStatus(user);

        user.Deleted = false;

        userRepository.Upsert(user);
    }
}