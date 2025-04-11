using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class ColliderSystem
{
    public void Update(GameTime gameTime)
    {
        var colliders = GetColliders();
        CheckCollisions(colliders);
    }

    private (Entity Entity, RotatedRectangle)[] GetColliders() =>
        EntityManager.GetEntitiesWithAll(typeof(ColliderComponent), typeof(TransformComponent))
            .Select(e =>
            {
                var transform = e.GetComponent<TransformComponent>();
                var collider = e.GetComponent<ColliderComponent>();
                var rect = new RotatedRectangle(
                    transform.Position + collider.Offset,
                    collider.Collider,
                    transform.Rotation
                );

                return (
                    Entity: e,
                    Rectangle: rect
                );
            })
            .ToArray();

    private void CheckCollisions((Entity Entity, RotatedRectangle Rectangle)[] colliders)
    {
        for (var i = 0; i < colliders.Length; i++)
        for (var j = i + 1; j < colliders.Length; j++)
        {
            if (!colliders[i].Rectangle.Intersects(colliders[j].Rectangle)) continue;

            TryNotifyCollision(colliders[i], colliders[j]);
            TryNotifyCollision(colliders[j], colliders[i]);
        }
    }

    private void TryNotifyCollision((Entity Entity, RotatedRectangle Rectangle) entity,
        (Entity Entity, RotatedRectangle Rectangle) other)
    {
        if (entity.Entity.TryGetComponent<ColliderResponse>(out var response))
        {
            response.OnCollision(new CollisionEvent(
                entity.Entity.Id,
                other.Entity.Id,
                RotatedRectangle.GetIntersectPoint(entity.Rectangle, other.Rectangle)
            ));
        }
    }
}