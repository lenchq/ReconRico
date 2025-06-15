using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReconRico.Components;
using ReconRico.General;

namespace ReconRico.Systems;

public class PlayerControlSystem
{
    public void Update(Game game)
    {
        if (!game.IsActive) return;

        var player = EntityManager.GetEntitiesWithComponent<PlayerComponent>()
            .FirstOrDefault();

        // Player is probably dead =)
        if (player is null)
            return;

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

        var mouse = Mouse.GetState();
        var cursor = EntityManager.GetEntitiesWithComponent<GameCursorComponent>()
            .FirstOrDefault();

        if (cursor is not null)
            RotateToGameCursor(cursor, player, ref moveVelocity);

        if (keyboard.IsKeyDown(GameSettings.PLAYER_MOVE_LEFT_KEY))
        {
            moveVelocity -= Vector2.UnitX * GameSettings.PLAYER_MOVE_HORIZONTAL_SPEED;
        }
        else if (keyboard.IsKeyDown(GameSettings.PLAYER_MOVE_RIGHT_KEY))
        {
            moveVelocity += Vector2.UnitX * GameSettings.PLAYER_MOVE_HORIZONTAL_SPEED;
        }

        var gun = player.GetComponent<GunComponent>();
        gun.ShootRequested = mouse.LeftButton == ButtonState.Pressed;

        var playerVelocity = player.GetComponent<VelocityComponent>();
        playerVelocity.Velocity += moveVelocity;
    }

    private void RotateToGameCursor(Entity cursor, Entity player, ref Vector2 moveVelocity)
    {
        var cursorPos = cursor.GetComponent<TransformComponent>();
        var playerPos = player.GetComponent<TransformComponent>();

        var fromPlayerToCursor = cursorPos.Position - playerPos.Position;
        var fromPlayerToCursorAngle =
            (float)Math.Atan2(fromPlayerToCursor.Y, fromPlayerToCursor.X) + MathHelper.PiOver2;

        playerPos.Rotation = fromPlayerToCursorAngle;

        moveVelocity.Rotate(playerPos.Rotation);
    }


    public static bool IsMouseInBounds(MouseState mouse)
    {
        return mouse.X is > 0 and < GameSettings.WINDOW_WIDTH
               && mouse.Y is > 0 and < GameSettings.WINDOW_HEIGHT;
    }
}