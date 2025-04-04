using Microsoft.Xna.Framework;
using ReconRico.Components;
using ReconRico.Entity;

namespace ReconRico.Systems;

public class MovementSystem
{
    public void Update(GameTime gameTime)
    {
        foreach (var entity in
                 EntityManager.GetEntitiesWithAll(typeof(TransformComponent), typeof(VelocityComponent)))
        {
            var velocity = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();

            // Friction
            velocity.Velocity = 0.696969f * velocity.Velocity;

            // Movement
            transform.Position += velocity.Velocity * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}