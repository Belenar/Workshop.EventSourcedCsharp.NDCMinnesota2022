namespace BeerSender.Domain;

public class BeerPackage
{
    public Guid AggregateId { get; private set; }

    public ShippingLabel? ShippingLabel { get; private set; }
    
    public void Apply(object @event)
    {
        switch (@event)
        {
            case PackageCreated createdEvent:
                AggregateId = createdEvent.PackageId;
                break;
            case ShippingLabelAdded shippingLabelAddedEvent:
                ShippingLabel = shippingLabelAddedEvent.ShippingLabel;
                break;
            case ShippingLabelFailedToAdd:
                ShippingLabel = null;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(@event));
        }
    }

    public BeerPackage(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }

    public IEnumerable<object> HandleCommand(object command)
    {
        return command switch
        {
            CreatePackage createCommand => Create(createCommand),
            AddShippingLabel addShippingLabel => ApplyLabel(addShippingLabel.ShippingLabel),
            _ => throw new ArgumentOutOfRangeException(nameof(command))
        };
    }

    private static IEnumerable<object> Create(CreatePackage createCommand)
    {
        yield return new PackageCreated(createCommand.PackageId);
    }

    private IEnumerable<object> ApplyLabel(ShippingLabel shippingLabel)
    {
        if (shippingLabel.IsValid())
        {
            ShippingLabel = shippingLabel;
            yield return new ShippingLabelAdded(AggregateId, ShippingLabel);
        }
        
        else if (!Enum.IsDefined(typeof(Carrier), shippingLabel.Carrier))
        {
            yield return new ShippingLabelFailedToAdd(AggregateId, shippingLabel, ShippingLabelFailure.UnknownCarrier);
        }

        else
        {
            yield return new ShippingLabelFailedToAdd(AggregateId, shippingLabel, ShippingLabelFailure.InvalidLabel);
        }
    }
}