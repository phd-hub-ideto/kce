using KhumaloCraft.Data.Entities;
using KhumaloCraft.Data.Entities.Entities;
using KhumaloCraft.Data.Sql.Queries;
using KhumaloCraft.Domain.Users;

namespace KhumaloCraft.Data.Sql.Users;

public sealed class UserRepository : IUserRepository
{
    public IQueryable<User> Query(bool includeDeleted = false)
    {
        return QueryContainerFactory.Create(scope =>
            from user in scope.KhumaloCraft.User
            where includeDeleted || user.Deleted == false
            select new User
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MobileNumber = user.MobileNumber,
                ImageReferenceId = user.ImageReferenceId,
                CreationDate = user.CreationDate,
                ActivatedDate = user.ActivatedDate,
                ActivationEmailSentDate = user.ActivationEmailSentDate,
                LastLoginDate = user.LastLoginDate,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                ValidatedEmail = user.ValidatedEmail,
                Deleted = user.Deleted
            }
        );
    }

    public void Upsert(User user)
    {
        using var scope = DalScope.Begin();

        DalUser dalUser;

        if (user.IsNew)
        {
            dalUser = new DalUser
            {
                CreationDate = scope.ServerDate()
            };
        }
        else
        {
            dalUser = scope.KhumaloCraft.User.Single(u => u.Id == user.Id);
        }

        dalUser.Username = user.Username.Trim();
        dalUser.FirstName = user.FirstName;
        dalUser.LastName = user.LastName;
        dalUser.MobileNumber = user.MobileNumber;
        dalUser.ImageReferenceId = user.ImageReferenceId;
        dalUser.ActivatedDate = user.ActivatedDate;
        dalUser.ActivationEmailSentDate = user.ActivationEmailSentDate;
        dalUser.PasswordHash = user.PasswordHash;
        dalUser.PasswordSalt = user.PasswordSalt;
        dalUser.ValidatedEmail = user.ValidatedEmail;
        dalUser.Deleted = user.Deleted;

        scope.KhumaloCraft.User.Update(dalUser);

        scope.Commit();

        user.Id = dalUser.Id;
        user.CreationDate = dalUser.CreationDate;
    }

    public void UpdateLastLoginDate(int userId)
    {
        using var scope = DalScope.Begin();

        var dalUser = scope.KhumaloCraft.User.Single(u => u.Id == userId);

        dalUser.LastLoginDate = scope.ServerDate();

        scope.KhumaloCraft.User.Update(dalUser);

        scope.Commit();
    }

    public bool ExistsByUsername(string username)
    {
        using var scope = DalScope.Begin();

        return scope.KhumaloCraft.User.Any(u => u.Username == username);
    }
}