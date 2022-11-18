using BeerSender.API.Read_models;
using BeerSender.API.Sql_event_stream;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.Projections.Infrastructure
{
    public class Projection_service<TProjection> : BackgroundService
        where TProjection : class, IProjection
    {
        private const int BatchSize = 10;
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

                    var events = eventContext.Events
                        .Where(e => filterEvents.Contains(e.Event_type) && e.Id > checkpoint.Event_id)
                        .OrderBy(e => e.Id)
                        .ToList();

                    if (events.Any())
                    {
                        foreach (var chunk in Chunk(events, BatchSize))
                        {
                            checkpoint.Event_id = chunk.MaxBy(e => e.Id).Id;
                            await RunProjection(chunk, projection);
                        }
                    }

                    await Task.Delay(30000);
                }
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

        public static Queue<List<T>> Chunk<T>(IList<T> source, int groupSize)
        {
            return new(source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / groupSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList());
        }
    }
}
