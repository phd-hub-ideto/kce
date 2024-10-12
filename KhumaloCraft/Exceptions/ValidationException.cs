using System.Runtime.Serialization;
using System.Text;

namespace KhumaloCraft.Exceptions;

[Serializable]
public class ValidationException : Exception, ISerializable
{
    public string[] ErrorMessages { get; private set; }

    public ValidationException(string errorMessage)
        : base(errorMessage)
    {
        ErrorMessages = new[] { errorMessage };
    }

    public ValidationException(Type invalidType, params string[] errorMessages)
        : base("Failed to validate " + invalidType.Name + ".")
    {
        ErrorMessages = errorMessages;
    }

    public string ToAggregateMessage(string separator)
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(Message);

        if (ErrorMessages.Length != 0 && (ErrorMessages.Length != 1 || ErrorMessages[0] != Message))
        {
            foreach (var errorMessage in ErrorMessages)
            {
                stringBuilder.Append(separator);

                stringBuilder.AppendLine(errorMessage);
            }
        }

        return stringBuilder.ToString();
    }

    public override string ToString()
    {
        var aggregateMessage = ToAggregateMessage(Environment.NewLine);

        string stackTrace = StackTrace;

        if (stackTrace != null)
            return aggregateMessage + Environment.NewLine + stackTrace;

        return aggregateMessage;
    }
}
