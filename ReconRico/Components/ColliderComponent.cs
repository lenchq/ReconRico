using Microsoft.Xna.Framework;

namespace ReconRico.Components;

public class ColliderComponent : IComponent
{
    public ColliderShape Shape { get; set; }
    public Vector2 Collider { get; set; }
}

public enum ColliderShape
{
    Circle,
    Rectangle
}