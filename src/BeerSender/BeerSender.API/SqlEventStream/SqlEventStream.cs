namespace BeerSender.API.SqlEventStream;

public class SqlEventStream
{
    private readonly EventContext _eventContext;

    public SqlEventStream(EventContext eventContext)
    {
        _eventContext = eventContext;
    }

    public IEnumerable<object> GetEvents(Guid aggregateId)
    {
        return _eventContext.PersistedEvent
            .Where(e => e.AggregateId == aggregateId)
            .OrderBy(e => e.CreatedOn)
            .ToList();
    }

    public void PublishEvent(Guid aggregateId, object @event)
    {
        var newEvent = new PersistedEvent
        {
            AggregateId = aggregateId,
            CreatedOn = DateTime.UtcNow,
            Event = @event
        };

        _eventContext.PersistedEvent.Add(newEvent);

        _eventContext.SaveChanges();
    }
}