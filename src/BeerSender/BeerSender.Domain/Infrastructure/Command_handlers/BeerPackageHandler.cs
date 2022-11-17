namespace BeerSender.Domain.Infrastructure.Command_handlers;

internal abstract class BeerPackageHandler<TCommand> 
    : Command_handler<Beer_package, TCommand>
{
    protected override void Apply(object @event)
    {
        switch (@event)
        {
            case Package_created created_event:
                _aggregate.Create(created_event.Package_id, created_event.Capacity);
                break;
            case Beer_added beer_added_event:
                _aggregate.Add_beer(beer_added_event.beer);
                break;
        }
    }
}