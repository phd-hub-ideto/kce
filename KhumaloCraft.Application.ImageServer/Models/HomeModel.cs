using Microsoft.AspNetCore.Mvc.Rendering;

namespace KhumaloCraft.Application.ImageServer.Models.System;

public class HomeModel
{
    public IEnumerable<SelectListItem> ImageSizeOptions
    {
        get
        {
            return
                Enum.GetValues(typeof(Imaging.ImageSizeOption))
                    .Cast<Imaging.ImageSizeOption>()
                    .Select(i => new SelectListItem()
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
        }
    }

    public int? ImageId { get; set; }
    public string ImageSizeOption { get; set; }
}