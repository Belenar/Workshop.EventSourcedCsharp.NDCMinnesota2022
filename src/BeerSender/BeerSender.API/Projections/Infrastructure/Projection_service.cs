using BeerSender.API.Read_models;
using BeerSender.API.Sql_event_stream;

namespace BeerSender.API.Projections.Infrastructure
{
    public class Projection_service<TProjection> : BackgroundService
        where TProjection : class, IProjection
    {
        private readonly IServiceProvider _services;

        public Projection_service(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Fetch a batch
            // Run the projection for said batch
            // Commit the batch
            // Update checkpoint
            // (wait)
            // Repeat

            while (!stoppingToken.IsCancellationRequested)
            {
                var projection = GetProjection();

                var isNewCheckpoint = false;

                var checkpoint = GetCheckpoint(projection.ProjectionName);

                if (checkpoint == null)
                {
                    checkpoint = new Projection_checkpoint
                    {
                        Event_id = int.MinValue,
                        Projection_name = projection.ProjectionName
                    };

                    isNewCheckpoint = true;
                }

                var events = GetEventsSince(checkpoint.Event_id, 100);

                foreach (var @event in events)
                {
                    if (!projection.Source_event_types.Contains(@event.GetType()))
                        continue;

                    projection.Project(@event.Event);

                    checkpoint.Event_id = @event.Id;
                }

                projection.Commit();

                PersistCheckpoint(checkpoint, isNewCheckpoint);

                await Task.Delay(5000, stoppingToken);
            }
        }

        private IProjection GetProjection()
        {
            using var scope = _services.CreateScope();

            return scope.ServiceProvider.GetRequiredService<IProjection>();
        }

        private Projection_checkpoint? GetCheckpoint(string projectionName)
        {
            using var scope = _services.CreateScope();

            var readContext = scope.ServiceProvider.GetRequiredService<Read_context>();

            return readContext.Projection_checkpoints
                        .SingleOrDefault(x =>
                            x.Projection_name == projectionName);
        }

        private IReadOnlyCollection<Persisted_event> GetEventsSince(int checkpointEventId, int batchSize)
        {
            using var scope = _services.CreateScope();

            var eventContext = scope.ServiceProvider.GetRequiredService<EventContext>();

            return eventContext.Events
                        .Where(e => e.Id > checkpointEventId)
                        .OrderBy(e => e.Id)
                        .Take(batchSize)
                        .ToList();
        }

        private void PersistCheckpoint(Projection_checkpoint checkpoint, bool isNewCheckpoint)
        {
            using var scope = _services.CreateScope();

            var readContext = scope.ServiceProvider.GetRequiredService<Read_context>();

            if (isNewCheckpoint)
            {
                readContext.Projection_checkpoints.Add(checkpoint);
            }

            readContext.SaveChanges();
        }
    }
}
