using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico;

public static class EntityDirector
{
    private static readonly EntityBuilder Builder = new();

    public static Entity CreateBullet(Vector2 position, float rotation, Vector2 velocity)
    {
        return Builder
            .WithTransform(position, rotation, new Vector2(0.5f, 0.5f))
            .WithVelocity(velocity)
            .WithSprite(AssetsManager.Bullet)
            .WithCollider(new Vector2(6f, 24f))
            .WithColliderResponse(e =>
            {
                Console.WriteLine("{0} collision with {1}", e.SourceId, e.TargetId);
                var bullet = EntityManager.Entities[e.SourceId];
                var target = EntityManager.Entities[e.TargetId];
                if (target.HasComponent<PlayerComponent>()) return;
                var pos = bullet.GetComponent<TransformComponent>();
                var vel = bullet.GetComponent<VelocityComponent>();
                // var ricochetAngle = (float)Math.Atan2(velocity.Y, velocity.X);
                pos.Rotation -= MathHelper.PiOver2;
                vel.Velocity = Vector2.Rotate(vel.Velocity, MathHelper.PiOver2);
            })
            .WithScript((entity, gt) =>
            {
                var vel = entity.GetComponent<VelocityComponent>();
                if (vel.Velocity.LengthSquared() < 0.1f)
                    entity.Destroy();
                else
                    vel.Velocity += vel.Velocity * 0.3f;
            })
            .Build();
    }
}