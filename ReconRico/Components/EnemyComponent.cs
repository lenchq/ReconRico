using Microsoft.Xna.Framework;
using ReconRico.General;

namespace ReconRico.Components;

public class EnemyComponent : IComponent
{
    public Vector2[] PatrolPoints { get; set; }
    public int CurrentPatrolIndex { get; set; }
    public float HearRadius { get; set; }
    public float VisionRadius { get; set; }
    public float VisionAngle { get; set; } // Field of view in degrees
    public bool IsAlerted { get; set; }
    public Vector2? LastKnownPlayerPosition { get; set; }
    public float AlertDuration { get; set; } // How long to stay alert after losing sight of player
    public float AlertTimer { get; set; }

    public EnemyComponent(Vector2[] patrolPoints, float hearRadius = 200f, float visionRadius = 300f, float visionAngle = 120f)
    {
        PatrolPoints = patrolPoints;
        CurrentPatrolIndex = 0;
        HearRadius = hearRadius;
        VisionRadius = visionRadius;
        VisionAngle = visionAngle;
        IsAlerted = false;
        LastKnownPlayerPosition = null;
        AlertDuration = 1500f; // 3 seconds
        AlertTimer = 0f;
    }
}