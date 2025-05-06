using System;
using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class RigidbodySystem
{
    public void Update(GameTime gameTime)
    {
        foreach (var moving in EntityManager.GetEntitiesWithAll(typeof(RigidbodyComponent),
                     typeof(TransformComponent), typeof(ColliderComponent), typeof(VelocityComponent)))
        {
            var tf = moving.GetComponent<TransformComponent>();
            var col = moving.GetComponent<ColliderComponent>();
            var vel = moving.GetComponent<VelocityComponent>();

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            var movement = vel.Velocity * deltaTime;

            // X-axis movement
            if (Math.Abs(movement.X) > 0.001f)
            {
                tf.Position.X += movement.X;
                var xMovedBounds = new Rectangle(
                    (int)(tf.Position.X + col.Offset.X - col.Collider.X / 2),
                    (int)(tf.Position.Y + col.Offset.Y - col.Collider.Y / 2),
                    (int)col.Collider.X,
                    (int)col.Collider.Y
                );
                CheckIntersection(moving, xMovedBounds, otherBounds =>
                {
                    if (movement.X > 0) // moving right
                        tf.Position.X = otherBounds.Left - col.Collider.X / 2 - col.Offset.X - 0.1f;
                    else // moving left
                        tf.Position.X = otherBounds.Right + col.Collider.X / 2 - col.Offset.X + 0.1f;
                    // Stop X velocity
                    vel.Velocity = new Vector2(0, vel.Velocity.Y);
                });
            }

            // Y-axis movement
            if (Math.Abs(movement.Y) > 0.001f)
            {
                tf.Position.Y += movement.Y;
                var yMovedBounds = new Rectangle(
                    (int)(tf.Position.X + col.Offset.X - col.Collider.X / 2),
                    (int)(tf.Position.Y + col.Offset.Y - col.Collider.Y / 2),
                    (int)col.Collider.X,
                    (int)col.Collider.Y
                );
                CheckIntersection(moving, yMovedBounds, otherBounds =>
                {
                    if (movement.Y > 0) // moving down
                        tf.Position.Y = otherBounds.Top - col.Collider.Y / 2 - col.Offset.Y - 0.1f;
                    else // moving up
                        tf.Position.Y = otherBounds.Bottom + col.Collider.Y / 2 - col.Offset.Y + 0.1f;
                    // Stop Y velocity
                    vel.Velocity = new Vector2(vel.Velocity.X, 0);
                });
            }
        }
    }

    public static void CheckIntersection(Entity moving, Rectangle bounds,
        Action<Rectangle> onIntersection)
    {
        foreach (var other in EntityManager.GetEntitiesWithAll(typeof(TransformComponent),
                     typeof(ColliderComponent), typeof(RigidbodyComponent)))
        {
            if (moving == other) continue;

            var oTf = other.GetComponent<TransformComponent>();
            var oCol = other.GetComponent<ColliderComponent>();

            var otherBounds = new Rectangle(
                (int)(oTf.Position.X + oCol.Offset.X - oCol.Collider.X / 2),
                (int)(oTf.Position.Y + oCol.Offset.Y - oCol.Collider.Y / 2),
                (int)oCol.Collider.X,
                (int)oCol.Collider.Y
            );

            if (!bounds.Intersects(otherBounds)) continue;

            onIntersection.Invoke(otherBounds);
            break;
        }
    }
}