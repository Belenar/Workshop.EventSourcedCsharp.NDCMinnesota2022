using BeerSender.API.Read_models;
using BeerSender.API.Sql_event_stream;
using Microsoft.EntityFrameworkCore;

namespace BeerSender.API.Projections.Infrastructure
{
    public class Projection_service<TProjection> : BackgroundService
        where TProjection : class, IProjection
    {
        private const int Batch_size = 100;

        private readonly IServiceProvider _services;

        public Projection_service(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var current_checkpoint = await Get_checkpoint();

            while (!stoppingToken.IsCancellationRequested)
            {
                current_checkpoint = await ProcessBatch(current_checkpoint, Batch_size);
                await Set_checkpoint(current_checkpoint);
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task<int> ProcessBatch(int from_id, int batch_size)
        {
            using var scope = _services.CreateScope();
            var projection = scope.ServiceProvider.GetService<TProjection>();

            var events = await Read_events(from_id, batch_size, projection);

            foreach (var persistedEvent in events)
            {
                projection.Project(persistedEvent.Event);
            }
            projection.Commit();

            return events.Count() == 0 ? from_id : events.Max(e => e.Id);
        }

        private async Task<IEnumerable<Persisted_event>> Read_events(int from_id, int batch_size, TProjection projection)
        {
            using var scope = _services.CreateScope();
            var event_store = scope.ServiceProvider.GetService<EventContext>();

            var event_types = projection.Source_event_types.Select(e => e.AssemblyQualifiedName).ToArray();

            var events = await event_store.Events
                .Where(e => e.Id > from_id)
                .Where(e => event_types.Contains(e.Event_type))
                .Take(Batch_size)
                .OrderBy(e => e.Id)
                .ToListAsync();

            return events;
        }

        private async Task<int> Get_checkpoint()
        {
            using var scope = _services.CreateScope();
            var read_context = scope.ServiceProvider.GetService<Read_context>();

            var checkpoint = await read_context.Projection_checkpoints
                .FindAsync(typeof(TProjection).AssemblyQualifiedName);

            return checkpoint?.Event_id ?? 0;
        }

        private async Task Set_checkpoint(int last_id)
        {
            using var scope = _services.CreateScope();
            var read_context = scope.ServiceProvider.GetService<Read_context>();

            var checkpoint = await read_context.Projection_checkpoints
                .FindAsync(typeof(TProjection).AssemblyQualifiedName);

            if (checkpoint == null)
            {
                checkpoint = new Projection_checkpoint
                {
                    Projection_name = typeof(TProjection).AssemblyQualifiedName
                };
                read_context.Projection_checkpoints.Add(checkpoint);
            }
            
            checkpoint.Event_id = last_id;
            await read_context.SaveChangesAsync();
        }
    }
}
