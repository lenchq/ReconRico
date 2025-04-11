using NUnit.Framework;
using ReconRico.Components;

namespace ReconRico.Tests;

file class TestComponent : IComponent
{
    public bool WasDestroyed { get; private set; }
    public void Destroy() => WasDestroyed = true;
}

[TestFixture]
public class EntityTests
{
    private Entity _entity;

    [SetUp]
    public void Setup() => _entity = new Entity(1);

    [Test]
    public void Constructor_SetsId()
    {
        Assert.That(_entity.Id, Is.EqualTo(1));
    }

    [Test]
    public void RegisterComponent_AddsComponent()
    {
        var component = new TestComponent();
        _entity.RegisterComponent(component);

        Assert.That(_entity.Components, Has.Count.EqualTo(1));
        Assert.That(_entity.GetComponent<TestComponent>(), Is.EqualTo(component));
    }

    [Test]
    public void RemoveComponent_RemovesExistingComponent()
    {
        var component = new TestComponent();
        _entity.RegisterComponent(component);
        _entity.RemoveComponent<TestComponent>();

        Assert.That(_entity.HasComponent<TestComponent>(), Is.False);
    }

    [Test]
    public void GetComponent_ReturnsCorrectComponent()
    {
        var component = new TestComponent();
        _entity.RegisterComponent(component);

        Assert.That(_entity.GetComponent<TestComponent>(), Is.EqualTo(component));
    }

    [Test]
    public void HasComponent_ReturnsTrueForExisting()
    {
        _entity.RegisterComponent(new TestComponent());

        Assert.That(_entity.HasComponent<TestComponent>(), Is.True);
    }

    [Test]
    public void HasComponent_ReturnsFalseForNonExisting()
    {
        Assert.That(_entity.HasComponent<TestComponent>(), Is.False);
    }

    [Test]
    public void HasAnyComponent_ReturnsTrueForMatchingType()
    {
        _entity.RegisterComponent(new TestComponent());

        Assert.That(_entity.HasAnyComponent(typeof(TestComponent)), Is.True);
    }

    [Test]
    public void Destroy_CallsDestroyOnComponents()
    {
        var component = new TestComponent();
        _entity.RegisterComponent(component);
        _entity.Destroy();

        Assert.That(component.WasDestroyed, Is.True);
    }
}