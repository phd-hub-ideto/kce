namespace KhumaloCraft.Application.Exceptions
{
    public class MappingException : Exception
    {
        public static MappingException EnumeratedTypeMapping(Type enumeratedTypeName, string value)
        {
            return new MappingException($"Type: {enumeratedTypeName.Name}, value: {value}");
        }

        public static MappingException MapperUnitValueException(Type enumeratedTypeName, string unit, string value)
        {
            return new MappingException($"Type: {enumeratedTypeName.Name}, Unit: {unit}, Value: {value}");
        }

        public MappingException() : base()
        {
        }

        public MappingException(string message) : base(message)
        {
        }

        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
