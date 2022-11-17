namespace BeerSender.API.Sql_event_stream
{
    public class Sql_event_stream
    {
        private readonly EventContext _context;

        public Sql_event_stream(EventContext context)
        {
            _context = context;
        }

        public IEnumerable<object> Get_events(Guid aggregate_id)
        {
            return _context.Events
                .Where(e => e.Aggregate_id == aggregate_id)
                .OrderBy(e => e.Timestamp)
                .ToList();
        }

        public void Publish_event(Guid aggregate_id, object @event)
        {
            var new_event = new Persisted_event
            {
                Aggregate_id = aggregate_id,
                Event = @event,
                Timestamp = DateTime.UtcNow
            };
            _context.Events.Add(new_event);
            _context.SaveChanges();
        }
    }
}
