using System.ComponentModel;

namespace KhumaloCraft.Domain.Enumerations;

public enum MaritalStatusType
{
    [Description("Unmarried")]
    Unmarried = 1,

    [Description("Married In Community of Property")]
    MarriedInCommunityOfProperty = 2,

    [Description("Married Out of Community of Property")]
    MarriedOutOfCommunityOfProperty = 3
}