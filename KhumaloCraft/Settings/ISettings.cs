namespace KhumaloCraft;

public interface ISettings
{
    [UserEditableSetting]
    int ConcurrentEmailProcessCount { get; }

    [UserEditableSetting("Email Queue: Number of SMTP failures (per thread task) to tolerate before stopping processing")]
    int EmailQueueSmtpFaultTolerance { get; }

    [UserEditableSetting("Email: Enable email domain checking")]
    bool EnableEmailDomainChecking { get; }

    [UserEditableSetting("Interest rate: (prime rate)")]
    decimal InterestRate { get; }

    uint MaximumImagePayloadSizeMB { get; }
    uint MaximumImageSizeMB { get; }
    int MinimumImageSizeB { get; }
    string NoReplyEmailEddress { get; }
    string Version { get; }
    string PortalBaseUri { get; }
    string ImageServerBaseUri { get; }
    string ImageServerFallbackServer { get; }

    string BranchName { get; }
    bool EnableTaskScheduler { get; }

    string SmtpServer { get; }
    string SmtpLoginUsername { get; }
    string SmtpLoginPassword { get; }

    decimal VatRate { get; }

    string AzureBlobStoragePrimary { get; }
    string AzureAdCertThumbprint { get; }
    string AzureAdApplicationId { get; }
    string AzureAdDirectoryId { get; }
    string AzureKCResourceConnectionString { get; }

    //TODO-LP : Setup SMS (OTP)
    //string SMSPortalClientId { get; }
    //string SMSPortalSecret { get; }

    [UserEditableSetting("Rate Limit Requests Per Second")]
    int RateLimitRequestsPerSecond { get; }

    string DataProtectionKey { get; }
    string CookieProtectionKey { get; }
    bool UseHostNameForCookies { get; }
}