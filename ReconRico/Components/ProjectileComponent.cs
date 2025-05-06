namespace ReconRico.Components;

public class ProjectileComponent : IComponent
{
    public int MaxRicochetTimes { get; set; } = 0;
    public int RicochetTimes { get; set; } = 0;
}