using Microsoft.Xna.Framework;
using ReconRico.Components;
using System.Linq;

namespace ReconRico.Systems;

public class DoorSystem
{
    private readonly Game _game;

    public DoorSystem(Game game)
    {
        _game = game;
    }

    public void Update(GameTime gameTime)
    {
        var player = EntityManager.GetEntitiesWithComponent<PlayerComponent>().FirstOrDefault();
        if (player is null) return;

        var playerTransform = player.GetComponent<TransformComponent>();
        var playerCollider = player.GetComponent<ColliderComponent>();

        // Get player's collider rectangle
        var playerRect = new Rectangle(
            (int)(playerTransform.Position.X + playerCollider.Offset.X - playerCollider.Collider.X / 2),
            (int)(playerTransform.Position.Y + playerCollider.Offset.Y - playerCollider.Collider.Y / 2),
            (int)playerCollider.Collider.X,
            (int)playerCollider.Collider.Y
        );

        // Check for door collisions
        foreach (var door in EntityManager.GetEntitiesWithAll(typeof(DoorComponent), typeof(TransformComponent), typeof(ColliderComponent)))
        {
            var doorTransform = door.GetComponent<TransformComponent>();
            var doorCollider = door.GetComponent<ColliderComponent>();

            // Get door's collider rectangle
            var doorRect = new Rectangle(
                (int)(doorTransform.Position.X + doorCollider.Offset.X - doorCollider.Collider.X / 2),
                (int)(doorTransform.Position.Y + doorCollider.Offset.Y - doorCollider.Collider.Y / 2),
                (int)doorCollider.Collider.X,
                (int)doorCollider.Collider.Y
            );

            // If player is inside door's collider
            if (playerRect.Intersects(doorRect))
            {
                // Increment current level and reload
                _game.CurrentLevel++;
                _game.LoadCurrentLevel();
                SfxManager.PlayPickup(); // Play a sound effect for level transition
                break; // Exit after finding first door collision
            }
        }
    }
}