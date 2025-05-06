using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using ReconRico;
using ReconRico.Components;
using ReconRico.General;

namespace ReconRico.Tests;

[TestFixture]
public class EntityDirectorTests
{
    private static readonly Vector2 TestPosition = new(100, 100);
    private static readonly Vector2 TestSize = new(50, 50);
    private static readonly Vector2 TestVelocity = new(5, 5);
    private static readonly Vector2[] TestPatrolPoints = { new(100, 100), new(200, 200) };

    [SetUp]
    public void Setup()
    {
        EntityManager.Entities.Clear();
    }

    [Test]
    public void CreateCursor_CreatesEntityWithCorrectComponents()
    {
        var cursor = EntityDirector.CreateCursor();

        Assert.That(cursor, Is.Not.Null);
        Assert.That(cursor.GetComponent<TransformComponent>(), Is.Not.Null);
        Assert.That(cursor.GetComponent<SpriteComponent>(), Is.Not.Null);
        Assert.That(cursor.GetComponent<GameCursorComponent>(), Is.Not.Null);

        var transform = cursor.GetComponent<TransformComponent>();
        Assert.That(transform.Position.X, Is.EqualTo(GameSettings.WINDOW_WIDTH / 2f));
        Assert.That(transform.Position.Y, Is.EqualTo(GameSettings.WINDOW_HEIGHT / 2f));
        Assert.That(transform.Layer, Is.EqualTo(1));
    }

    [Test]
    public void CreateBullet_CreatesEntityWithCorrectComponents()
    {
        var bullet = EntityDirector.CreateBullet(TestPosition, 45f, TestVelocity, 3);

        Assert.That(bullet, Is.Not.Null);
        Assert.That(bullet.GetComponent<TransformComponent>(), Is.Not.Null);
        Assert.That(bullet.GetComponent<VelocityComponent>(), Is.Not.Null);
        Assert.That(bullet.GetComponent<SpriteComponent>(), Is.Not.Null);
        Assert.That(bullet.GetComponent<ColliderComponent>(), Is.Not.Null);
        Assert.That(bullet.GetComponent<ProjectileComponent>(), Is.Not.Null);
        Assert.That(bullet.GetComponent<ColliderResponse>(), Is.Not.Null);
        Assert.That(bullet.GetComponent<ScriptComponent>(), Is.Not.Null);

        var transform = bullet.GetComponent<TransformComponent>();
        Assert.That(transform.Position, Is.EqualTo(TestPosition));
        Assert.That(transform.Rotation, Is.EqualTo(45f));
        Assert.That(transform.Scale, Is.EqualTo(new Vector2(0.5f, 0.5f)));

        var velocity = bullet.GetComponent<VelocityComponent>();
        Assert.That(velocity.Velocity, Is.EqualTo(TestVelocity));

        var projectile = bullet.GetComponent<ProjectileComponent>();
        Assert.That(projectile.MaxRicochetTimes, Is.EqualTo(3));
    }

    [Test]
    public void CreatePlayer_CreatesEntityWithCorrectComponents()
    {
        var player = EntityDirector.CreatePlayer(TestPosition, 45f);


        Assert.That(player, Is.Not.Null);
        Assert.That(player.GetComponent<TransformComponent>(), Is.Not.Null);
        Assert.That(player.GetComponent<VelocityComponent>(), Is.Not.Null);
        Assert.That(player.GetComponent<ColliderComponent>(), Is.Not.Null);
        Assert.That(player.GetComponent<SpriteComponent>(), Is.Not.Null);
        Assert.That(player.GetComponent<RigidbodyComponent>(), Is.Not.Null);
        Assert.That(player.GetComponent<PlayerComponent>(), Is.Not.Null);
        Assert.That(player.GetComponent<GunComponent>(), Is.Not.Null);

        var transform = player.GetComponent<TransformComponent>();
        Assert.That(transform.Position, Is.EqualTo(TestPosition));
        Assert.That(transform.Rotation, Is.EqualTo(45f));

        var collider = player.GetComponent<ColliderComponent>();
        Assert.That(collider.Collider, Is.EqualTo(new Vector2(64, 32)));

        // var gun = player.GetComponent<GunComponent>();
        // Assert.That(gun.Ammo, Is.EqualTo(10));
        // Assert.That(gun.MaxAmmo, Is.EqualTo(100));
        // Assert.That(gun.ShootDelay, Is.EqualTo(8));
    }

    [Test]
    public void CreateSolidWallObstacle_CreatesEntityWithCorrectComponents()
    {
        var wall = EntityDirector.CreateSolidWallObstacle(TestPosition, TestSize, 45f);


        Assert.That(wall, Is.Not.Null);
        Assert.That(wall.GetComponent<TransformComponent>(), Is.Not.Null);
        Assert.That(wall.GetComponent<ColliderComponent>(), Is.Not.Null);
        Assert.That(wall.GetComponent<RigidbodyComponent>(), Is.Not.Null);
        Assert.That(wall.GetComponent<ObstacleComponent>(), Is.Not.Null);

        var transform = wall.GetComponent<TransformComponent>();
        Assert.That(transform.Position, Is.EqualTo(TestPosition));
        Assert.That(transform.Rotation, Is.EqualTo(45f));

        var obstacle = wall.GetComponent<ObstacleComponent>();
        Assert.That(obstacle.BulletCollisionType, Is.EqualTo(BulletCollisionType.Absorb));
        Assert.That(obstacle.IsBreakable, Is.False);
    }

    [Test]
    public void CreatePassBreakableWallObstacle_CreatesEntityWithCorrectComponents()
    {
        var wall = EntityDirector.CreatePassBreakableWallObstacle(TestPosition, TestSize, 45f);

        Assert.That(wall, Is.Not.Null);
        var obstacle = wall.GetComponent<ObstacleComponent>();
        Assert.That(obstacle.BulletCollisionType, Is.EqualTo(BulletCollisionType.Pass));
        Assert.That(obstacle.IsBreakable, Is.True);
    }

    [Test]
    public void CreateAbsorbBreakableWallObstacle_CreatesEntityWithCorrectComponents()
    {
        var wall = EntityDirector.CreateAbsorbBreakableWallObstacle(TestPosition, TestSize, 45f);

        Assert.That(wall, Is.Not.Null);
        var obstacle = wall.GetComponent<ObstacleComponent>();
        Assert.That(obstacle.BulletCollisionType, Is.EqualTo(BulletCollisionType.Absorb));
        Assert.That(obstacle.IsBreakable, Is.True);
    }

    [Test]
    public void CreateReflectorWallObstacle_CreatesEntityWithCorrectComponents()
    {
        var wall = EntityDirector.CreateReflectorWallObstacle(TestPosition, TestSize, 45f);

        Assert.That(wall, Is.Not.Null);
        var obstacle = wall.GetComponent<ObstacleComponent>();
        Assert.That(obstacle.BulletCollisionType, Is.EqualTo(BulletCollisionType.Reflect));
        Assert.That(obstacle.IsBreakable, Is.False);
    }

    [Test]
    public void CreateReflectorBreakableWallObstacle_CreatesEntityWithCorrectComponents()
    {
        var wall = EntityDirector.CreateReflectorBreakableWallObstacle(TestPosition, TestSize, 45f);

        Assert.That(wall, Is.Not.Null);
        var obstacle = wall.GetComponent<ObstacleComponent>();
        Assert.That(obstacle.BulletCollisionType, Is.EqualTo(BulletCollisionType.Reflect));
        Assert.That(obstacle.IsBreakable, Is.True);
    }

    [Test]
    public void CreateDoor_CreatesEntityWithCorrectComponents()
    {
        var door = EntityDirector.CreateDoor(TestPosition, TestSize);

        Assert.That(door, Is.Not.Null);
        Assert.That(door.GetComponent<TransformComponent>(), Is.Not.Null);
        Assert.That(door.GetComponent<ColliderComponent>(), Is.Not.Null);
        Assert.That(door.GetComponent<DoorComponent>(), Is.Not.Null);

        var transform = door.GetComponent<TransformComponent>();
        Assert.That(transform.Position, Is.EqualTo(TestPosition));
        Assert.That(transform.Rotation, Is.EqualTo(0f));

        var collider = door.GetComponent<ColliderComponent>();
        Assert.That(collider.Collider, Is.EqualTo(TestSize));
    }

    [Test]
    public void CreateEnemy_CreatesEntityWithCorrectComponents()
    {
        var enemy = EntityDirector.CreateEnemy(TestPosition, TestPatrolPoints);

        Assert.That(enemy, Is.Not.Null);
        Assert.That(enemy.GetComponent<TransformComponent>(), Is.Not.Null);
        Assert.That(enemy.GetComponent<VelocityComponent>(), Is.Not.Null);
        Assert.That(enemy.GetComponent<ColliderComponent>(), Is.Not.Null);
        Assert.That(enemy.GetComponent<SpriteComponent>(), Is.Not.Null);
        Assert.That(enemy.GetComponent<RigidbodyComponent>(), Is.Not.Null);
        Assert.That(enemy.GetComponent<EnemyComponent>(), Is.Not.Null);

        var transform = enemy.GetComponent<TransformComponent>();
        Assert.That(transform.Position, Is.EqualTo(TestPosition));
        Assert.That(transform.Rotation, Is.EqualTo(0f));

        var collider = enemy.GetComponent<ColliderComponent>();
        Assert.That(collider.Collider, Is.EqualTo(new Vector2(64f, 32f)));

        var enemyComponent = enemy.GetComponent<EnemyComponent>();
        Assert.That(enemyComponent.PatrolPoints, Is.EqualTo(TestPatrolPoints));
    }

    [Test]
    public void CreateBaseWall_CreatesEntityWithCorrectComponents()
    {
        var wall = EntityDirector.CreateSolidWallObstacle(TestPosition, TestSize, 45f);

        Assert.That(wall, Is.Not.Null);
        Assert.That(wall.GetComponent<TransformComponent>(), Is.Not.Null);
        Assert.That(wall.GetComponent<ColliderComponent>(), Is.Not.Null);
        Assert.That(wall.GetComponent<RigidbodyComponent>(), Is.Not.Null);

        var transform = wall.GetComponent<TransformComponent>();
        Assert.That(transform.Position, Is.EqualTo(TestPosition));
        Assert.That(transform.Rotation, Is.EqualTo(45f));

        var collider = wall.GetComponent<ColliderComponent>();
        Assert.That(collider.Collider, Is.EqualTo(TestSize));
    }
}