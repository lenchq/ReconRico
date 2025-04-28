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
            var rb = moving.GetComponent<RigidbodyComponent>();
            var tf = moving.GetComponent<TransformComponent>();
            var col = moving.GetComponent<ColliderComponent>();
            var vel = moving.GetComponent<VelocityComponent>();

            // Store original position for potential rollback
            var originalPos = tf.Position;
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Calculate the movement for this frame
            var movement = vel.Velocity * deltaTime;

            // Handle X-axis movement first
            if (Math.Abs(movement.X) > 0.001f)
            {
                // Apply X movement only
                tf.Position.X += movement.X;

                // Create bounds for the entity after X movement
                var xMovedBounds = new RotatedRectangle(tf.Position + col.Offset, col.Collider, tf.Rotation);

                // Check for collisions after X movement
                CheckIntersection(moving, xMovedBounds, otherBounds =>
                {
                    // Resolve X axis collision based on direction of movement
                    if (movement.X > 0) // moving right
                        //     // Place entity to the left of the collided object with a small gap
                        tf.Position.X = otherBounds.BottomLeft.X - col.Collider.X / 2 - col.Offset.X - 0.1f;
                    else // moving left
                        //     // Place entity to the right of the collided object with a small gap
                        tf.Position.X = otherBounds.BottomRight.X + col.Collider.X / 2 - col.Offset.X + 0.1f;

                    // Stop X velocity
                    vel.Velocity = new Vector2(0, vel.Velocity.Y);
                });
            }

            // Then handle Y-axis movement
            if (Math.Abs(movement.Y) > 0.001f)
            {
                // Apply Y movement only
                tf.Position.Y += movement.Y;

                // Create bounds for the entity after Y movement
                var yMovedBounds = new RotatedRectangle(tf.Position + col.Offset, col.Collider, tf.Rotation);

                // Check for collisions after Y movement
                CheckIntersection(moving, yMovedBounds, otherBounds =>
                {
                    // tf.Position.Y = otherBounds.TopLeft.Y - col.Collider.Y / 2 - col.Offset.Y - 0.01f;
                    // Resolve Y axis collision based on direction of movement
                    // if (movement.Y > 0) // moving down
                    //     //     // Place entity above the collided object with a small gap
                    //     tf.Position.Y = otherBounds.BottomLeft.Y - col.Collider.Y / 2 - col.Offset.Y - 0.1f;
                    // else // moving up
                    //     //     // Place entity below the collided object with a small gap
                    //     tf.Position.Y = otherBounds.BottomLeft.Y + col.Collider.Y / 2 + col.Offset.Y + 0.1f;

                    // Stop Y velocity
                    vel.Velocity = new Vector2(vel.Velocity.X, 0);
                });
            }
        }
    }

    public static void CheckIntersection(Entity moving, RotatedRectangle bounds,
        Action<RotatedRectangle> onIntersection)
    {
        foreach (var other in EntityManager.GetEntitiesWithAll(typeof(TransformComponent),
                     typeof(ColliderComponent), typeof(RigidbodyComponent)))
        {
            if (moving == other) continue;

            var oTf = other.GetComponent<TransformComponent>();
            var oCol = other.GetComponent<ColliderComponent>();

            var otherBounds = new RotatedRectangle(oTf.Position + oCol.Offset, oCol.Collider, oTf.Rotation);

            if (!bounds.Intersects(otherBounds)) continue;

            onIntersection.Invoke(otherBounds);
            break;
        }
    }
}