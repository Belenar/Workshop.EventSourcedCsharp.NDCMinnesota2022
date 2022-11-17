namespace BeerSender.Domain;

public class Command_router
{
    private readonly Func<Guid, IEnumerable<object>> _eventStream;
    private readonly Action<object> _publishEvent;

    public Command_router(Func<Guid, IEnumerable<object>> event_stream,
        Action<object> publish_event)
    {
        _eventStream = event_stream;
        _publishEvent = publish_event;
    }

    public void Handle_command(object command)
    {

    }
}
