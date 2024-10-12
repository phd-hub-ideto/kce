using System.Net.Mail;
using System.Net.Mime;

namespace KhumaloCraft;

public static class Constants
{
    public const string KhumaloCraft = "KhumaloCraft";
    public static readonly Uri KhumaloCraftUri = Urls.Constants.KhumaloCraftUri;
    public static readonly int RecentSearchMaxDisplay = 5;

    public static class SocialUrls
    {
        public static readonly string TikTokUrl = "https://www.tiktok.com";
        public static readonly string FacebookUrl = "https://www.facebook.com";
        public static readonly string TwitterUrl = "https://www.x.com";
        public static readonly string YoutubeUrl = "https://www.youtube.com";
        public static readonly string InstagramUrl = "https://www.instagram.com";
    }

    public static class UrchinTrackingModuleKey
    {
        public static readonly string Source = "utm_source";
        public static readonly string Medium = "utm_medium";
        public static readonly string Term = "utm_term";
        public static readonly string Content = "utm_content";
        public static readonly string Campaign = "utm_campaign";
    }

    public static class Email
    {
        public static readonly string Support = "customerservice@khumalocraft.co.za";
    }

    public static class EMailAddress
    {
        public static readonly string NoReply = Settings.Instance.NoReplyEmailEddress;
        public static readonly string Support = $"KhumaloCraft Support <{Email.Support}>";
        public static readonly string CustomerServiceCentre = $"KhumaloCraft Support Team <{Email.Support}>";

        public static class Subject
        {
            public static readonly string AccountActivate = KhumaloCraft + " Account Activation";
            public static readonly string AccountDeletion = KhumaloCraft + " Delete My Profile";
            public static readonly string PasswordReset = KhumaloCraft + " Reset Password";
            public static readonly string CompleteActivation = KhumaloCraft + " - Verify Your Account";
            public static readonly string AccountPasswordActivation = KhumaloCraft + " - Account Password Activation";
        }

        public static string GetSupportEmail()
        {
            return new MailAddress(CustomerServiceCentre).Address;
        }
    }

    public static class Telephone
    {
        public static readonly string CustomerServiceCentre = "0874 65 28 28";
        public static string CustomerServiceCentreInternational => CustomerServiceCentre;
        public static readonly string CustomerServiceCentreInternationalDialFriendly = $"(+27) {CustomerServiceCentre}";
    }

    public static class MediaTypes
    {
        public const string Jpeg = MediaTypeNames.Image.Jpeg;
        public const string Png = "image/png";
    }

    public static class Pages
    {
        public static readonly string BaseHome = "/";

        public const string PasswordResetUrl = "user/password/reset";
        public const string PasswordChangeUrl = "user/password/change";
        public const string AccountActivationUrl = "user/activate";
        public const string AccountActivationDelayedPasswordUrl = "account-activation";
    }
    public static class Regional
    {
        public static class Countries
        {
            public static readonly SouthAfricaConstantCountry SouthAfrica = new SouthAfricaConstantCountry();
            public static readonly ConstantCountry Botswana = new ConstantCountry("Botswana", 2);
            public static readonly ConstantCountry Mozambique = new ConstantCountry("Mozambique", 3);
            public static readonly ConstantCountry Mauritius = new ConstantCountry("Mauritius", 4);
            public static readonly ConstantCountry Swaziland = new ConstantCountry("Swaziland", 5);
            public static readonly ConstantCountry Namibia = new ConstantCountry("Namibia", 6);
            public static readonly ConstantCountry Zambia = new ConstantCountry("Zambia", 7);
            public static readonly ConstantCountry Zimbabwe = new ConstantCountry("Zimbabwe", 9);
            public static readonly ConstantCountry Nigeria = new ConstantCountry("Nigeria", 10);
            public static readonly ConstantCountry Kenya = new ConstantCountry("Kenya", 11);
        }

        public static class Restrictions
        {
            public static readonly int[] SouthAfricaOnlyCountryIds = { Countries.SouthAfrica.Id, };
        }
    }

    // See also Portal.ContentType class.
    // https://msdn.microsoft.com/en-us/library/system.web.mimemapping.getmimemapping(v=vs.110).aspx
    public static class MimeTypes
    {
        public const string Json = ContentType.Json;

        public static class Exe
        {
            public static readonly IEnumerable<string> Types = new[]
            {
                "application/octet-stream",
                "application/x-msdownload",
                "application/exe",
                "application/x-exe",
                "application/dos-exe",
                "vms/exe",
                "application/x-winexe",
                "application/msdos-windows",
                "application/x-msdos-program",
            };
        }

        public static class Csv
        {
            public const string Default = "text/csv";

            public static readonly IEnumerable<string> OtherTypes = new[]
            {
                "text/comma-separated-values",
                "application/csv",
                "application/excel",
                "application/vnd.ms-excel",
                "application/vnd.msexcel",
                "text/anytext",
            };
        }

        public static class Zip
        {
            public const string Default = "application/zip";

            public static readonly IEnumerable<string> OtherTypes = new[]
            {
                "application/octet-stream",
                "application/x-zip-compressed",
                "multipart/x-zip"
            };
        }

        public static class Pdf
        {
            public const string Default = "application/pdf";
        }
    }
}

public abstract class ConstantEntity
{
    public string Name { get; }

    public int Id { get; }

    protected ConstantEntity(string name, int id)
    {
        Name = name;
        Id = id;
    }
}

public class ConstantCountry : ConstantEntity
{
    public ConstantCountry(string name, int id)
        : base(name, id)
    {
    }
}

public interface ISouthAfricaProvinces
{
    ConstantProvince Gauteng { get; }
    ConstantProvince KwaZuluNatal { get; }
    ConstantProvince FreeState { get; }
    ConstantProvince Mpumalanga { get; }
    ConstantProvince NorthWest { get; }
    ConstantProvince EasternCape { get; }
    ConstantProvince NorthernCape { get; }
    ConstantProvince WesternCape { get; }
    ConstantProvince Limpopo { get; }
}

public class SouthAfricaConstantCountry : ConstantCountry
{
    public SouthAfricaConstantCountry()
        : base("South Africa", 1)
    {
        Provinces = new SouthAfricaProvinces(this);
    }

    public ISouthAfricaProvinces Provinces { get; }

    private class SouthAfricaProvinces : ISouthAfricaProvinces
    {
        public SouthAfricaProvinces(ConstantCountry southAfrica)
        {
            Gauteng = new ConstantProvince(southAfrica, "Gauteng", 1);
            KwaZuluNatal = new ConstantProvince(southAfrica, "KwaZulu-Natal", 2);
            FreeState = new ConstantProvince(southAfrica, "Free State", 3);
            Mpumalanga = new ConstantProvince(southAfrica, "Mpumalanga", 5);
            NorthWest = new ConstantProvince(southAfrica, "North West", 6);
            EasternCape = new ConstantProvince(southAfrica, "Eastern Cape", 7);
            NorthernCape = new ConstantProvince(southAfrica, "Northern Cape", 8);
            WesternCape = new ConstantProvince(southAfrica, "Western Cape", 9);
            Limpopo = new ConstantProvince(southAfrica, "Limpopo", 14);
        }

        public ConstantProvince Gauteng { get; }
        public ConstantProvince KwaZuluNatal { get; }
        public ConstantProvince FreeState { get; }
        public ConstantProvince Mpumalanga { get; }
        public ConstantProvince NorthWest { get; }
        public ConstantProvince EasternCape { get; }
        public ConstantProvince NorthernCape { get; }
        public ConstantProvince WesternCape { get; }
        public ConstantProvince Limpopo { get; }
    }
}

public class ConstantProvince : ConstantEntity
{
    public ConstantProvince(ConstantCountry country, string name, int id)
        : base(name, id)
    {
        Country = country;
    }

    public ConstantCountry Country { get; }
}

public class ConstantCity : ConstantEntity
{
    public ConstantCity(ConstantProvince province, string name, int id)
        : base(name, id)
    {
        Province = province;
    }

    public ConstantProvince Province { get; }
}

public class ConstantCityAlias : ConstantEntity
{
    public ConstantCityAlias(ConstantProvince province, string name, int id)
        : base(name, id)
    {
        Province = province;
    }

    public ConstantProvince Province { get; set; }
}

public class ConstantSuburbAlias : ConstantEntity
{
    public ConstantSuburbAlias(ConstantCity city, string name, int id)
        : base(name, id)
    {
        City = city;
    }

    public ConstantCity City { get; set; }
}

public class ConstantSuburb : ConstantEntity
{
    public ConstantSuburb(string name, int id)
        : base(name, id)
    {
    }
}