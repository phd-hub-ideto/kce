using System.ComponentModel;

namespace KhumaloCraft.Domain.Platforms;

// this enum MUST be kept in sync with the UIContext table!
public enum Platform
{
    Unknown = 0,

    Desktop = 1,

    SmartPhone = 2,

    [Description("Android")]
    Android = 3,

    [Description("iOS")]
    iOS = 4,

    Manage = 5,
}
