using BeerSender.Domain;
using BeerSender.Domain.Infrastructure;
using FluentAssertions;

namespace BeerSender.Tests;

public abstract class Beer_sender_test<TAggregate>
    where TAggregate : class, new()
{
    private object[] _events;
    private List<object> _resulting_events = new();


    protected void Given(params object[] events)
    {
        _events = events;
    }

    protected void When<TCommand>(TCommand command)
        where TCommand : Command<TAggregate>
    {
        var router = new Command_router<TAggregate>(
            _ => _events,
            (_, @event) => _resulting_events.Add(@event));

        router.Handle_command(command);
    }

    protected void Expect(params object[] expected_events)
    {
        _resulting_events
            .ToArray()
            .Should().BeEquivalentTo(expected_events);
    }
}