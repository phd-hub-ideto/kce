using System.Reflection;
using System.Reflection.Emit;

namespace KhumaloCraft.Reflection;

// see https://stackoverflow.com/q/10698915/70345
public static class DynamicProperty
{
    public delegate object Getter(object target);

    public delegate void Setter(object target, object value);

    public static Getter CreateGetter(PropertyInfo propertyInfo)
    {
        var getMethod = propertyInfo.GetGetMethod() ?? propertyInfo.GetGetMethod(true);
        if (getMethod == null)
            return null;

        var arguments = new Type[1];
        arguments[0] = typeof(object);

        var getter = new DynamicMethod(string.Concat("_Get", propertyInfo.Name, "_"), typeof(object), arguments, propertyInfo.Module);

        var generator = getter.GetILGenerator();
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
        generator.EmitCall(OpCodes.Callvirt, getMethod, null);

        if (!propertyInfo.PropertyType.IsClass)
            generator.Emit(OpCodes.Box, propertyInfo.PropertyType);

        generator.Emit(OpCodes.Ret);

        return (Getter)getter.CreateDelegate(typeof(Getter));
    }

    public static Setter CreateSetter(PropertyInfo propertyInfo)
    {
        var setMethod = propertyInfo.GetSetMethod() ?? propertyInfo.GetSetMethod(true);
        if (setMethod == null)
            return null;

        var arguments = new Type[2];
        arguments[0] = arguments[1] = typeof(object);

        var setter = new DynamicMethod(string.Concat("_Set", propertyInfo.Name, "_"), typeof(void), arguments, propertyInfo.Module);

        var generator = setter.GetILGenerator();
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
        generator.Emit(OpCodes.Ldarg_1);

        if (propertyInfo.PropertyType.IsClass)
            generator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
        else
            generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);

        generator.EmitCall(OpCodes.Callvirt, setMethod, null);
        generator.Emit(OpCodes.Ret);

        return (Setter)setter.CreateDelegate(typeof(Setter));
    }
}
