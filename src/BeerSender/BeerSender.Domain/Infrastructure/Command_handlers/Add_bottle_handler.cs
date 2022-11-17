namespace BeerSender.Domain.Infrastructure.Command_handlers;

internal class Add_bottle_handler
    : BeerPackageHandler<Add_beer>
{
    public override IEnumerable<object> Handle_command(Add_beer command)
    {
        if (_aggregate.Capacity.Capacity <= _aggregate.Contents.Count)
            yield return new Beer_failed_to_add(command.Package_id, command.beer, Fail_reason.Box_was_full);
        else
            yield return new Beer_added(command.Package_id, command.beer);
    }
}