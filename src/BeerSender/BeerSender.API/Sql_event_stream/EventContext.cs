using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.Sql_event_stream;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> options): base(options)
    { }

    public DbSet<Persisted_event> Events { get; set; }
}

public class Persisted_event
{
    public int Id { get; set; }
    public Guid Aggregate_id { get; set; }
    [MaxLength(255)]
    public string Event_type { get; set; }

    public string Payload { get; set; }
    public DateTime Timestamp { get; set; }

    object _event;
    [NotMapped]
    public object Event
    {
        get
        {
            if (_event == null)
            {
                var type = Type.GetType(Event_type);
                _event = JsonSerializer.Deserialize(Payload, type);
            }
            return _event;
        }
        set
        {
            if (!(_event?.Equals(value) ?? false))
            {
                _event = value;

                Event_type = _event.GetType().AssemblyQualifiedName;
                Payload = JsonSerializer.Serialize(_event, _event.GetType());
            }
        }
    }
}

