using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class MovementSystem
{
    const float FRICTION_PER_SECOND = 12f;

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var entity in EntityManager.GetEntitiesWithAll(typeof(TransformComponent), typeof(VelocityComponent)))
        {
            var velocity = entity.GetComponent<VelocityComponent>();
            var transform = entity.GetComponent<TransformComponent>();

            // Apply friction only if applicable
            if (!entity.HasComponent<ProjectileComponent>())
            {
                var decay = MathF.Exp(-FRICTION_PER_SECOND * deltaTime);
                velocity.Velocity *= decay;
            }

            transform.Position += velocity.Velocity * deltaTime;
        }
    }
}