using Microsoft.Xna.Framework;

namespace ReconRico.Components;

public class ColliderComponent : IComponent
{
    public ColliderShape Shape { get; set; }
    public Vector2 Collider { get; set; }
    public Vector2 Offset { get; set; }

    // public RotatedRectangle GetColliderRectangle(TransformComponent transform)
    // {
    //     return new RotatedRectangle(
    //         transform.Position + this.Offset,
    //         this.Collider,
    //         transform.Rotation
    //     );
    // }
    public Rectangle GetColliderRectangle(TransformComponent transform)
    {
        return new Rectangle(
            (int)(transform.Position.X + Offset.X - Collider.X / 2),
            (int)(transform.Position.Y + Offset.Y - Collider.Y / 2),
            (int)Collider.X,
            (int)Collider.Y
        );
    }
}

public enum ColliderShape
{
    Circle,
    Rectangle
}