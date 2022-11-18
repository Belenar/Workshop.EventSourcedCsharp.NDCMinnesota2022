namespace BeerSender.Domain.Infrastructure;

public interface IEventStream
{
    IEnumerable<IEvent> Get_events(Guid aggregate_id);
    void Publish_event(Guid aggregate_id, IEvent @event);
}