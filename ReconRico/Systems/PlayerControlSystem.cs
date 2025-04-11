using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReconRico.Components;
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

        var mouse = Mouse.GetState();
        var mouseInBounds = mouse.X is > 0 and < GameSettings.WINDOW_WIDTH
                            && mouse.Y is > 0 and < GameSettings.WINDOW_HEIGHT;

        var transform = player.GetComponent<TransformComponent>();
        if (mouseInBounds)
        {
            var fromPlayerToCursor = mouse.Position.ToVector2() - transform.Position;
            var fromPlayerToCursorAngle =
                (float)Math.Atan2(fromPlayerToCursor.Y, fromPlayerToCursor.X) + MathHelper.PiOver2;

            transform.Rotation = fromPlayerToCursorAngle;
        }
        moveVelocity.Rotate(transform.Rotation);

        var playerVelocity = player.GetComponent<VelocityComponent>();
        playerVelocity.Velocity += moveVelocity;
    }
}