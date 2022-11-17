using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.SqlEventStream;

public class EventContext : DbContext
{
    public EventContext(DbContextOptions<EventContext> options) : base(options)
    {
    }
    
    public DbSet<PersistedEvent> PersistedEvent { get; private set; } = null!;
}

public class PersistedEvent
{
    public int Id { get; private set; }

    public Guid AggregateId { get; private set; }

    [MaxLength(255)]
    public string EventType { get; private set; }

    public string Payload { get; private set; }

    public DateTime CreatedOn { get; private set; }

    object _event;

    public PersistedEvent(Guid aggregateId, object @event)
    {
        AggregateId = aggregateId;
        Event = @event;
        CreatedOn = DateTime.UtcNow;
    }

    private PersistedEvent()
    {
        
    }
    
    [NotMapped]
    public object Event
    {
        get
        {
            if (_event == null)
            {
                var type = Type.GetType(EventType);
                _event = JsonSerializer.Deserialize(Payload, type);
            }
            return _event;
        }
        set
        {
            if (!(_event?.Equals(value) ?? false))
            {
                _event = value;

                EventType = _event.GetType().AssemblyQualifiedName;
                Payload = JsonSerializer.Serialize(_event, _event.GetType());
            }
        }
    }
}
