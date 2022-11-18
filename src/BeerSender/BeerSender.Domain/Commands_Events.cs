namespace BeerSender.Domain;

public interface ICommand
{
}

public interface ICommand<TAggregate> : ICommand
{
    Guid Aggregate_id { get; }
}

public interface IEvent
{
}

public abstract record BaseCommand<TAggregate>(Guid Aggregate_id) : ICommand<TAggregate>;
