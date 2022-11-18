namespace BeerSender.Domain.Infrastructure;

internal abstract class EventHandlerBase<TEvent, TAggregate>
    where TEvent : IEvent
{
    public abstract void Handle(TAggregate aggregate, TEvent @event);
}