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
                Vector2.One, 1)
            .WithSprite(AssetsManager.Cursor)
            .WithComponent(new GameCursorComponent())
            .Build();
    }

    public static Entity CreateBullet(Vector2 position, float rotation, Vector2 velocity, int maxRicochetTimes)
    {
        return Builder
            .WithTransform(position, rotation, new Vector2(0.5f, 0.5f), 0)
            .WithVelocity(velocity)
            .WithSprite(AssetsManager.Bullet)
            .WithCollider(new Vector2(12, 12))
            .WithComponent(new ProjectileComponent
            {
                MaxRicochetTimes = maxRicochetTimes,
            })
            .WithColliderResponse(e =>
            {
                var bullet = EntityManager.Entities[e.SourceId];
                var target = EntityManager.Entities[e.TargetId];

                if (target.IsDestroyed) return;

                if (target.HasComponent<PlayerComponent>())
                {
                    var playerGun = target.GetComponent<GunComponent>();
                    playerGun.Ammo += 1;
                    bullet.Destroy();
                    SfxManager.PlayPickup();
                    return;
                }

                var bulletPos = bullet.GetComponent<TransformComponent>();
                var bulletVel = bullet.GetComponent<VelocityComponent>();
                var bulletCollider = bullet.GetComponent<ColliderComponent>();
                var bulletProj = bullet.GetComponent<ProjectileComponent>();

                var targetPos = target.GetComponent<TransformComponent>();
                var targetCollider = target.GetComponent<ColliderComponent>();

                if (target.TryGetComponent<EnemyComponent>(out var enemy))
                    target.Destroy();

                else if (target.TryGetComponent<ObstacleComponent>(out var obstacle))
                {
                    switch (obstacle.BulletCollisionType)
                    {
                        case BulletCollisionType.Reflect:
                            if (bulletProj.RicochetTimes++ > maxRicochetTimes)
                            {
                                bullet.Destroy();
                                break;
                            }

                            var reflectedVelocity = GunSystem.GetWallReflectedVelocity(velocity, bulletCollider,
                                bulletPos,
                                targetCollider, targetPos, bulletVel);

                            bulletVel.Velocity = reflectedVelocity;
                            bulletPos.Rotation =
                                (float)Math.Atan2(reflectedVelocity.Y, reflectedVelocity.X) + MathHelper.PiOver2;
                            SfxManager.PlayRicochet();

                            break;
                        case BulletCollisionType.Absorb:
                            bullet.Destroy();
                            break;
                    }

                    if (obstacle.IsBreakable)
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
            .Build();
    }

    public static Entity CreatePlayer(Vector2 position, float rotation)
    {
        return Builder
            .WithTransform(position, rotation, Vector2.One)
            .WithVelocity()
            .WithCollider(new Vector2(64, 32))
            .WithSprite(AssetsManager.Player)
            .WithRigidbody()
            .WithPlayer()
            .WithComponent(new GunComponent(10, 100, 8))
            .Build();
    }

    public static Entity CreateSolidWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            // .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Absorb, false)
            .Build();
    }

    public static Entity CreatePassBreakableWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            // .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Pass, true)
            .Build();
    }

    public static Entity CreateAbsorbBreakableWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            // .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Absorb, true)
            .Build();
    }

    public static Entity CreateReflectorWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            // .WithSprite(AssetsManager.Ball, true)
            .WithObstacle(BulletCollisionType.Reflect, false)
            .Build();
    }

    public static Entity CreateReflectorBreakableWallObstacle(Vector2 position, Vector2 size, float rotation)
    {
        return CreateBaseWall(position, size, rotation)
            // .WithSprite(AssetsManager.Ball)
            .WithObstacle(BulletCollisionType.Reflect, true)
            .Build();
    }

    public static Entity CreateDoor(Vector2 position, Vector2 size)
    {
        return Builder
            .WithTransform(position, 0, Vector2.One)
            .WithCollider(size)
            .WithComponent(new DoorComponent())
            .Build();
    }

    public static Entity CreateEnemy(Vector2 position, Vector2[] patrolPoints)
    {
        return Builder
            .WithTransform(position, 0, Vector2.One)
            .WithVelocity()
            .WithCollider(new Vector2(64f, 32f))
            .WithSprite(AssetsManager.Enemy)
            .WithRigidbody()
            .WithComponent(new EnemyComponent(patrolPoints))
            .OnDestroy(_ => { SfxManager.PlayHit(); })
            .Build();
    }

    private static EntityBuilder CreateBaseWall(Vector2 position, Vector2 size, float rotation)
    {
        return Builder
            .WithTransform(position, rotation, Vector2.One)
            .WithCollider(size)
            .WithRigidbody()
            .OnDestroy(entity =>
            {
                if (entity.TryGetComponent<ObstacleComponent>(out var obstacle) && obstacle.IsBreakable)
                {
                    SfxManager.PlayExplosion();
                }
            });
    }
}