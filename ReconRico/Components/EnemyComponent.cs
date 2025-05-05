using Microsoft.Xna.Framework;
using ReconRico.General;

namespace ReconRico.Components;

public class EnemyComponent : IComponent
{
    public Point[] PatrolPoints { get; set; }
    public float hearRadius { get; set; } = GameSettings.ENEMY_HEAR_RADIUS;
    public float viewRadius { get; set; } = GameSettings.ENEMY_VIEW_RADIUS;

}