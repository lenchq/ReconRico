using Microsoft.Xna.Framework;
using ReconRico.Components;
using System.Linq;

namespace ReconRico.Systems;

public class ColliderSystem
{
    private const int Substeps = 5; // Increase for better precision
    const float StepSize = 1f / Substeps;

    public void Update(GameTime gameTime)
    {
        var colliders = GetColliders();
        CheckCollisions(colliders, gameTime);
    }

    private (Entity Entity, RotatedRectangle Rect, Vector2 Velocity)[] GetColliders() =>
        EntityManager.GetEntitiesWithAll(typeof(ColliderComponent), typeof(TransformComponent))
            .Select(e =>
            {
                var transform = e.GetComponent<TransformComponent>();
                var collider = e.GetComponent<ColliderComponent>();
                var velocity = e.TryGetComponent<VelocityComponent>(out var v) ? v.Velocity : Vector2.Zero;

                var rect = new RotatedRectangle(
                    transform.Position + collider.Offset,
                    collider.Collider,
                    transform.Rotation
                );

                return (e, rect, velocity);
            })
            .ToArray();

    private void CheckCollisions((Entity Entity, RotatedRectangle Rect, Vector2 Velocity)[] colliders,
        GameTime gameTime)
    {
        for (int i = 0; i < colliders.Length; i++)
        for (int j = i + 1; j < colliders.Length; j++)
        {
            if (SweptIntersects(colliders[i], colliders[j], gameTime))
            {
                NotifyCollision(colliders[i], colliders[j]);
                NotifyCollision(colliders[j], colliders[i]);
            }
        }
    }

    private bool RaycastIntersectsBullet((Entity Entity, RotatedRectangle Rect, Vector2 Velocity) bullet,
        (Entity Entity, RotatedRectangle Rect, Vector2 Velocity) other,
        float deltaTime)
    {
        var start = bullet.Rect.Position;
        var end = start + bullet.Velocity * deltaTime;

        var corners = other.Rect.GetCorners();
        for (int i = 0; i < 4; i++)
        {
            var p1 = corners[i];
            var p2 = corners[(i + 1) % 4];

            if (RotatedRectangle.LineSegmentsIntersect(start, end, p1, p2, out _))
                return true;
        }

        return false;
    }

    private bool SweptIntersects((Entity Entity, RotatedRectangle Rect, Vector2 Velocity) a,
        (Entity Entity, RotatedRectangle Rect, Vector2 Velocity) b, GameTime gameTime)
    {
        for (var step = 0; step <= Substeps; step++)
        {
            var posA = a.Rect.Position + a.Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds * StepSize * step;
            var posB = b.Rect.Position + b.Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds * StepSize * step;

            var rectA = new RotatedRectangle(posA, a.Rect.Size, a.Rect.Rotation);
            var rectB = new RotatedRectangle(posB, b.Rect.Size, b.Rect.Rotation);

            if (rectA.Intersects(rectB))
                return true;
        }

        return false;
    }
    // private bool SweptIntersects(
    //     (Entity Entity, RotatedRectangle Rect, Vector2 Velocity) a,
    //     (Entity Entity, RotatedRectangle Rect, Vector2 Velocity) b)
    // {
    //     float deltaTime = 1f / 60f; // assume fixed timestep (or get from GameTime)
    //
    //     // If either is very fast (e.g., bullets), use raycasting
    //     if (a.Velocity.LengthSquared() > 5000f || b.Velocity.LengthSquared() > 5000f)
    //     {
    //         return RaycastIntersectsBullet(a, b, deltaTime) || RaycastIntersectsBullet(b, a, deltaTime);
    //     }
    //
    //     // fallback to substeps
    //     float stepSize = 1f / Substeps;
    //     for (int step = 0; step <= Substeps; step++)
    //     {
    //         var posA = a.Rect.Position + a.Velocity * stepSize * step;
    //         var posB = b.Rect.Position + b.Velocity * stepSize * step;
    //
    //         var rectA = new RotatedRectangle(posA, a.Rect.Size, a.Rect.Rotation);
    //         var rectB = new RotatedRectangle(posB, b.Rect.Size, b.Rect.Rotation);
    //
    //         if (rectA.Intersects(rectB))
    //             return true;
    //     }
    //
    //     return false;
    // }


    private void NotifyCollision(
        (Entity Entity, RotatedRectangle Rect, Vector2 Velocity) a,
        (Entity Entity, RotatedRectangle Rect, Vector2 Velocity) b)
    {
        if (a.Entity.TryGetComponent<ColliderResponse>(out var response))
        {
            response.OnCollision(new CollisionEvent(
                a.Entity.Id,
                b.Entity.Id,
                RotatedRectangle.GetIntersectPoint(a.Rect, b.Rect)
            ));
        }
    }
}