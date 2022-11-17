namespace BeerSender.Domain;

public class CommandRouter
{
    private readonly Func<Guid, IEnumerable<object>> _eventStream;
    private readonly Action<Guid, object> _publishEvent;
    private BeerPackage _beerPackage = null!;

    public CommandRouter(Func<Guid, IEnumerable<object>> eventStream, Action<Guid, object> publishEvent)
    {
        _eventStream = eventStream;
        _publishEvent = publishEvent;
    }

    public void HandleCommand(object command)
    {
        switch (command)
        {
            case CreatePackage create:
                _beerPackage = new BeerPackage(create.PackageId);
                foreach (var @event in _eventStream(create.PackageId))
                {
                    _beerPackage.Apply(@event);
                }

                foreach (var @event in _beerPackage.HandleCommand(create))
                {
                    _publishEvent(create.PackageId, @event);
                }
                return;
            
            case AddShippingLabel addShippingLabel:
                _beerPackage = new BeerPackage(addShippingLabel.PackageId);
                foreach (var @event in _eventStream(addShippingLabel.PackageId))
                {
                    _beerPackage.Apply(@event);
                }

                foreach (var @event in _beerPackage.HandleCommand(addShippingLabel))
                {
                    _publishEvent(addShippingLabel.PackageId, @event);
                }
                return;
        }
    }
}