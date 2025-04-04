using Microsoft.Xna.Framework;

namespace ReconRico.Component;

public class Transform : IComponent
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Scale = Vector2.Zero;
    public int Layer = 0;
    /// <summary>
    /// Rotation in degrees
    /// </summary>
    public float Rotation = 0;
}