
namespace BeerSender.Domain.Infrastructure.Command_handlers;

internal abstract class Command_handler<TAggregate, TCommand>
    where TAggregate : class, new()

{
    protected TAggregate _aggregate = new();

    public abstract IEnumerable<object> Handle_command(TCommand command);

    protected abstract void Apply(object @event);

    public void ApplyEventStream(IEnumerable<object> event_stream)
    {
        foreach (var @event in event_stream)
        {
            Apply(@event);
        }
    }
}