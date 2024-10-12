namespace KhumaloCraft;

public interface ITemplateNameProvider
{
    bool TryGetTemplateName(out string templateName);
}