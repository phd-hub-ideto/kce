using System.Text;
using Newtonsoft.Json;
using KhumaloCraft.Reflection;
using Microsoft.AspNetCore.Routing;

namespace KhumaloCraft.Application.Mvc;

public interface ICustomRouteValueDictionaryDefaults
{
    void Prepare();
}

public class CustomRouteValueDictionary : RouteValueDictionary
{
    public CustomRouteValueDictionary(IDictionary<string, object> dictionary)
        : base(dictionary ?? new Dictionary<string, object>())
    {
    }

    public CustomRouteValueDictionary(object values = null, bool excludeDefaultValues = false, HashSet<string> excludeMembers = null)
    {
        Merge(values, excludeDefaultValues, excludeMembers);
    }

    public CustomRouteValueDictionary(ActionData actionData)
        : this(actionData.ToRouteValueDictionary())
    {
    }

    private static Dictionary<Type, CustomRouteValueDictionary> _defaultValues = new Dictionary<Type, CustomRouteValueDictionary>();

    private static CustomRouteValueDictionary GetDefaultValues(Type type)
    {
        CustomRouteValueDictionary result;
        lock (_defaultValues)
        {
            if (_defaultValues.TryGetValue(type, out result))
            {
                return result;
            }
        }

        var defaultConstructor = type.GetConstructor(new Type[0]);
        if (defaultConstructor == null)
        {
            throw new Exception($"Type {type.Name} does not have a public parameterless constructor.");
        }

        var instance = defaultConstructor.Invoke(new object[0]);
        if (instance is ICustomRouteValueDictionaryDefaults customRouteValueDictionaryDefaults)
        {
            customRouteValueDictionaryDefaults.Prepare();
        }

        result = new CustomRouteValueDictionary(instance, false);

        lock (_defaultValues)
        {
            _defaultValues[type] = result;
        }

        return result;
    }

    public override int GetHashCode()
    {
        return Count.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(obj, this))
        {
            return true;
        }

        if (!(obj is CustomRouteValueDictionary customRouteValueDictionary))
        {
            return false;
        }

        if (customRouteValueDictionary.Count != Count)
        {
            return false;
        }

        foreach (var item in this)
        {
            if (!customRouteValueDictionary.TryGetValue(item.Key, out object value))
            {
                return false;
            }

            if (item.Value == null)
            {
                if (value != null)
                {
                    return false;
                }
            }
            else
            {
                if (!item.Value.Equals(value))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public CustomRouteValueDictionary Clone()
    {
        return new CustomRouteValueDictionary(this);
    }

    public override string ToString()
    {
        return ToString(null);
    }

    public string ToString(Func<object, CustomRouteValueDictionary> convertValue = null)
    {
        using (var textWriter = new StringWriter())
        {
            using (var writer = new JsonTextWriter(textWriter))
            {
                ToString(writer, convertValue);
            }

            return textWriter.ToString();
        }
    }

    public void ToString(JsonWriter writer, Func<object, CustomRouteValueDictionary> convertValue = null)
    {
        writer.WriteStartObject();
        foreach (var item in this)
        {
            writer.WritePropertyName(item.Key);
            var customRouteValueDictionary = item.Value as CustomRouteValueDictionary;
            if ((customRouteValueDictionary == null) && (convertValue != null) && (item.Value != null))
            {
                customRouteValueDictionary = convertValue(item.Value);
            }
            if (customRouteValueDictionary != null)
            {
                customRouteValueDictionary.ToString(writer, convertValue);
            }
            else
            {
                writer.WriteValue(item.Value?.ToString());
            }
        }

        writer.WriteEndObject();
    }

    public void Merge(IDictionary<string, object> routeValueDictionary, HashSet<string> excludeMembers = null)
    {
        foreach (var item in routeValueDictionary)
        {
            if (excludeMembers?.Contains(item.Key) == true)
            {
                continue;
            }

            this[item.Key] = item.Value;
        }
    }

    public void Merge(object model, bool excludeDefaultValues = false, HashSet<string> excludeMembers = null)
    {
        if (model == null)
        {
            return;
        }

        if (model is IDictionary<string, object> routeValueDictionary)
        {
            Merge(routeValueDictionary, excludeMembers);
        }
        else
        {
            CustomRouteValueDictionary defaultValues = null;

            if (excludeDefaultValues)
            {
                var type = model.GetType();
                if (!type.IsAnonymous()) //exclude anonymous types
                {
                    defaultValues = GetDefaultValues(type);
                }
            }

            foreach (var value in PropertyValue.Get(model))
            {
                if (excludeMembers?.Contains(value.Name) == true)
                {
                    continue;
                }

                var obj2 = value.Value;

                if (defaultValues != null && defaultValues.TryGetValue(value.Name, out object defaultValue))
                {
                    if (ReferenceEquals(obj2, defaultValue))
                    {
                        continue;
                    }

                    if (defaultValue?.Equals(obj2) == true)
                    {
                        continue;
                    }
                }

                if (obj2 != null)
                {
                    var type = value.Type;
                    if (type.IsArray)
                    {
                        var stringBuilder = new StringBuilder();
                        var array = (Array)obj2;

                        if (array.Length == 0)
                        {
                            continue; //don't bother adding empty arrays
                        }

                        var first = true;
                        foreach (var item in array)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                stringBuilder.Append(',');
                            }

                            if (item != null)
                            {
                                stringBuilder.Append(item.ToString());
                            }
                        }

                        obj2 = stringBuilder.ToString();
                    }
                }

                this[value.Name] = obj2;
            }
        }
    }
}
