using BeerSender.Domain.Infrastructure;

namespace BeerSender.Domain;

public class Add_beer
{
    public record Command(Guid Package_id, Beer_bottle beer) : BaseCommand<Beer_package>(Package_id);
    public record Success(Guid package_id, Beer_bottle beer) : IEvent;
    public record Fail(Guid package_id, Beer_bottle beer, Fail_reason reason) : IEvent;

    public enum Fail_reason
    {
        Box_was_full
    }

    internal class CommandHandler
        : CommandHandlerBase<Command, Beer_package>
    {
        public override IEnumerable<IEvent> Handle(Beer_package aggregate, Command command)
        {
            if (aggregate.Capacity.Capacity <= aggregate.Contents.Count)
                yield return new Fail(command.Aggregate_id, command.beer, Fail_reason.Box_was_full);
            else
                yield return new Success(command.Aggregate_id, command.beer);
        }
    }

    internal class SuccessEventHandler : EventHandlerBase<Success, Beer_package>
    {
        public override void Handle(Beer_package aggregate, Success @event)
        {
            aggregate.Add_beer(@event.beer);
        }
    }
}