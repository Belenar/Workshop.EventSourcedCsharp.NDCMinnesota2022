using BeerSender.Domain;
using BeerSender.Domain.Infrastructure;
using FluentAssertions;

namespace BeerSender.Tests;

public abstract class Beer_sender_test
{
    private object[] _events;
    private List<object> _resulting_events = new();


    protected void Given(params object[] events)
    {
        _events = events;
    }

    protected void When(object command)
    {
        var router = new Command_router(
            _ => _events,
            @event => _resulting_events.Add(@event));

        router.Handle_command(command);
    }

    protected void Expect(params object[] expected_events)
    {
        _resulting_events
            .ToArray()
            .Should().BeEquivalentTo(expected_events);
    }
}