using System.Diagnostics;

namespace ReconRico.Entity;

public static class EntityManager
{
    public static readonly Dictionary<long, Entity> Entities = new();

    private static long _nextEntityId;

    public static void AddEntity(Entity entity)
    {
        Entities.Add(_nextEntityId++, entity);
    }

    public static void RemoveEntity(long entityId)
    {
        if (!Entities.ContainsKey(entityId))
        {
            Debug.WriteLine($"{nameof(RemoveEntity)}: Entity {entityId} not found");
            return;
        }

        Entities.Remove(entityId);
    }
}