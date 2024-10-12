using KhumaloCraft.Data.Sql.Images;
using KhumaloCraft.Data.Sql.Settings;
using KhumaloCraft.Data.Sql.Transactions;
using KhumaloCraft.Data.Sql.Users;
using KhumaloCraft.Dependencies;
using KhumaloCraft.Domain.Configuration;
using KhumaloCraft.Domain.Events;
using KhumaloCraft.Domain.Images;
using KhumaloCraft.Domain.Users;
using SimpleInjector;

namespace KhumaloCraft.Data.Sql;

public static class SqlDependencies
{
    public static void Register(Container container, EntryPoint entryPoint)
    {
        container.RegisterUsingAttributes(typeof(SqlDependencies).Assembly);

        container.RegisterSingleton<ISettingsRepository, SettingsRepository>();
        container.RegisterSingleton<IUserRepository, UserRepository>();
        container.RegisterSingleton<IUserRolePermissionRepository, UserRolePermissionRepository>();
        container.RegisterSingleton<ITransactionProvider, TransactionProvider>();
        container.RegisterSingleton<IImageBlobRepository, ImageBlobRepository>();
        container.RegisterSingleton<IImageRepository, ImageRepository>();
    }
}