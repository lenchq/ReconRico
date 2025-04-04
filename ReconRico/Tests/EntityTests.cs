using NUnit.Framework;
using ReconRico.Component;
using ReconRico.Entity;

namespace ReconRico.Tests;

public class TestComponent : IComponent;

public class DestroyComponent : IComponent
{
    public bool WasDestroyed { get; private set; }

    public void Destroy()
    {
        WasDestroyed = true;
    }
}

[TestFixture]
public class EntityTests
{
    private class TestEntity(long id) : Entity.Entity(id);

    [Test]
    public void Constructor_SetsId()
    {
        var entity = new TestEntity(42);
        Assert.That(entity.Id, Is.EqualTo(42));
    }

    [Test]
    public void RegisterComponent_AddsComponent()
    {
        var entity = new TestEntity(1);
        var component = new TestComponent();
        entity.RegisterComponent(component);
        Assert.That(entity.Components, Contains.Item(component));
    }

    [Test]
    public void RegisterComponent_DuplicateType_DoesNotAdd()
    {
        var entity = new TestEntity(1);
        var comp1 = new TestComponent();
        var comp2 = new TestComponent();
        entity.RegisterComponent(comp1);
        entity.RegisterComponent(comp2);
        Assert.That(entity.Components.Count, Is.EqualTo(1));
        Assert.That(entity.Components[0], Is.EqualTo(comp1));
    }

    [Test]
    public void RemoveComponent_Existing_RemovesIt()
    {
        var entity = new TestEntity(1);
        var component = new TestComponent();
        entity.RegisterComponent(component);
        entity.RemoveComponent<TestComponent>();
        Assert.That(entity.Components, Is.Empty);
    }

    [Test]
    public void RemoveComponent_NonExistent_DoesNothing()
    {
        var entity = new TestEntity(1);
        entity.RemoveComponent<TestComponent>();
        Assert.That(entity.Components, Is.Empty);
    }

    [Test]
    public void Destroy_CallsDestroyOnComponents()
    {
        var entity = new TestEntity(1);
        var component = new DestroyComponent();
        entity.RegisterComponent(component);
        entity.Destroy();
        Assert.That(component.WasDestroyed, Is.True);
    }
}