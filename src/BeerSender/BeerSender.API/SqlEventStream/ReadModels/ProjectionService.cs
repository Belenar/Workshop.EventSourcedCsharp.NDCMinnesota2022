using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.SqlEventStream.ReadModels;

public class ProjectionService<TProjection> : BackgroundService where TProjection : IProjection
{
    private readonly EventContext _eventContext;

    private TProjection _projection;
    private readonly ReadContext _readContext;

    public ProjectionService(EventContext eventContext, TProjection projection, ReadContext readContext)
    {
        _eventContext = eventContext;
        _projection = projection;
        _readContext = readContext;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Fetches a batch
        // Runs the projections for the batch
        // Commits batch
        // Updates its checkpoint
        // Waits
        // Repeats

        do
        {
            var checkpoint = await _readContext.ProjectionCheckpoint
                .Where(pc => pc.ProjectionName == _projection.GetType().FullName)
                .FirstOrDefaultAsync(cancellationToken: stoppingToken);
            
            var sourceNames = _projection.SourceEventTypes.Select(t => t.AssemblyQualifiedName);
        
            var events = await _eventContext.PersistedEvent
                .Where(e => sourceNames.Contains(e.EventType))
                .OrderBy(e => e.CreatedOn)
                .ToListAsync(cancellationToken: stoppingToken);

            foreach (var @event in events)
            {
                _projection.Project(@event);
            }

            if (checkpoint is null)
            {
                var newCheckPoint = new ProjectionCheckpoint(typeof(TProjection).AssemblyQualifiedName!, 0);
                await _readContext.ProjectionCheckpoint.AddAsync(newCheckPoint, stoppingToken);
            }
            else
            {
                checkpoint.UpdateEventId(events.Max(e => e.Id));
            }
           
            await _readContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        } while (!stoppingToken.IsCancellationRequested);
       
    }
}