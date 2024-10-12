using KhumaloCraft.Helpers;

namespace KhumaloCraft.Application.Models.Shared;

public class SelectListItemModel<T>
{
    public T Value { get; set; }

    public string Text { get; set; }

    public bool? Disabled { get; set; }

    public SelectListItemModel()
    {

    }

    public SelectListItemModel(T value, string text, bool? disabled = null)
    {
        Value = value;
        Text = text;
        Disabled = disabled;
    }
}

public static class SelectListItemModel
{
    public static readonly SelectListItemModel<string> Empty = new SelectListItemModel<string> { Value = string.Empty, Text = string.Empty, };
    public static readonly SelectListItemModel<int?> EmptyNumericItem = new SelectListItemModel<int?> { Value = null, Text = string.Empty };

    public static List<SelectListItemModel<T>> CreateDownDropListForEnum<T>() where T : struct, Enum
    {
        var items = new List<SelectListItemModel<T>>();

        foreach (var item in EnumHelper.GetValues<T>())
        {
            items.Add(new SelectListItemModel<T>(item, item.GetBestDescription()));
        }

        return items;
    }

    public static List<SelectListItemModel<T>> CreateDownDropListForEnumExcluding<T>(T[] valuesToExclude) where T : struct, Enum
    {
        var items = new List<SelectListItemModel<T>>();

        foreach (var item in EnumHelper.GetValues<T>(item => !valuesToExclude.Contains(item)))
        {
            items.Add(new SelectListItemModel<T>(item, item.GetBestDescription()));
        }

        return items;
    }

    public static List<SelectListItemModel<T>> CreateDropDownListForEnumOrderedByText<T>() where T : struct, Enum
    {
        return EnumHelper.GetValues<T>()
            .OrderBy(item => item.GetBestDescription())
            .Select(item => new SelectListItemModel<T>(item, item.GetBestDescription())).ToList();
    }
}