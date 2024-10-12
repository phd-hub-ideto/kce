namespace KhumaloCraft.Domain.Events;

public interface IDomainEventMediator
{
    void RaiseEvent<T>(T domainEvent) where T : IDomainEvent;
}