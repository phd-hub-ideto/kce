namespace KhumaloCraft.Application.Attributes.TypeScript;

//
// Summary:
//     Specifies that a parameter should be ignored when generating a TypeScript client.
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class GeneratedClientIgnoreParameterAttribute : Attribute
{

}