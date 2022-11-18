namespace BeerSender.API.Projections.Infrastructure
{
    public class Projection_service<TProjection> : BackgroundService
        where TProjection : class, IProjection
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Fetch a batch
            // Run the projection for said batch
            // Commit the batch
            // Update checkpoint
            // (wait)
            // Repeat
        }
    }
}
