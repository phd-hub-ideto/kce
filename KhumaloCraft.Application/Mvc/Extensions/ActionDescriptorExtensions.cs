using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace KhumaloCraft.Mvc;

public static class ActionDescriptorExtensions
{
    /// <summary>
    /// Returns all attributes of the given type that have been applied to the current action, via the controller or the action. Includes
    /// attributes inherited from base controller types. USE THIS METHOD instead of the standard GetCustomAttributes() or
    /// GetFilterAttributes() methods, as these will not correctly return attributes in the inheritance hierarchy.
    /// </summary>
    /// <typeparam name="T">The type of the attributes to return.</typeparam>
    /// <param name="actionDescriptor"></param>
    public static T[] GetAttributes<T>(this ActionDescriptor actionDescriptor)
        where T : Attribute
    {
        if (actionDescriptor is ControllerActionDescriptor reflectedActionDescriptor)
        {
            return GetAttributesSync<T>(reflectedActionDescriptor);
        }

        throw new InvalidOperationException();
    }

    private static T[] GetAttributesSync<T>(ControllerActionDescriptor reflectedActionDescriptor)
        where T : Attribute
    {
        var controllerType = reflectedActionDescriptor.ControllerTypeInfo;
        var controllerName = reflectedActionDescriptor.ControllerName;

        var foundAttributes = controllerType.GetCustomAttributes<T>(true).ToList();

        var actionMethod = reflectedActionDescriptor.MethodInfo;
        if (actionMethod == null)
            throw new Exception($"Unable to find action {reflectedActionDescriptor.MethodInfo.Name} on controller {controllerName}.");

        foundAttributes.AddRange(actionMethod.GetCustomAttributes<T>(true));

        return foundAttributes.ToArray();
    }

    /// <summary>
    /// Returns a value indicating whether no attributes of the given type have been applied to the current action, via the controller or the action. Includes
    /// attributes inherited from base controller types. USE THIS METHOD instead of the standard GetCustomAttributes() or
    /// GetFilterAttributes() methods, as these will not correctly return attributes in the inheritance hierarchy.
    /// </summary>
    /// <typeparam name="T">The type of the attribute to test.</typeparam>
    /// <param name="actionDescriptor"></param>
    public static bool NoAttributes<T>(this ActionDescriptor actionDescriptor)
        where T : Attribute
    {
        return actionDescriptor.GetAttributes<T>().IsNullOrEmpty();
    }

    /// <summary>
    /// Returns a value indicating whether any attributes of the given type have been applied to the current action, via the controller or the action. Includes
    /// attributes inherited from base controller types. USE THIS METHOD instead of the standard GetCustomAttributes() or
    /// GetFilterAttributes() methods, as these will not correctly return attributes in the inheritance hierarchy.
    /// </summary>
    /// <typeparam name="T">The type of the attribute to test.</typeparam>
    /// <param name="actionDescriptor"></param>
    public static bool AnyAttributes<T>(this ActionDescriptor actionDescriptor) where T : Attribute
    {
        return !actionDescriptor.GetAttributes<T>().IsNullOrEmpty();
    }
}