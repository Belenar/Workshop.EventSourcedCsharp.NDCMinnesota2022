using BeerSender.Domain;
using BeerSender.Domain.Infrastructure;
using FluentAssertions;

namespace BeerSender.Tests;

public class TestEventStream : IEventStream
{
    private readonly Func<Guid, IEnumerable<IEvent>> _event_stream;
    private readonly Action<Guid, IEvent> _publish_event;

    public TestEventStream(Func<Guid, IEnumerable<IEvent>> event_stream,
        Action<Guid, IEvent> publish_event)
    {
        _event_stream = event_stream;
        _publish_event = publish_event;
    }

    public IEnumerable<IEvent> Get_events(Guid aggregate_id) => _event_stream(aggregate_id);

    public void Publish_event(Guid aggregate_id, IEvent @event)
    {
        _publish_event(aggregate_id, @event);
    }
}

public abstract class Beer_sender_test<TAggregate>
    where TAggregate : class, new()
{
    private IEvent[] _events;
    private List<object> _resulting_events = new();

    protected void Given(params IEvent[] events)
    {
        _events = events;
    }

    protected void When<TCommand>(TCommand command)
        where TCommand : ICommand<TAggregate>
    {
        var eventStream = new TestEventStream(_ => _events,
            (_, @event) => _resulting_events.Add(@event));
        var router = new Command_router(eventStream, new AggregateCache());

        router.Handle_command(command);
    }

    protected void Expect(params object[] expected_events)
    {
        _resulting_events
            .ToArray()
            .Should().BeEquivalentTo(expected_events);
    }
}