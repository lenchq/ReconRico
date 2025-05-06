using Microsoft.Xna.Framework;
using ReconRico.Components;
using System.Linq;

namespace ReconRico.Systems;

public class ColliderSystem
{
    private const int Substeps = 4; // Increased for better precision
    const float StepSize = 1f / Substeps;
    private const float MinSeparation = 0.1f; // Minimum separation between objects

    public void Update(GameTime gameTime)
    {
        var colliders = GetColliders();
        CheckCollisions(colliders, gameTime);
    }

    private (Entity Entity, Rectangle Rect, Vector2 Velocity)[] GetColliders() =>
        EntityManager.GetEntitiesWithAll(typeof(ColliderComponent), typeof(TransformComponent))
            .Select(e =>
            {
                var transform = e.GetComponent<TransformComponent>();
                var collider = e.GetComponent<ColliderComponent>();
                var velocity = e.TryGetComponent<VelocityComponent>(out var v) ? v.Velocity : Vector2.Zero;

                var rect = new Rectangle(
                    (int)(transform.Position.X + collider.Offset.X - collider.Collider.X / 2),
                    (int)(transform.Position.Y + collider.Offset.Y - collider.Collider.Y / 2),
                    (int)collider.Collider.X,
                    (int)collider.Collider.Y
                );

                return (e, rect, velocity);
            })
            .ToArray();

    private void CheckCollisions((Entity Entity, Rectangle Rect, Vector2 Velocity)[] colliders,
        GameTime gameTime)
    {
        for (int i = 0; i < colliders.Length; i++)
        for (int j = i + 1; j < colliders.Length; j++)
        {
            if (SweptIntersects(colliders[i], colliders[j], gameTime))
            {
                // Check if objects are already intersecting and push them apart
                if (colliders[i].Rect.Intersects(colliders[j].Rect) &&
                    colliders[i].Entity.HasComponent<RigidbodyComponent>() &&
                    colliders[j].Entity.HasComponent<RigidbodyComponent>() &&
                    !colliders[i].Entity.HasComponent<ObstacleComponent>() &&
                    !colliders[j].Entity.HasComponent<ObstacleComponent>())
                {
                    PushApart(colliders[i], colliders[j]);
                }

                NotifyCollision(colliders[i], colliders[j]);
                NotifyCollision(colliders[j], colliders[i]);
            }
        }
    }

    private void PushApart(
        (Entity Entity, Rectangle Rect, Vector2 Velocity) a,
        (Entity Entity, Rectangle Rect, Vector2 Velocity) b)
    {
        var aTransform = a.Entity.GetComponent<TransformComponent>();
        var bTransform = b.Entity.GetComponent<TransformComponent>();

        // Calculate the overlap
        var intersection = Rectangle.Intersect(a.Rect, b.Rect);

        // Calculate the centers
        var aCenter = new Vector2(a.Rect.X + a.Rect.Width / 2f, a.Rect.Y + a.Rect.Height / 2f);
        var bCenter = new Vector2(b.Rect.X + b.Rect.Width / 2f, b.Rect.Y + b.Rect.Height / 2f);

        // Calculate the direction from a to b
        var direction = bCenter - aCenter;
        if (direction == Vector2.Zero) direction = Vector2.UnitX; // Fallback if centers are the same
        direction.Normalize();

        // Calculate the push distance
        float pushDistance = Math.Max(intersection.Width, intersection.Height) / 2f + MinSeparation;

        // Only move the object that has velocity (or both if neither has velocity)
        bool aHasVelocity = a.Entity.HasComponent<VelocityComponent>();
        bool bHasVelocity = b.Entity.HasComponent<VelocityComponent>();

        if (aHasVelocity && !bHasVelocity)
        {
            aTransform.Position -= direction * pushDistance;
        }
        else if (!aHasVelocity && bHasVelocity)
        {
            bTransform.Position += direction * pushDistance;
        }
        else
        {
            // If both or neither have velocity, move both apart
            aTransform.Position -= direction * (pushDistance / 2f);
            bTransform.Position += direction * (pushDistance / 2f);
        }
    }

    private bool SweptIntersects(
        (Entity Entity, Rectangle Rect, Vector2 Velocity) a,
        (Entity Entity, Rectangle Rect, Vector2 Velocity) b, GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        float stepSize = deltaTime / Substeps;

        for (int step = 0; step <= Substeps; step++)
        {
            var posA = new Vector2(a.Rect.X, a.Rect.Y) + a.Velocity * stepSize * step;
            var posB = new Vector2(b.Rect.X, b.Rect.Y) + b.Velocity * stepSize * step;

            var rectA = new Rectangle(
                (int)posA.X,
                (int)posA.Y,
                a.Rect.Width,
                a.Rect.Height
            );

            var rectB = new Rectangle(
                (int)posB.X,
                (int)posB.Y,
                b.Rect.Width,
                b.Rect.Height
            );

            if (rectA.Intersects(rectB))
                return true;
        }

        return false;
    }

    private void NotifyCollision(
        (Entity Entity, Rectangle Rect, Vector2 Velocity) a,
        (Entity Entity, Rectangle Rect, Vector2 Velocity) b)
    {
        if (!a.Entity.TryGetComponent<ColliderResponse>(out var response)) return;

        // Calculate intersection point as the center of the intersection rectangle
        var intersection = Rectangle.Intersect(a.Rect, b.Rect);
        var intersectPoint = new Vector2(
            intersection.X + intersection.Width / 2f,
            intersection.Y + intersection.Height / 2f
        );

        response.OnCollision(new CollisionEvent(
            a.Entity.Id,
            b.Entity.Id,
            intersectPoint
        ));
    }
}