using BeerSender.Domain;
using FluentAssertions;

namespace BeerSender.Tests;

public abstract class BeerSenderTest
{
    private object[] _events;
    private List<object> _resultingEvents = new();

    protected void Given(params object[] events)
    {
        _events = events;
    }

    protected void When(object command)
    {
        var router = new CommandRouter(_ => _events, (_, @event) => _resultingEvents.Add(@event));

        router.HandleCommand(command);
    }

    protected void Expect(params object[] expectedEvents)
    {
        _resultingEvents.ToArray().Should().BeEquivalentTo(expectedEvents);
    }
}