using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class EnemySystem
{
    private const float MovementSpeed = .3f;
    private const float PatrolPointReachedDistance = 10f;
    private Entity? _player;

    public void Update(GameTime gameTime)
    {
        // Find player if not already found
        if (_player == null)
        {
            _player = EntityManager.GetEntitiesWithComponent<PlayerComponent>()
                .FirstOrDefault();
            if (_player == null) return;
        }

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        foreach (var enemy in EntityManager.GetEntitiesWithAll(typeof(EnemyComponent)))
        {
            var enemyComponent = enemy.GetComponent<EnemyComponent>();
            var transform = enemy.GetComponent<TransformComponent>();
            var velocity = enemy.GetComponent<VelocityComponent>();

            // Update alert timer
            if (enemyComponent.IsAlerted)
            {
                enemyComponent.AlertTimer -= deltaTime;
                if (enemyComponent.AlertTimer <= 0)
                {
                    enemyComponent.IsAlerted = false;
                    enemyComponent.LastKnownPlayerPosition = null;
                }
            }

            // Check for player detection
            bool canSeePlayer = CanSeePlayer(enemy, transform);
            bool canHearPlayer = CanHearPlayer(enemy, transform);

            if (canSeePlayer || canHearPlayer)
            {
                // Update last known position and alert state
                enemyComponent.IsAlerted = true;
                enemyComponent.AlertTimer = enemyComponent.AlertDuration;
                enemyComponent.LastKnownPlayerPosition = _player.GetComponent<TransformComponent>().Position;
            }

            // Move towards target (either patrol point or player)
            Vector2 targetPosition;
            if (enemyComponent.IsAlerted && enemyComponent.LastKnownPlayerPosition.HasValue)
            {
                targetPosition = enemyComponent.LastKnownPlayerPosition.Value;
            }
            else
            {
                targetPosition = enemyComponent.PatrolPoints[enemyComponent.CurrentPatrolIndex];
            }

            // Calculate direction to target
            Vector2 direction = targetPosition - transform.Position;
            if (direction != Vector2.Zero &&
                Vector2.Distance(transform.Position, targetPosition) > 2f)
            {
                direction.Normalize();

                // Update velocity
                velocity.Velocity = direction * MovementSpeed;

                // Update rotation to face movement direction
                transform.Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
            }
            else
            {
                velocity.Velocity = Vector2.Zero;
            }

            // Check if reached patrol point
            if (!enemyComponent.IsAlerted &&
                Vector2.Distance(transform.Position, targetPosition) < PatrolPointReachedDistance)
            {
                // Move to next patrol point
                enemyComponent.CurrentPatrolIndex =
                    (enemyComponent.CurrentPatrolIndex + 1) % enemyComponent.PatrolPoints.Length;
            }
        }
    }

    private bool CanSeePlayer(Entity enemy, TransformComponent enemyTransform)
    {
        if (_player == null) return false;

        var enemyComponent = enemy.GetComponent<EnemyComponent>();
        var playerTransform = _player.GetComponent<TransformComponent>();

        // Check distance
        float distance = Vector2.Distance(enemyTransform.Position, playerTransform.Position);
        if (distance > enemyComponent.VisionRadius) return false;

        // Check if player is within vision angle
        Vector2 directionToPlayer = playerTransform.Position - enemyTransform.Position;
        directionToPlayer.Normalize();

        // Convert enemy's forward direction to angle
        float enemyAngle = enemyTransform.Rotation - MathHelper.PiOver2; // Adjust for sprite orientation
        Vector2 enemyForward = new Vector2(
            (float)Math.Cos(enemyAngle),
            (float)Math.Sin(enemyAngle)
        );

        // Calculate angle between enemy's forward direction and direction to player
        float angle = (float)Math.Acos(Vector2.Dot(enemyForward, directionToPlayer));
        float angleDegrees = MathHelper.ToDegrees(angle);

        // Check if angle is within vision cone
        if (angleDegrees > enemyComponent.VisionAngle / 2) return false;

        // Check for line of sight (no walls in between)
        return !HasLineOfSightBlocked(enemyTransform.Position, playerTransform.Position);
    }

    private bool CanHearPlayer(Entity enemy, TransformComponent enemyTransform)
    {
        if (_player == null) return false;

        var enemyComponent = enemy.GetComponent<EnemyComponent>();
        var playerTransform = _player.GetComponent<TransformComponent>();

        // Check if player is within hearing radius
        float distance = Vector2.Distance(enemyTransform.Position, playerTransform.Position);
        return distance <= enemyComponent.HearRadius;
    }

    private bool HasLineOfSightBlocked(Vector2 start, Vector2 end)
    {
        foreach (var wall in EntityManager.GetEntitiesWithComponent<ObstacleComponent>())
        {
            var wallTransform = wall.GetComponent<TransformComponent>();
            var wallCollider = wall.GetComponent<ColliderComponent>();
            var wallRect = new Rectangle(
                (int)(wallTransform.Position.X + wallCollider.Offset.X - wallCollider.Collider.X / 2),
                (int)(wallTransform.Position.Y + wallCollider.Offset.Y - wallCollider.Collider.Y / 2),
                (int)wallCollider.Collider.X,
                (int)wallCollider.Collider.Y
            );

            return LineIntersectsRectangle(start, end, wallRect);
        }

        return false;
    }

    private bool LineIntersectsRectangle(Vector2 start, Vector2 end, Rectangle rect)
    {
        // Check if line intersects any of the rectangle's edges
        Vector2[] corners = new Vector2[]
        {
            new Vector2(rect.Left, rect.Top),
            new Vector2(rect.Right, rect.Top),
            new Vector2(rect.Right, rect.Bottom),
            new Vector2(rect.Left, rect.Bottom)
        };

        for (int i = 0; i < 4; i++)
        {
            Vector2 p1 = corners[i];
            Vector2 p2 = corners[(i + 1) % 4];

            if (LinesIntersect(start, end, p1, p2))
            {
                return true;
            }
        }

        return false;
    }

    private bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        float denominator = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);

        if (denominator == 0)
            return false;

        float ua = ((p4.X - p3.X) * (p1.Y - p3.Y) - (p4.Y - p3.Y) * (p1.X - p3.X)) / denominator;
        float ub = ((p2.X - p1.X) * (p1.Y - p3.Y) - (p2.Y - p1.Y) * (p1.X - p3.X)) / denominator;

        return ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
    }
}