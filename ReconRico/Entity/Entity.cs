using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ReconRico.Components;

namespace ReconRico;

public class Entity(long id)
{
    public long Id { get; } = id;
    public List<IComponent> Components => _components.Values.ToList();

    private readonly Dictionary<Type, IComponent> _components = new();

    public void RegisterComponent(IComponent component)
    {
        var componentType = component.GetType();
        if (_components.ContainsKey(componentType))
            Console.WriteLine($"[E_{Id}]: Components {componentType} already registered");

        _components.Add(component.GetType(), component);
    }

    public void RemoveComponent<T>()
    {
        if (_components.ContainsKey(typeof(T)))
            _components.Remove(typeof(T));
        else
            Console.WriteLine($"[E_{Id}]: Cannot remove component {typeof(T)}");
    }

    public T GetComponent<T>() where T : IComponent => (T)_components[typeof(T)];
    public bool TryGetComponent<T>(out T component) where T : IComponent
    {
        if (!_components.ContainsKey(typeof(T)))
        {
            component = default;
            return false;
        }
        
        component = (T)_components[typeof(T)];
        return true;
    }

    public bool HasComponent<T>() => _components.ContainsKey(typeof(T));

    public bool HasAnyComponent(params Type[] types) =>
        _components.Values.Any(component => types.Contains(component.GetType()));

    public virtual void Destroy()
    {
        foreach (var component in _components.Values)
        {
            component.Destroy();
            _components.Remove(component.GetType());
        }
        Console.WriteLine($"[E_{Id}]: Entity destroyed");
    }
}