using Microsoft.Xna.Framework;

namespace ReconRico.Components;

public class TransformComponent : IComponent
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Scale = Vector2.One;
    public int Layer = 0;
    /// <summary>
    /// Rotation in degrees
    /// </summary>
    public float Rotation = 0;
}