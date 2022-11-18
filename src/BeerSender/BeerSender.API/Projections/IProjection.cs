using BeerSender.Domain;

namespace BeerSender.API.Projections;

public interface IProjection
{
    Type[] Source_event_types { get; }
    void Project(IEvent @event);
    void Commit();
}