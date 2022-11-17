namespace BeerSender.Domain;

// Commands
public interface Command<TAggregate>
    where TAggregate : class
{
    Guid AggregateId { get; }
}

public record BeerPackageCommand(Guid AggregateId) : Command<Beer_package>;

public record Create_package(Guid AggregateId, Package_capacity Capacity) : BeerPackageCommand(AggregateId);

public record Add_beer(Guid AggregateId, Beer_bottle beer) : BeerPackageCommand(AggregateId);

// Events
public record Package_created(Guid Package_id, Package_capacity Capacity);
public record Beer_added(Guid package_id, Beer_bottle beer);

public record Beer_failed_to_add(Guid package_id, Beer_bottle beer, Fail_reason reason);

public enum Fail_reason
{
    Box_was_full
}