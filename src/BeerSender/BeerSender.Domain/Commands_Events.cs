namespace BeerSender.Domain;

// Commands
public interface Command {}
public record Create_package(Guid Package_id, Package_capacity Capacity): Command;
public record Add_beer(Guid Package_id, Beer_bottle beer): Command;

// Events
public record Package_created(Guid Package_id, Package_capacity Capacity);
public record Beer_added(Guid package_id, Beer_bottle beer);

public record Beer_failed_to_add(Guid package_id, Beer_bottle beer, Fail_reason reason);

public enum Fail_reason
{
    Box_was_full
}