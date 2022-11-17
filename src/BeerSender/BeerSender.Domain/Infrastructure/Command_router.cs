using BeerSender.Domain.Infrastructure.Command_handlers;

namespace BeerSender.Domain.Infrastructure;

public class Command_router<TAggregate>
    where TAggregate : class, new()
{
    private readonly Func<Guid, IEnumerable<object>> _event_stream;
    private readonly Action<Guid, object> _publish_event;

    public Command_router(Func<Guid, IEnumerable<object>> event_stream,
        Action<Guid, object> publish_event)
    {
        _event_stream = event_stream;
        _publish_event = publish_event;
    }

    public void Handle_command<TCommand>(TCommand command)
        where TCommand : Command<TAggregate>
    {
        var handler = FindHandler(command);
        handler.ApplyEventStream(_event_stream(command.AggregateId));
        foreach (var @event in handler.Handle_command(command))
        {
            _publish_event(command.AggregateId, @event);
        }
    }

    private static Command_handler<TAggregate, TCommand> FindHandler<TCommand>(TCommand command)
        where TCommand : Command<TAggregate>
    {
        var type = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .FirstOrDefault(x => typeof(Command_handler<TAggregate, TCommand>).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);

        if (type == null)
            throw new Exception($"Handler not found for type {command.GetType()}");
        var handler = Activator.CreateInstance(type) as Command_handler<TAggregate, TCommand>;
        return handler;
    }
}
