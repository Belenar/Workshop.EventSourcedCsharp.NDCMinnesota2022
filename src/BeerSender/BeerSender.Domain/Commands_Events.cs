namespace BeerSender.Domain;

public interface ICommand
{
    Guid Aggregate_id { get; }
}

public interface ICommand<TAggregate> : ICommand
{
}

public interface IEvent
{
}

public abstract record BaseCommand<TAggregate>(Guid Aggregate_id) : ICommand<TAggregate>;
