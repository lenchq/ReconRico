using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class EnemySystem
{
    private const float MovementSpeed = .5f;
    private const float PatrolPointReachedDistance = 10f;
    private readonly WeakReference<Entity?> _playerRef = new(null, false);

    public void Update(GameTime gameTime)
    {
        // Find player if not already found
        if (!_playerRef.TryGetTarget(out var player)
            || player.IsDestroyed)
        {
            var playerEntity = EntityManager.GetEntitiesWithComponent<PlayerComponent>()
                .FirstOrDefault();
            if (playerEntity is null)
                return;
            _playerRef.SetTarget(playerEntity);
            player = playerEntity;
        }

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        foreach (var enemy in EntityManager.GetEntitiesWithAll(typeof(EnemyComponent)))
        {
            var enemyComponent = enemy.GetComponent<EnemyComponent>();
            var transform = enemy.GetComponent<TransformComponent>();
            var velocity = enemy.GetComponent<VelocityComponent>();

            var wasAlerted = enemyComponent.IsAlerted;

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
                enemyComponent.LastKnownPlayerPosition = player.GetComponent<TransformComponent>().Position;

                if (!wasAlerted)
                    SfxManager.PlayAlarm();
            }

            // Move towards target (either patrol point or player)
            Vector2 targetPosition;
            if (enemyComponent.IsAlerted && enemyComponent.LastKnownPlayerPosition.HasValue)
                targetPosition = enemyComponent.LastKnownPlayerPosition.Value;
            else if (enemyComponent.PatrolPoints.Length > 0)
                targetPosition = enemyComponent.PatrolPoints[enemyComponent.CurrentPatrolIndex];
            else // No patrol points, no player visible
                return;

            var direction = targetPosition - transform.Position;
            if (direction != Vector2.Zero &&
                Vector2.Distance(transform.Position, targetPosition) > 2f)
            {
                direction.Normalize();
                velocity.Velocity = direction * MovementSpeed;
                transform.Rotation = (float)Math.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;
            }
            else
                velocity.Velocity = Vector2.Zero;

            // Check if enemy is close enough to destroy player
            if (Vector2.Distance(transform.Position, player.GetComponent<TransformComponent>().Position) <= 70f)
            {
                player.Destroy();
                SfxManager.PlayExplosion();
            }

            if (!enemyComponent.IsAlerted &&
                Vector2.Distance(transform.Position, targetPosition) < PatrolPointReachedDistance)
                // Move to the next patrol point
                enemyComponent.CurrentPatrolIndex =
                    (enemyComponent.CurrentPatrolIndex + 1) % enemyComponent.PatrolPoints.Length;
        }
    }

    private bool CanSeePlayer(Entity enemy, TransformComponent enemyTransform)
    {
        if (!_playerRef.TryGetTarget(out var player))
            return false;

        var enemyComponent = enemy.GetComponent<EnemyComponent>();
        var playerTransform = player.GetComponent<TransformComponent>();

        // Check distance
        var distance = Vector2.Distance(enemyTransform.Position, playerTransform.Position);
        if (distance > enemyComponent.VisionRadius)
            return false;

        // Check if player is within vision angle
        var directionToPlayer = playerTransform.Position - enemyTransform.Position;
        directionToPlayer.Normalize();

        // Convert enemy's forward direction to angle
        var enemyAngle = enemyTransform.Rotation - MathHelper.PiOver2; // Adjust for sprite orientation
        var enemyForward = new Vector2(
            (float)Math.Cos(enemyAngle),
            (float)Math.Sin(enemyAngle)
        );

        // Calculate angle between enemy's forward direction and direction to player
        var angle = (float)Math.Acos(Vector2.Dot(enemyForward, directionToPlayer));
        var angleDegrees = MathHelper.ToDegrees(angle);

        // Check if angle is within vision cone
        if (angleDegrees > enemyComponent.VisionAngle / 2) return false;

        // Check for line of sight (no walls in between)
        return !HasLineOfSightBlocked(enemyTransform.Position, playerTransform.Position);
    }

    private bool CanHearPlayer(Entity enemy, TransformComponent enemyTransform)
    {
        if (!_playerRef.TryGetTarget(out var player))
            return false;

        var enemyComponent = enemy.GetComponent<EnemyComponent>();
        var playerTransform = player.GetComponent<TransformComponent>();

        // if player is within hearing radius
        var dist = Vector2.Distance(enemyTransform.Position, playerTransform.Position);
        return dist <= enemyComponent.HearRadius;
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
        // Check intersection on any of the rectangle edges
        Vector2[] corners =
        [
            new(rect.Left, rect.Top),
            new(rect.Right, rect.Top),
            new(rect.Right, rect.Bottom),
            new(rect.Left, rect.Bottom)
        ];

        for (var i = 0; i < 4; i++)
        {
            var p1 = corners[i];
            var p2 = corners[(i + 1) % 4];

            if (LinesIntersect(start, end, p1, p2))
                return true;
        }

        return false;
    }

    private static bool LinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        var denominator = (p4.Y - p3.Y) * (p2.X - p1.X) - (p4.X - p3.X) * (p2.Y - p1.Y);

        if (denominator == 0)
            return false;

        var ua = (
            (p4.X - p3.X) * (p1.Y - p3.Y)
            - (p4.Y - p3.Y) * (p1.X - p3.X)
        ) / denominator;
        var ub = (
            (p2.X - p1.X) * (p1.Y - p3.Y)
            - (p2.Y - p1.Y) * (p1.X - p3.X)
        ) / denominator;

        return ua is >= 0 and <= 1 && ub is >= 0 and <= 1;
    }
}