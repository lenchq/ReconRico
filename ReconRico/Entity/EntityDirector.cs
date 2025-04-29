using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.Components;
using ReconRico.General;

namespace ReconRico;

public static class EntityDirector
{
    private static readonly EntityBuilder Builder = new();

    public static Entity CreateCursor()
    {
        return Builder
            .WithTransform(new Vector2(GameSettings.WINDOW_WIDTH / 2f, GameSettings.WINDOW_HEIGHT / 2f), 0,
                Vector2.One)
            .WithSprite(AssetsManager.Cursor)
            .WithComponent(new GameCursorComponent())
            .Build();
    }

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

    public static Entity CreatePlayer(Vector2 position, float rotation)
    {
        return Builder
            .WithTransform(position, rotation, Vector2.One)
            .WithVelocity()
            .WithCollider(new Vector2(2f, 2f))
            .WithSprite(AssetsManager.Ball)
            .WithRigidbody()
            .WithPlayer()
            .WithComponent(new GunComponent(20, 100, 5)
            {
                Ammo = 9099
            })
            .Build();
    }

    public static Entity CreateSolidWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Absorb, false)
            .Build();
    }

    public static Entity CreateBreakableWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Absorb, true)
            .Build();
    }

    public static Entity CreateReflectorWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Reflect, false)
            .Build();
    }

    public static Entity CreateReflectorBreakableWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Reflect, true)
            .Build();
    }

    private static EntityBuilder CreateBaseWall(Vector2 position, Vector2 size, float rotation)
    {
        return Builder
            .WithTransform(position, rotation, Vector2.One)
            .WithCollider(size)
            .WithRigidbody();
    }
}