using Microsoft.Xna.Framework;
using ReconRico.Components;
using System.Linq;

namespace ReconRico.Systems;

public class DoorSystem(Game _game)
{

    public void Update(GameTime gameTime)
    {
        foreach (var door in EntityManager.GetEntitiesWithComponent<DoorComponent>()
                     .Where(ent => !ent.HasComponent<ColliderResponse>()))
        {
            door.RegisterComponent(new ColliderResponse()
            {
                OnCollision = (e) =>
                {
                    var source = EntityManager.Entities[e.SourceId];
                    var target = EntityManager.Entities[e.TargetId];

                    if (!source.HasComponent<PlayerComponent>() && !target.HasComponent<PlayerComponent>())
                        return;

                    _game.CurrentLevel++;
                    _game.LoadCurrentLevel();
                    SfxManager.PlayPickup();
                }
            });
        }
    }
}