namespace BeerSender.Domain.Infrastructure
{
    public interface IAggregateCache
    {
        TAggregate Get<TAggregate>(Guid aggregate_id) where TAggregate : class, new();
    }

    public class AggregateCache : IAggregateCache
    {
        private readonly Dictionary<Guid, object> _cache;

        public AggregateCache()
        {
            _cache = new Dictionary<Guid, object>();
        }

        public TAggregate Get<TAggregate>(Guid aggregate_id)
            where TAggregate : class, new()
        {
            if (_cache.TryGetValue(aggregate_id, out var aggregate))
                return (TAggregate)aggregate;

            var tmp = new TAggregate();
            _cache.Add(aggregate_id, tmp);
            return tmp;
        }
    }
}
