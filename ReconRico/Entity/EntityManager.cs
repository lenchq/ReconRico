using System.Diagnostics;
using ReconRico.Components;
using ReconRico.Extensions;

namespace ReconRico;

public static class EntityManager
{
    public static readonly Dictionary<long, Entity> Entities = new();

    private static long _nextEntityId;

    public static Entity CreateEntity()
    {
        var entity = new Entity(_nextEntityId);
        Entities.Add(_nextEntityId, entity);
        _nextEntityId += 1;
        return entity;
    }

    public static void AddEntity(Entity entity)
    {
        Entities.Add(entity.Id, entity);
    }

    public static void RemoveEntity(long entityId)
    {
        if (!Entities.TryGetValue(entityId, out var entity))
        {
            Console.WriteLine($"{nameof(RemoveEntity)}: Entity {entityId} not found");
            return;
        }

        foreach (var component in entity.Components)
        {
            component.Destroy();
        }

        entity.Destroy();
        Entities.Remove(entityId);
    }

    public static IEnumerable<Entity> GetEntitiesWithComponent<T>() where T : IComponent
    {
        return Entities.Values
            .Where(entity =>
                entity.HasComponent<T>());
    }

    public static IEnumerable<Entity> GetEntitiesWithAny(params Type[] types)
    {
        return Entities.Values
            .Where(entity => entity.HasAnyComponent(types));
    }

    public static IEnumerable<Entity> GetEntitiesWithAll(params Type[] types)
    {
        return Entities.Values
            .Where(entity => types
                .IsSubsetOf(entity.Components
                    .Select(component => component.GetType()
                    )
                )
            );
    }
}