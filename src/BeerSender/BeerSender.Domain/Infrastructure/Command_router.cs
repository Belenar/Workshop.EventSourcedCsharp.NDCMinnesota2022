using BeerSender.Domain.Infrastructure.Command_handlers;

namespace BeerSender.Domain.Infrastructure;

public class Command_router
{
    private readonly Func<Guid, IEnumerable<object>> _event_stream;
    private readonly Action<object> _publish_event;

    public Command_router(Func<Guid, IEnumerable<object>> event_stream,
        Action<object> publish_event)
    {
        _event_stream = event_stream;
        _publish_event = publish_event;
    }

    public void Handle_command(object command)
    {
        switch (command)
        {
            case Create_package create:
                var handler = new Create_package_handler();
                handler.ApplyEventStream(_event_stream(create.Package_id));
                foreach (var @event in handler.Handle_command(create))
                {
                    _publish_event(@event);
                }
                return;
        }
    }
}
