using BeerSender.Domain.Infrastructure;

namespace BeerSender.Domain;

public class Create_package
{
    public record CreatePackageCommand(Guid Package_id, Package_capacity Capacity) : BaseCommand<Beer_package>(Package_id);

    public record Success(Guid Package_id, Package_capacity Capacity) : IEvent;

    internal class CommandHandler
        : CommandHandlerBase<CreatePackageCommand, Beer_package>
    {
        public override IEnumerable<IEvent> Handle(Beer_package aggregate, CreatePackageCommand createPackageCommand)
        {
            yield return new Success(createPackageCommand.Package_id, createPackageCommand.Capacity);
        }
    }

    internal class SuccessEventHandler : EventHandlerBase<Success, Beer_package>
    {
        public override void Handle(Beer_package aggregate, Success @event)
        {
            aggregate.Create(@event.Package_id, @event.Capacity);
        }
    }
}