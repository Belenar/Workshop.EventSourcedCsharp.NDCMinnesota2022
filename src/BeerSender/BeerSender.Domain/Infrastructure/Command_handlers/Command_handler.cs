
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

internal abstract class BeerPackageHandler<TCommand> 
    : Command_handler<Beer_package, TCommand>
{
    protected override void Apply(object @event)
    {
        switch (@event)
        {
            case Package_created created_event:
                _aggregate.Create(created_event.Package_id);
                break;
        }
    }
}

internal class Create_package_handler
    : BeerPackageHandler<Create_package>
{
    public override IEnumerable<object> Handle_command(Create_package command)
    {
        yield return new Package_created(command.Package_id);
    }
}
