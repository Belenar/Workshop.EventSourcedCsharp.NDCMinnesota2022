namespace BeerSender.Domain.Infrastructure.Command_handlers;

internal class Create_package_handler
    : BeerPackageHandler<Create_package>
{
    public override IEnumerable<object> Handle_command(Create_package command)
    {
        yield return new Package_created(command.Package_id, command.Capacity);
    }
}