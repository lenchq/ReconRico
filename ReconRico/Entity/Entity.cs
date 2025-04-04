using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReconRico.Component;

namespace ReconRico.Entity;

public abstract class Entity(long id)
{
    public long Id { get; } = id;
    public List<IComponent> Components => _components.Values.ToList();

    private readonly Dictionary<Type, IComponent> _components = new();

    public void RegisterComponent(IComponent component)
    {
        var componentType = component.GetType();
        if (_components.ContainsKey(componentType))
            Debug.WriteLine($"[E_{Id}]: Component {componentType} already registered");

        _components.Add(component.GetType(), component);
    }

    public void RemoveComponent<T>()
    {
        if (_components.ContainsKey(typeof(T)))
            _components.Remove(typeof(T));
        else
            Debug.WriteLine($"[E_{Id}]: Cannot remove component {typeof(T)}");
    }

    public bool HasComponent<T>() => _components.ContainsKey(typeof(T));

    public virtual void Destroy()
    {
        foreach (var component in _components.Values)
            component.Destroy();
        Debug.WriteLine($"[E_{Id}]: Entity destroyed");
    }
}