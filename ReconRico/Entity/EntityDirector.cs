using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.Components;
using ReconRico.General;
using ReconRico.Systems;

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
            .WithCollider(new Vector2(6f, 6f))
            .WithColliderResponse(e =>
            {
                var bullet = EntityManager.Entities[e.SourceId];
                var target = EntityManager.Entities[e.TargetId];

                // Skip collision with player
                if (target.HasComponent<PlayerComponent>()) return;

                var bulletPos = bullet.GetComponent<TransformComponent>();
                var bulletVel = bullet.GetComponent<VelocityComponent>();
                var bulletCollider = bullet.GetComponent<ColliderComponent>();

                var targetPos = target.GetComponent<TransformComponent>();
                var targetCollider = target.GetComponent<ColliderComponent>();

                if (!target.TryGetComponent<ObstacleComponent>(out var obstacle)) return;

                if (obstacle.BulletCollisionType == BulletCollisionType.Reflect)
                {
                    var reflectedVelocity = GunSystem.GetWallReflectedVelocity(velocity, bulletCollider, bulletPos,
                        targetCollider, targetPos, bulletVel);

                    bulletVel.Velocity = reflectedVelocity;
                    bulletPos.Rotation =
                        (float)Math.Atan2(reflectedVelocity.Y, reflectedVelocity.X) + MathHelper.PiOver2;
                }
                else
                {
                    bullet.Destroy();
                }


                // Check if target is breakable
                if (obstacle.IsBreakable)
                {
                    target.Destroy();
                }
            })
            .WithScript((entity, gt) =>
            {
                var pos = entity.GetComponent<TransformComponent>();
                if (Math.Abs(pos.Position.X) > 2000f || Math.Abs(pos.Position.Y) > 2000f)
                {
                    entity.Destroy();
                }
            })
            .WithComponent(new ProjectileComponent())
            .Build();
    }

    public static Entity CreatePlayer(Vector2 position, float rotation)
    {
        return Builder
            .WithTransform(position, rotation, Vector2.One)
            .WithVelocity()
            .WithCollider(new Vector2(32f, 16f))
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