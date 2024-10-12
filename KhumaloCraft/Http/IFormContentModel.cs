using System.Reflection;

namespace KhumaloCraft.Http;

public interface IFormContentModel
{
    IEnumerable<KeyValuePair<string, string>> ToNameValueCollection(BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);
}