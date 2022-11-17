using System.Diagnostics.CodeAnalysis;

namespace BeerSender.Domain;

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
        switch(command) {
            case Create_package create:
                var aggregate = new Beer_package();
                foreach (var @event in _event_stream(create.Package_id))
                {
                    aggregate.Apply(@event);
                }

                foreach (var @event in aggregate.Handle_command(create))
                {
                    _publish_event(@event);
                }
                
                return;
        }
    }
}
