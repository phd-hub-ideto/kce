using System.ComponentModel;

namespace KhumaloCraft.Domain.Team;

public enum Role
{
    [Description("Founder & CEO")]
    FounderAndCeo = 1,

    [Description("Chief Operating Officer")]
    ChiefOperatingOfficer = 2,

    [Description("Chief Technical Officer")]
    ChiefTechnicalOfficer = 3,

    [Description("Head of HR")]
    HeadOfHr = 4,

    [Description("Marketing Director")]
    MarketingDirector = 5,
}
