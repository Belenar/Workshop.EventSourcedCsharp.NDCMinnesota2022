using BeerSender.Domain.Infrastructure.Command_handlers;

namespace BeerSender.Domain.Infrastructure;

public class Command_router
{
    private readonly Func<Guid, IEnumerable<object>> _event_stream;
    private readonly Action<Guid, object> _publish_event;

    public Command_router(Func<Guid, IEnumerable<object>> event_stream,
        Action<Guid, object> publish_event)
    {
        _event_stream = event_stream;
        _publish_event = publish_event;
    }

    public void Handle_command(object command)
    {
        switch (command)
        {
            case Create_package create:
                var create_handler = new Create_package_handler();
                create_handler.ApplyEventStream(_event_stream(create.Package_id));
                foreach (var @event in create_handler.Handle_command(create))
                {
                    _publish_event(create.Package_id, @event);
                }
                return;
            case Add_beer add_beer:
                var add_bottle_handler = new Add_bottle_handler();
                add_bottle_handler.ApplyEventStream(_event_stream(add_beer.Package_id));
                foreach (var @event in add_bottle_handler.Handle_command(add_beer))
                {
                    _publish_event(add_beer.Package_id, @event);
                }
                return;
        }
    }
}
