using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReconRico.Components;
using ReconRico.Entity;
using ReconRico.General;

namespace ReconRico.Systems;

public class PlayerControlSystem
{
    public void Update(GameTime gameTime)
    {
        var player = EntityManager.GetEntityWithComponent<PlayerComponent>();

        var moveVelocity = Vector2.Zero;
        
        var keyboard = Keyboard.GetState();
        if (keyboard.IsKeyDown(GameSettings.PLAYER_MOVE_UP_KEY))
        {
            moveVelocity -= Vector2.UnitY * GameSettings.PLAYER_MOVE_VERTICAL_SPEED;
        }
        else if (keyboard.IsKeyDown(GameSettings.PLAYER_MOVE_DOWN_KEY))
        {
            moveVelocity += Vector2.UnitY * GameSettings.PLAYER_MOVE_VERTICAL_SPEED;
        }
        
        if (keyboard.IsKeyDown(GameSettings.PLAYER_MOVE_LEFT_KEY))
        {
            moveVelocity -= Vector2.UnitX * GameSettings.PLAYER_MOVE_HORIZONTAL_SPEED;
        }
        else if (keyboard.IsKeyDown(GameSettings.PLAYER_MOVE_RIGHT_KEY))
        {
            moveVelocity += Vector2.UnitX * GameSettings.PLAYER_MOVE_HORIZONTAL_SPEED;
        }

        var playerVelocity = player.GetComponent<VelocityComponent>();
        playerVelocity.Velocity += moveVelocity;
    }
}