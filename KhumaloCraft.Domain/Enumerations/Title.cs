using System.ComponentModel;

namespace KhumaloCraft.Domain.Enumerations;

public enum Title
{
    [Description("Mr")]
    Mr = 1,

    [Description("Ms")]
    Ms = 2,

    [Description("Mrs")]
    Mrs = 3,

    [Description("Miss")]
    Miss = 4,

    [Description("Doctor")]
    Doctor = 5,

    [Description("Reverend")]
    Reverend = 6,

    [Description("Professor")]
    Professor = 7,

    [Description("Mr and Mrs")]
    MrAndMrs = 8,

    [Description("Advocate")]
    Advocate = 9,
}