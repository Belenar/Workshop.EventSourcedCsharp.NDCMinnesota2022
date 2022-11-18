using BeerSender.API.Read_models;
using BeerSender.API.Sql_event_stream;
using BeerSender.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeerSender.API.Projections.Infrastructure
{
    public class Projection_service<TProjection> : BackgroundService
        where TProjection : class, IProjection
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public Projection_service(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    Console.WriteLine("Running projection service");

                    var projection = scope.ServiceProvider.GetRequiredService<Beer_package_projection>();
                    var filterEvents = projection.Source_event_types
                        .Select(e => e.AssemblyQualifiedName)
                        .ToList();

                    var readContext = scope.ServiceProvider.GetRequiredService<Read_context>();
                    var checkpoint = await readContext.Projection_checkpoints.FirstOrDefaultAsync(c =>
                        c.Projection_name == typeof(TProjection).Name);

                    var lastCheckpointId = checkpoint?.Event_id ?? 0;

                    var eventContext = scope.ServiceProvider.GetRequiredService<EventContext>();
                    var events = eventContext.Events
                        .Where(e => filterEvents.Contains(e.Event_type) && e.Id > lastCheckpointId)
                        .OrderBy(e => e.Id)
                        .ToList();

                    if (events.Any())
                    {
                        foreach (var @event in events)
                        {
                            projection.Project(@event.Event);
                        }

                        if (checkpoint == null)
                        {
                            checkpoint = new Projection_checkpoint
                            {
                                Projection_name = typeof(TProjection).Name
                            };
                            readContext.Projection_checkpoints.Add(checkpoint);
                        }

                        checkpoint.Event_id = events.MaxBy(e => e.Id).Id;

                        projection.Commit();
                    }

                    await Task.Delay(5000);
                }
            }
            // Fetch a batch
            // Run the projection for said batch
            // Commit the batch
            // Update checkpoint
            // (wait)
            // Repeat
            
        }
    }
}
