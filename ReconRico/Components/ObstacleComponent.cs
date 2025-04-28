namespace ReconRico.Components;

public class ObstacleComponent
{
    public CollisionType CollisionType { get; set; }
    /// <summary>
    /// Breakable obstacle, destroys after collision event.
    /// </summary>
    public bool IsBreakable { get; set; } = false;
}

public enum CollisionType
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