using KhumaloCraft.Domain.Dates;
using KhumaloCraft.Domain.Users;
using System.Transactions;

namespace KhumaloCraft.Application.Users;

public class UserActivationService(
    IUserRepository userRepository,
    IDateProvider dateProvider) : IUserActivationService
{
    public void Activate(User user)
    {
        if (!user.IsActivated)
        {
            using var scope = new TransactionScope();

            if (!user.IsRegistered)
            {
                throw Exceptions.UserIsNotRegistered(user.Username);
            }

            if (user.ActivatedDate == null)
            {
                user.ActivatedDate = dateProvider.GetDate();
            }

            user.ValidatedEmail = true;
            user.Deleted = false;

            userRepository.Upsert(user);

            scope.Complete();
        }
    }
}