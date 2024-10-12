namespace KhumaloCraft.Application.Models.Breadcrumb;

public class BreadcrumbModel
{
    public List<HtmlAnchorData> Breadcrumbs { get; }

    public BreadcrumbModel(List<HtmlAnchorData> breadcrumbs)
    {
        Breadcrumbs = breadcrumbs;
    }
}