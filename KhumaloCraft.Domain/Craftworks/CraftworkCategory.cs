using System.ComponentModel;

namespace KhumaloCraft.Domain.Craftworks;

public enum CraftworkCategory
{
    [Description("Woodworking")]
    Woodworking = 1,

    [Description("Embroidery")]
    Embroidery = 2,

    [Description("Pottery")]
    Pottery = 3,

    [Description("Metalworking")]
    Metalworking = 4,

    [Description("Weaving")]
    Weaving = 5,

    [Description("Glassblowing")]
    Glassblowing = 6,

    [Description("Tie Dye")]
    TieDye = 7,

    [Description("Sculpture")]
    Sculpture = 8,

    [Description("Sewing")]
    Sewing = 9,

    [Description("Printmaking")]
    Printmaking = 10,

    [Description("Beadwork")]
    Beadwork = 11,

    [Description("Recycled Crafts")]
    RecycledCrafts = 12,

    [Description("Appliqué")]
    Appliqué = 13,

    [Description("Modelmaking")]
    Modelmaking = 14,

    [Description("Leatherworking")]
    Leatherworking = 15,

    [Description("Knitting")]
    Knitting = 16,

    [Description("Traditional Crafts")]
    TraditionalCrafts = 17,

    [Description("Digital Art")]
    DigitalArt = 18,

    [Description("Mixed Media")]
    MixedMedia = 19,

    [Description("Needlepoint")]
    Needlepoint = 20,

    [Description("Stone Carving")]
    StoneCarving = 21,

    [Description("Paper Crafts")]
    PaperCrafts = 22,

    [Description("Collage")]
    Collage = 23,

    [Description("Textiles")]
    Textiles = 24,

    [Description("Misc")]
    Misc = 25
}