namespace KhumaloCraft.Domain.Events;

public interface IEventHandler<in T> where T : IDomainEvent
{
    void HandleEventAfterScope(T domainEvent);
    void HandleEventDuringScope(T domainEvent);
}