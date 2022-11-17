namespace BeerSender.Domain;

// Commands
public interface Command { }

public record CreatePackage(Guid PackageId) : Command;
public record AddBeer(Guid PackageId, BeerBottle Beer) : Command;

public record AddShippingLabel(Guid PackageId, ShippingLabel ShippingLabel) : Command;


// Events
public record PackageCreated(Guid PackageId);
public record BeerAdded(Guid PackageId, BeerBottle Beer);
public record BeerFailedToAdd(Guid PackageId, BeerBottle Beer, FailReason FailureReason);

public record ShippingLabelAdded(Guid BeerPackageId, ShippingLabel ShippingLabel);

public record ShippingLabelFailedToAdd(Guid BeerPackageId, ShippingLabel ShippingLabel, ShippingLabelFailure Reason);

public enum FailReason
{
    BoxWasFull
}

public enum ShippingLabelFailure
{
    InvalidLabel,
    UnknownCarrier
}