namespace KhumaloCraft.Domain;

/// <summary>
/// Identifiers have an integer identifier, "Id", and a string name, "Name".
/// </summary>
public interface IIdentifier
{
    int Id { get; }
    string Name { get; }
}