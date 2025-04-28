using Microsoft.Xna.Framework;
using ReconRico.Components;

namespace ReconRico.Systems;

public class ScriptSystem
{
    public void Update(GameTime gameTime)
    {
        foreach (var entity in EntityManager.GetEntitiesWithComponent<ScriptComponent>())
        {
            entity.GetComponent<ScriptComponent>()
                .OnUpdate?.Invoke(entity, gameTime);
        }
    }
}