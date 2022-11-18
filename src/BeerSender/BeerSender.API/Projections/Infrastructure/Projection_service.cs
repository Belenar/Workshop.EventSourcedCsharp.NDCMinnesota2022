using BeerSender.API.Read_models;
using BeerSender.API.Sql_event_stream;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.Projections.Infrastructure
{
    public class Projection_service<TProjection> : BackgroundService
        where TProjection : class, IProjection
    {
        private const int BatchSize = 2;
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
                    var readContext = scope.ServiceProvider.GetRequiredService<Read_context>();
                    var eventContext = scope.ServiceProvider.GetRequiredService<EventContext>();

                    var filterEvents = projection.Source_event_types
                        .Select(e => e.AssemblyQualifiedName)
                        .ToList();

                    var checkpoint = await GetProjectionCheckpoint(readContext);

                    await ProcessEvents(eventContext, filterEvents, checkpoint, projection);

                    await Task.Delay(30000);
                }
            }
        }

        private static async Task ProcessEvents(EventContext eventContext, List<string?> filterEvents, Projection_checkpoint checkpoint,
            Beer_package_projection projection)
        {
            while (true)
            {
                var events = eventContext.Events
                    .Where(e => filterEvents.Contains(e.Event_type) && e.Id > checkpoint.Event_id)
                    .OrderBy(e => e.Id)
                    .Take(BatchSize)
                    .ToList();

                if (!events.Any())
                    return;

                checkpoint.Event_id = events.Max(e => e.Id);
                await RunProjection(events, projection);
            }
        }

        private static async Task<Projection_checkpoint> GetProjectionCheckpoint(Read_context readContext)
        {
            var checkpoint = await readContext.Projection_checkpoints
                .FirstOrDefaultAsync(c => c.Projection_name == typeof(TProjection).Name);
            if (checkpoint == null)
            {
                checkpoint = new Projection_checkpoint
                {
                    Projection_name = typeof(TProjection).Name
                };
                readContext.Projection_checkpoints.Add(checkpoint);
            }
            return checkpoint;
        }

        private static async Task RunProjection(List<Persisted_event> events, Beer_package_projection projection)
        {
            foreach (var @event in events)
            {
                projection.Project(@event.Event);
            }

            await projection.Commit();
        }
    }
}
