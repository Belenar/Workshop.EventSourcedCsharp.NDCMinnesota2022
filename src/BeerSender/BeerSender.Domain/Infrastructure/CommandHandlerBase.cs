namespace BeerSender.Domain.Infrastructure;

internal abstract class CommandHandlerBase<TCommand, TAggregate>
    where TCommand : ICommand<TAggregate>
{
    public abstract IEnumerable<IEvent> Handle(TAggregate aggregate, TCommand command);
}