using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;
using ReconRico;
using ReconRico.Components;

namespace ReconRico.Tests;

[TestFixture]
public class EntityBuilderTests
{
    private EntityBuilder _builder;
    private static readonly Vector2 TestPosition = new(100, 100);
    private static readonly Vector2 TestScale = new(2, 2);
    private static readonly Vector2 TestCollider = new(50, 50);

    [SetUp]
    public void Setup()
    {
        _builder = new EntityBuilder();
    }

    [Test]
    public void WithTransform_SetsCorrectTransformComponent()
    {
        var entity = _builder
            .WithTransform(TestPosition, 45f, TestScale, 1)
            .Build();

        var transform = entity.GetComponent<TransformComponent>();

        Assert.That(transform, Is.Not.Null);
        Assert.That(transform.Position, Is.EqualTo(TestPosition));
        Assert.That(transform.Rotation, Is.EqualTo(45f));
        Assert.That(transform.Scale, Is.EqualTo(TestScale));
        Assert.That(transform.Layer, Is.EqualTo(1));
    }

    // [Test]
    // public void WithSprite_SetsCorrectSpriteComponent()
    // {
    //     
    //     var texture = new Texture2D(, 1, 1);
    //     var color = Color.Red;
    //
    //     
    //     var entity = _builder
    //         .WithSprite(texture, true, SpriteEffects.FlipHorizontally, color)
    //         .Build();
    //
    //     
    //     var sprite = entity.GetComponent<SpriteComponent>();
    //     Assert.That(sprite, Is.Not.Null);
    //     Assert.That(sprite.Texture, Is.EqualTo(texture));
    //     Assert.That(sprite.Tiled, Is.True);
    //     Assert.That(sprite.SpriteEffects, Is.EqualTo(SpriteEffects.FlipHorizontally));
    //     Assert.That(sprite.ColorMask, Is.EqualTo(color));
    // }

    [Test]
    public void WithVelocity_SetsCorrectVelocityComponent()
    {
        var velocity = new Vector2(5, 5);

        var entity = _builder
            .WithVelocity(velocity)
            .Build();

        var velocityComponent = entity.GetComponent<VelocityComponent>();
        Assert.That(velocityComponent, Is.Not.Null);
        Assert.That(velocityComponent.Velocity, Is.EqualTo(velocity));
    }

    [Test]
    public void WithCollider_SetsCorrectColliderComponent()
    {
        var entity = _builder
            .WithCollider(TestCollider)
            .Build();

        var collider = entity.GetComponent<ColliderComponent>();
        Assert.That(collider, Is.Not.Null);
        Assert.That(collider.Collider, Is.EqualTo(TestCollider));
        Assert.That(collider.Shape, Is.EqualTo(ColliderShape.Rectangle));
    }

    [Test]
    public void WithColliderResponse_SetsCorrectColliderResponseComponent()
    {
        var collisionHandled = false;
        void OnCollision(CollisionEvent e) => collisionHandled = true;

        var entity = _builder
            .WithColliderResponse(OnCollision)
            .Build();

        var response = entity.GetComponent<ColliderResponse>();
        Assert.That(response, Is.Not.Null);
        Assert.That(response.OnCollision, Is.Not.Null);
    }

    [Test]
    public void WithScript_SetsCorrectScriptComponent()
    {
        var updateCalled = false;
        void OnUpdate(Entity e, GameTime gt) => updateCalled = true;


        var entity = _builder
            .WithScript(OnUpdate)
            .Build();

        var script = entity.GetComponent<ScriptComponent>();
        Assert.That(script, Is.Not.Null);
        Assert.That(script.OnUpdate, Is.Not.Null);
    }

    [Test]
    public void WithRigidbody_SetsCorrectRigidbodyComponent()
    {
        var entity = _builder
            .WithRigidbody()
            .Build();

        var rigidbody = entity.GetComponent<RigidbodyComponent>();
        Assert.That(rigidbody, Is.Not.Null);
    }

    [Test]
    public void WithObstacle_SetsCorrectObstacleComponent()
    {
        var entity = _builder
            .WithObstacle(BulletCollisionType.Reflect, true)
            .Build();

        var obstacle = entity.GetComponent<ObstacleComponent>();
        Assert.That(obstacle, Is.Not.Null);
        Assert.That(obstacle.BulletCollisionType, Is.EqualTo(BulletCollisionType.Reflect));
        Assert.That(obstacle.IsBreakable, Is.True);
    }

    [Test]
    public void WithPlayer_SetsCorrectPlayerComponent()
    {
        var entity = _builder
            .WithPlayer()
            .Build();

        var player = entity.GetComponent<PlayerComponent>();
        Assert.That(player, Is.Not.Null);
    }

    [Test]
    public void WithComponent_AddsCustomComponent()
    {
        var customComponent = new TestComponent();

        var entity = _builder
            .WithComponent(customComponent)
            .Build();

        var component = entity.GetComponent<TestComponent>();
        Assert.That(component, Is.Not.Null);
        Assert.That(component, Is.EqualTo(customComponent));
    }

    [Test]
    public void OnDestroy_SetsDestroyCallback()
    {
        var destroyCalled = false;
        void OnDestroy(Entity e) => destroyCalled = true;

        var entity = _builder
            .OnDestroy(OnDestroy)
            .Build();

        entity.Destroy();

        Assert.That(destroyCalled, Is.True);
    }

    [Test]
    public void Build_ReturnsNewEntity()
    {
        var entity1 = _builder.Build();
        var entity2 = _builder.Build();


        Assert.That(entity1, Is.Not.Null);
        Assert.That(entity2, Is.Not.Null);
        Assert.That(entity1, Is.Not.EqualTo(entity2));
    }

    [Test]
    public void Create_AddsEntityToManager()
    {
        var entity = _builder
            .WithTransform()
            .Create();

        Assert.That(EntityManager.GetEntitiesWithComponent<TransformComponent>(), Contains.Item(entity));
    }

    private class TestComponent : IComponent
    {
    }
}