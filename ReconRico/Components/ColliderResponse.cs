using Microsoft.Xna.Framework;

namespace ReconRico.Components;

public class ColliderResponse : IComponent
{
    public Action<CollisionEvent> OnCollision { get; set; }
}
public struct CollisionEvent(long sourceId, long targetId, Vector2? contactPoint)
{
    public long SourceId => sourceId; 
    public long TargetId => targetId; 
    public Vector2? ContactPoint => contactPoint; // Точка контакта
}