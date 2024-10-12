using KhumaloCraft.Dependencies;
using SimpleInjector;

namespace KhumaloCraft.Domain.Events;

[Singleton]
public class DomainEventMediator : IDomainEventMediator
{
    private readonly Container _container;
    private readonly ITransactionProvider _transactionProvider;

    public DomainEventMediator(Container container, ITransactionProvider transactionProvider)
    {
        _container = container;
        _transactionProvider = transactionProvider;
    }

    public void RaiseEvent<T>(T domainEvent) where T : IDomainEvent
    {
        // PM: Do not put event specific code here. Ever!

        var eventHandlers = _container.GetAllInstances<IEventHandler<T>>();

        foreach (var eventHandler in eventHandlers)
        {
            eventHandler.HandleEventDuringScope(domainEvent);
        }

        var transaction = _transactionProvider.GetTransaction();

        if (transaction != null)
        {
            transaction.AfterCommit(() =>
            {
                InternalRaiseEvent(domainEvent);
            });
        }
        else
        {
            InternalRaiseEvent(domainEvent);
        }
    }

    private void InternalRaiseEvent<T>(T domainEvent) where T : IDomainEvent
    {
        List<Exception> exceptions = null;

        var eventHandlers = _container.GetAllInstances<IEventHandler<T>>();
        foreach (var eventHandler in eventHandlers)
        {
            try
            {
                eventHandler.HandleEventAfterScope(domainEvent);
            }
            catch (Exception ex)
            {
                exceptions = exceptions ?? new List<Exception>();
                exceptions.Add(ex);
            }
        }

        if (exceptions?.Count > 0)
        {
            throw new AggregateException($"Exceptions while raising domain event {nameof(T)}", exceptions.ToArray());
        }
    }
}