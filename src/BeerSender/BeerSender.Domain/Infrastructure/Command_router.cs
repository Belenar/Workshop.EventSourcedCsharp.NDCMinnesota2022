namespace BeerSender.Domain.Infrastructure;

public class Command_router
{
    private readonly IEventStream _event_stream;
    private readonly IAggregateCache _aggregateCache;

    public Command_router(IEventStream event_stream, IAggregateCache aggregate_cache)
    {
        _event_stream = event_stream;
        _aggregateCache = aggregate_cache;
    }

    public void Handle_command(ICommand command)
    {
        var actualType = command.GetType();
        var aggregateType = actualType.BaseType.GetGenericArguments()[0];

        var aggregate = _aggregateCache.Get(aggregateType, command.Aggregate_id);

        foreach (var @event in _event_stream.Get_events(command.Aggregate_id))
        {
            InvokeEventHandler(aggregateType, aggregate, @event);
        }

        foreach (var @event in InvokeCommandHandler(command.GetType(), aggregateType, aggregate, command))
        {
            _event_stream.Publish_event(command.Aggregate_id, @event);
        }
    }

    private IEnumerable<IEvent>? InvokeCommandHandler(Type commandType, Type aggregateType, object aggregate, ICommand command)
    {
        var allTypes = typeof(Command_router).Assembly.GetTypes().ToList();
        var genericType = typeof(CommandHandlerBase<,>).MakeGenericType(commandType, aggregateType);

        var commandHandlerType = allTypes
            .FirstOrDefault(t => genericType.IsAssignableFrom(t) && !t.IsAbstract);

        if (commandHandlerType == null)
            throw new Exception($"No command handler registered for {commandType.FullName}");

        var handler = Activator.CreateInstance(commandHandlerType);
        var handleMethod = genericType.GetMethod("Handle");
        return (IEnumerable<IEvent>)handleMethod.Invoke(handler, new[] { aggregate, command });
    }

    private void InvokeEventHandler(Type aggregateType, object aggregate, IEvent @event)
    {
        var allTypes = typeof(Command_router).Assembly.GetTypes().ToList();
        var genericType = typeof(EventHandlerBase<,>).MakeGenericType(@event.GetType(), aggregateType);

        var eventHandlerType = allTypes
            .FirstOrDefault(t => genericType.IsAssignableFrom(t) && !t.IsAbstract);

        if (eventHandlerType == null)
            return;

        var handler = Activator.CreateInstance(eventHandlerType);
        var handleMethod = genericType.GetMethod("Handle");
        handleMethod.Invoke(handler, new[] { aggregate, @event });
    }
}
