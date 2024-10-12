namespace KhumaloCraft.Imaging;

// if you change this enum, you MUST also change Core.ImageSize!
[Serializable]
public enum ImageSizeOption
{
    Original,

    [ImageSize(ImageResizeOption.EnsurePanoramic, 1438, 430)]
    Panoramic1438x430,

    [ImageSize(ImageResizeOption.Ensure, 1920, 1080)]
    Ensure1920x1080,
    [ImageSize(ImageResizeOption.Ensure, 960, 540)]
    Ensure960x540,
    [ImageSize(ImageResizeOption.Ensure, 360, 270)]
    Ensure360x270,
    [ImageSize(ImageResizeOption.Ensure, 320, 80)]
    Ensure320x80,

    [ImageSize(ImageResizeOption.Crop, 800, 600)]
    Crop800x600,
    [ImageSize(ImageResizeOption.Crop, 676, 507)]
    Crop676x507,
    [ImageSize(ImageResizeOption.Crop, 640, 480)]
    Crop640x480,
    [ImageSize(ImageResizeOption.Crop, 600, 400)]
    Crop600x400,
    [ImageSize(ImageResizeOption.Crop, 460, 345)]
    Crop460x345,
    [ImageSize(ImageResizeOption.Crop, 508, 373)]
    Crop508x373,
    [ImageSize(ImageResizeOption.Crop, 481, 298)]
    Crop481x298,
    [ImageSize(ImageResizeOption.Crop, 350, 210)]
    Crop350x210,
    [ImageSize(ImageResizeOption.Crop, 360, 270)]
    Crop360x270,
    [ImageSize(ImageResizeOption.Crop, 328, 246)]
    Crop328x246,
    [ImageSize(ImageResizeOption.Crop, 320, 80)]
    Crop320x80,
    [ImageSize(ImageResizeOption.Crop, 286, 215)]
    Crop286x215,
    [ImageSize(ImageResizeOption.Crop, 280, 275)]
    Crop280x275,
    [ImageSize(ImageResizeOption.Crop, 280, 239)]
    Crop280x239,
    [ImageSize(ImageResizeOption.Crop, 280, 210)]
    Crop280x210,
    [ImageSize(ImageResizeOption.Crop, 237, 198)]
    Crop237x198,
    [ImageSize(ImageResizeOption.Crop, 213, 160)]
    Crop213x160,
    [ImageSize(ImageResizeOption.Crop, 190, 190)]
    Crop190x190,
    [ImageSize(ImageResizeOption.Crop, 160, 120)]
    Crop160x120,
    [ImageSize(ImageResizeOption.Crop, 160, 40)]
    Crop160x40,
    [ImageSize(ImageResizeOption.Crop, 150, 90)]
    Crop150x90,
    [ImageSize(ImageResizeOption.Crop, 120, 55)]
    Crop120x55,
    [ImageSize(ImageResizeOption.Crop, 120, 65)]
    Crop120x65,
    [ImageSize(ImageResizeOption.Crop, 120, 72)]
    Crop120x72,
    [ImageSize(ImageResizeOption.Crop, 120, 82)]
    Crop120x82,
    [ImageSize(ImageResizeOption.Crop, 107, 80)]
    Crop107x80,
    [ImageSize(ImageResizeOption.Crop, 106, 65)]
    Crop106x65,
    [ImageSize(ImageResizeOption.Crop, 103, 103)]
    Crop103x103,
    [ImageSize(ImageResizeOption.Crop, 88, 51)]
    Crop88x51,
    [ImageSize(ImageResizeOption.Crop, 71, 53)]
    Crop71x53,

    [ImageSize(ImageResizeOption.FitInside, 280, 275)]
    Fit280x275,
    [ImageSize(ImageResizeOption.FitInside, 320, 240)]
    Fit320x240,
    [ImageSize(ImageResizeOption.FitInside, 508, 373)]
    Fit508x373,
    [ImageSize(ImageResizeOption.FitInside, 280, 210)]
    Fit280x210,
    [ImageSize(ImageResizeOption.FitInside, 237, 198)]
    Fit237x198,
    [ImageSize(ImageResizeOption.FitInside, 213, 160)]
    Fit213x160,
    [ImageSize(ImageResizeOption.FitInside, 190, 190)]
    Fit190x190,
    [ImageSize(ImageResizeOption.FitInside, 160, 40)]
    Fit160x40,
    [ImageSize(ImageResizeOption.FitInside, 160, 120)]
    Fit160x120,
    [ImageSize(ImageResizeOption.FitInside, 144, 144)]
    Fit144x144,
    [ImageSize(ImageResizeOption.FitInside, 120, 65)]
    Fit120x65,
    [ImageSize(ImageResizeOption.FitInside, 107, 80)]
    Fit107x80,
    [ImageSize(ImageResizeOption.FitInside, 90, 120)]
    Fit90x120,
    [ImageSize(ImageResizeOption.FitInside, 88, 51)]
    Fit88x51,
    [ImageSize(ImageResizeOption.FitInside, 71, 53)]
    Fit71x53,
    [ImageSize(ImageResizeOption.FitInside, 100, 130)]
    Fit100x130,
    [ImageSize(ImageResizeOption.FitInside, 140, 182)]
    Fit140x182,
    [ImageSize(ImageResizeOption.FitInside, 119, 40)]
    Fit119x40,
    [ImageSize(ImageResizeOption.FitInside, 640, 480)]
    Fit640x480,
    [ImageSize(ImageResizeOption.FitInside, 320, 80)]
    Fit320x80,
}