namespace BeerSender.Domain.Infrastructure;

public record AggregateEvent<TEvent>(Guid Aggregate_id, TEvent Event)
    where TEvent : IEvent;

public class Command_router
{
    private readonly IEventStream _event_stream;
    private readonly IAggregateCache _aggregateCache;

    public Command_router(IEventStream event_stream, IAggregateCache aggregate_cache)
    {
        _event_stream = event_stream;
        _aggregateCache = aggregate_cache;
    }

    public void Handle_command<TCommand, TAggregate>(TCommand command)
        where TCommand : ICommand<TAggregate>
        where TAggregate : class, new()
    {
        var aggregate = _aggregateCache.Get<TAggregate>(command.Aggregate_id);

        foreach (var @event in _event_stream.Get_events(command.Aggregate_id))
        {
            InvokeEventHandler(aggregate, @event);
        }

        var commandHandler = GetCommandHandler<TCommand, TAggregate>();
        foreach (var @event in commandHandler.Handle(aggregate, command))
        {
            _event_stream.Publish_event(command.Aggregate_id, @event);
        }
    }

    private CommandHandlerBase<TCommand, TAggregate> GetCommandHandler<TCommand, TAggregate>()
        where TCommand : ICommand<TAggregate>
        where TAggregate : class, new()
    {
        var allTypes = typeof(Command_router).Assembly.GetTypes().ToList();
        var genericType = typeof(CommandHandlerBase<,>).MakeGenericType(typeof(TCommand), typeof(TAggregate));

        var commandHandlerType = allTypes
            .Where(t => genericType.IsAssignableFrom(t) && !t.IsAbstract)
            .FirstOrDefault();

        return (CommandHandlerBase<TCommand, TAggregate>)Activator.CreateInstance(commandHandlerType);
    }

    private void InvokeEventHandler<TAggregate>(TAggregate aggregate, IEvent @event)
        where TAggregate : class, new()
    {
        var allTypes = typeof(Command_router).Assembly.GetTypes().ToList();
        var genericType = typeof(EventHandlerBase<,>).MakeGenericType(@event.GetType(), typeof(TAggregate));

        var eventHandlerType = allTypes
            .Where(t => genericType.IsAssignableFrom(t) && !t.IsAbstract)
            .FirstOrDefault();

        var handler = Activator.CreateInstance(eventHandlerType);
        var handleMethod = genericType.GetMethod("Handle");
        handleMethod.Invoke(handler, new object[] { aggregate, @event });
    }
}
