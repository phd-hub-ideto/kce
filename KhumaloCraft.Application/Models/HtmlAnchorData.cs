namespace KhumaloCraft.Application.Models
{
    [System.Diagnostics.DebuggerDisplay(nameof(HtmlAnchorData) + ": "
        + nameof(Text) + " = {" + nameof(Text) + "}" + ", "
        + nameof(Url) + " = {" + nameof(Url) + "}" + ", "
        + nameof(Title) + " = {" + nameof(Title) + "}" + ", "
        + nameof(Target) + " = {" + nameof(Target) + "}" + ", "
        + nameof(AddSeparator) + " = {" + nameof(AddSeparator) + "}"
    )]
    public class HtmlAnchorData
    {
        public HtmlAnchorData()
        {
        }

        public HtmlAnchorData(string textAndTitle, string url, bool addSeparator = false, string target = null, string @class = null)
        {
            Text = textAndTitle;
            Title = textAndTitle;
            Url = url;
            AddSeparator = addSeparator;
            Target = target;
            Class = @class;
        }

        public HtmlAnchorData(string text, string title, string url, bool addSeparator = false, string target = null, string @class = null)
            : this(title, url, addSeparator, target, @class)
        {
            Text = text;
        }

        public string Url { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Class { get; set; }
        public bool AddSeparator { get; set; }
        public string Target { get; set; }
        public string BadgeText { get; set; }
    }
}