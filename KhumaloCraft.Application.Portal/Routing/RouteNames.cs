namespace KhumaloCraft.Application.Portal.Routing;

// TODO-L: unit test to ensure these are all unique
// or, maybe we could just use GUIDs since the actual value doesn't matter so long as it's unique
// TODO-L: gen via t4 template
public static class RouteNames
{
    public static class Site
    {
        public const string Home = nameof(Site) + nameof(Home);
    }

    public static class Account
    {
        public const string Login = nameof(Account) + nameof(Login);
        public const string LoginUser = nameof(Account) + nameof(LoginUser);
        public const string Logout = nameof(Account) + nameof(Logout);
        public const string Register = nameof(Account) + nameof(Register);
        public const string RegisterUser = nameof(Account) + nameof(RegisterUser);
        public const string Activate = nameof(Account) + nameof(Activate);
        public const string ActivateResult = nameof(Account) + nameof(ActivateResult);
        public const string ResetPassword = nameof(Account) + nameof(ResetPassword);
        public const string ResetPasswordResult = nameof(Account) + nameof(ResetPasswordResult);
        public const string ChangePassword = nameof(Account) + nameof(ChangePassword);
        public const string ChangePasswordResult = nameof(Account) + nameof(ChangePasswordResult);
    }

    public static class UserManagement
    {
        public const string Users = nameof(UserManagement) + nameof(Users);
        public const string CreateUser = nameof(UserManagement) + nameof(CreateUser);
        public const string AdminCreateUser = nameof(UserManagement) + nameof(AdminCreateUser);
        public const string ViewUser = nameof(UserManagement) + nameof(ViewUser);
    }

    public static class Cart
    {
        public const string AddToCart = nameof(Cart) + nameof(AddToCart);
        public const string IncrementCartItem = nameof(Cart) + nameof(IncrementCartItem);
        public const string DecrementCartItem = nameof(Cart) + nameof(DecrementCartItem);
        public const string ViewCart = nameof(Cart) + nameof(ViewCart);
    }

    public static class UserOrders
    {
        public const string Orders = nameof(UserOrders) + nameof(Orders);
        public const string PlaceOrder = nameof(UserOrders) + nameof(PlaceOrder);
    }

    public static class ManageOrders
    {
        public const string ViewOrders = nameof(ManageOrders) + nameof(ViewOrders);
        public const string ViewOrder = nameof(ManageOrders) + nameof(ViewOrder);
        public const string UpdateOrder = nameof(UserOrders) + nameof(UpdateOrder);
    }

    public static class Craftwork
    {
        public const string Craftworks = nameof(Craftwork) + nameof(Craftworks);
        public const string CreateCraftwork = nameof(Craftwork) + nameof(CreateCraftwork);
        public const string CreateCraftworkPost = nameof(Craftwork) + nameof(CreateCraftworkPost);
        public const string UpdateCraftwork = nameof(Craftwork) + nameof(UpdateCraftwork);
        public const string ViewCraftwork = nameof(Craftwork) + nameof(ViewCraftwork);
    }

    public static class Information
    {
        //TODO-LP : Add more site information routes / pages
        public const string PrivacyPolicy = nameof(Information) + nameof(PrivacyPolicy);
        public const string DataProcessingAgreement = nameof(Information) + nameof(DataProcessingAgreement);
        public const string CookiePolicy = nameof(Information) + nameof(CookiePolicy);
        public const string TermsConditions = nameof(Information) + nameof(TermsConditions);
        public const string FAQ = nameof(Information) + nameof(FAQ);
        public const string SafetyAndSecurity = nameof(Information) + nameof(SafetyAndSecurity);
        public const string About = nameof(Information) + nameof(About);
        public const string ContactUs = nameof(Information) + nameof(ContactUs);
    }

    public static class Profile
    {
        public const string Default = nameof(Profile) + nameof(Default);
        public const string ProfileGet = nameof(Profile) + nameof(ProfileGet);
        public const string ProfileUpdate = nameof(Profile) + nameof(ProfileUpdate);
        public const string ChangePassword = nameof(Profile) + nameof(ChangePassword);
        public const string UpdatePassword = nameof(Profile) + nameof(UpdatePassword);
        public const string SignOut = nameof(Profile) + nameof(SignOut);
        public const string DeleteProfile = nameof(Profile) + nameof(DeleteProfile);
        public const string ViewOrders = nameof(Profile) + nameof(ViewOrders);
    }

    public static class Robot
    {
        //TODO-LP : Robots - Site Indexing (SEO)
        public const string Robots = nameof(Robot) + nameof(Robots);
    }
}