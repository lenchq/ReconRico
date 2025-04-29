namespace ReconRico.Components;

public class ObstacleComponent : IComponent
{
    public BulletCollisionType BulletCollisionType { get; set; }
    /// <summary>
    /// Breakable obstacle, destroys after collision event.
    /// </summary>
    public bool IsBreakable { get; set; } = false;
}

public enum BulletCollisionType
{
    /// <summary>
    /// Reflectable obstacle, allow bullets to ricochet.
    /// </summary>
    Reflect,

    /// <summary>
    /// Absorbable obstacle, absorbing any bullet every time.
    /// </summary>
    Absorb
}