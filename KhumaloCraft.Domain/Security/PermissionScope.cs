using KhumaloCraft.Domain.Authentication;
using KhumaloCraft.Helpers;

namespace KhumaloCraft.Domain.Security;

public abstract class PermissionScope : IDisposable, IPermissionSource
{
    private static readonly PermissionKey[] _emptyPermissions = [];

    internal static PermissionKey[] _allAdministratorPermissions;

    internal static PermissionKey[] AllAdministrationPermissions
    {
        get
        {
            if (_allAdministratorPermissions == null)
            {
                var permissions = new List<PermissionKey>();

                var permissionKeys = PermissionKey.CreateFrom((AdministratorPermission[])EnumHelper.GetValues<AdministratorPermission>());

                permissions.AddRange(permissionKeys);

                _allAdministratorPermissions = permissions.ToArray();
            }

            return _allAdministratorPermissions;
        }
    }

    static PermissionScope() { }

    public static PermissionScope GrantAdminForAllCountries()
    {
        return GrantPermissions(AllAdministrationPermissions);
    }

    public static PermissionScope GrantAdminPermission(params AdministratorPermission[] administratorPermissions)
    {
        var permissionKeys = administratorPermissions.ToList().ConvertAll(u => new PermissionKey(u));

        return GrantPermissions(permissionKeys);
    }

    public static PermissionScope GrantPermissions(params PermissionKey[] permissions)
    {
        return GrantPermissions((IEnumerable<PermissionKey>)permissions);
    }

    public static PermissionScope GrantPermissions(IEnumerable<PermissionKey> permissions)
    {
        var nestedScope = new NestedPermissionScope(Current, permissions);
        Scopes.Push(nestedScope);

        return nestedScope;
    }

    public static PermissionScope Restrict()
    {
        var nestedScope = new RestrictivePermissionScope();
        Scopes.Push(nestedScope);

        return nestedScope;
    }

    private static AsyncLocal<Stack<PermissionScope>> _scopes = new AsyncLocal<Stack<PermissionScope>>();
    private static Stack<PermissionScope> Scopes => _scopes.Value ??= new Stack<PermissionScope>(
        [
            new RootPermissionScope(Dependencies.DependencyManager.Container.GetInstance<IWebContext>())
        ]
    );

    public int UserId => PrincipalResolver.Instance.GetRequiredUserId();

    public abstract IEnumerable<PermissionKey> UserPermissions { get; }

    public static PermissionScope Current => Scopes.Peek();

    public virtual void Dispose()
    {
        if (!ReferenceEquals(Current, this))
        {
            throw new InvalidOperationException("PermissionScopes have become imbalanced");
        }

        Scopes.Pop();
    }

    private class RootPermissionScope : PermissionScope
    {
        private readonly IWebContext _webContext;

        public RootPermissionScope(IWebContext webContext)
        {
            _webContext = webContext;
        }

        public override IEnumerable<PermissionKey> UserPermissions
        {
            get
            {
                if (PrincipalResolver.Instance.TryResolveCurrentPrincipal(out var principal))
                {
                    return principal.UserPermissions;
                }

                // If we are in a web context we need to honour all permission checks.
                var httpContextProvider = Dependencies.DependencyManager.HttpContextProvider;

                if (httpContextProvider.CanGetHttpContext && !_webContext.IsUnitTest)
                {
                    return _emptyPermissions;
                }

                // During unit tests and tasks we can assume that the code is running with administrative permissions
                return AllAdministrationPermissions;
            }
        }

        public override void Dispose()
        {
            throw new InvalidOperationException("The root PermissionScope may not be closed.");
        }
    }

    private class NestedPermissionScope : PermissionScope
    {
        public NestedPermissionScope(PermissionScope parent, IEnumerable<PermissionKey> permissions)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions), "userPermissions");
        }

        private PermissionScope _parent;
        private IEnumerable<PermissionKey> _permissions;

        public override IEnumerable<PermissionKey> UserPermissions
        {
            get
            {
                foreach (var permission in _permissions)
                {
                    yield return permission;
                }

                foreach (var permission in _parent.UserPermissions)
                {
                    yield return permission;
                }
            }
        }
    }

    private class RestrictivePermissionScope : PermissionScope
    {
        public RestrictivePermissionScope() { }

        public override IEnumerable<PermissionKey> UserPermissions => _emptyPermissions;
    }
}