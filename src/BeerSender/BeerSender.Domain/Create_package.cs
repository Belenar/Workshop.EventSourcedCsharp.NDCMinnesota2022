using BeerSender.Domain.Infrastructure;

namespace BeerSender.Domain;

public class Create_package
{
    public record Command(Guid Package_id, Package_capacity Capacity) : BaseCommand<Beer_package>(Package_id);

    public record Success(Guid Package_id, Package_capacity Capacity) : IEvent;

    internal class CommandHandler
        : CommandHandlerBase<Command, Beer_package>
    {
        public override IEnumerable<IEvent> Handle(Beer_package aggregate, Command command)
        {
            yield return new Success(command.Package_id, command.Capacity);
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