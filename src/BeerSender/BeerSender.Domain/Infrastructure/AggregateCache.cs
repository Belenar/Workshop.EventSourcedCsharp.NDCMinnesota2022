namespace BeerSender.Domain.Infrastructure
{
    public interface IAggregateCache
    {
        object Get(Type aggregate_type, Guid aggregate_id);
    }

    public class AggregateCache : IAggregateCache
    {
        private readonly Dictionary<Guid, object> _cache;

        public AggregateCache()
        {
            _cache = new Dictionary<Guid, object>();
        }

        public object Get(Type aggregate_type, Guid aggregate_id)
        {
            if (_cache.TryGetValue(aggregate_id, out var aggregate))
                return aggregate;

            aggregate = Activator.CreateInstance(aggregate_type);
            _cache.Add(aggregate_id, aggregate);
            return aggregate;
        }
    }
}
