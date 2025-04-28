using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReconRico.Components;
using ReconRico.General;

namespace ReconRico.Systems;

public class GameCursorSystem
{
    private MouseState _prevMouse = Mouse.GetState();
    private Vector2 _prevPlayerPos = Vector2.Zero;

    public void Update(Game game)
    {
        if (!game.IsActive) return;
        
        var player = EntityManager.GetEntitiesWithComponent<PlayerComponent>()
            .FirstOrDefault();
        var cursorEntity = EntityManager
            .GetEntitiesWithAll(typeof(TransformComponent), typeof(GameCursorComponent))
            .FirstOrDefault();

        if (cursorEntity is null
            || player is null) return;

        var gameCursorPos = cursorEntity.GetComponent<TransformComponent>();
        var playerPos = player.GetComponent<TransformComponent>();

        var currentMouse = Mouse.GetState();
        var deltaMouse = currentMouse.Position - _prevMouse.Position;
        var deltaPlayer = _prevPlayerPos - playerPos.Position;

        gameCursorPos.Position += deltaMouse.ToVector2() - deltaPlayer;

        if (gameCursorPos.Position.X >= GameSettings.WINDOW_WIDTH
            || gameCursorPos.Position.X <= 0
            || gameCursorPos.Position.Y >= GameSettings.WINDOW_HEIGHT
            || gameCursorPos.Position.Y <= 0)
        {
            gameCursorPos.Position = new Vector2(
                MathHelper.Clamp(gameCursorPos.Position.X, 0, GameSettings.WINDOW_WIDTH),
                MathHelper.Clamp(gameCursorPos.Position.Y, 0, GameSettings.WINDOW_HEIGHT)
            );
        }

        if (currentMouse.X <= 0
            || currentMouse.X >= GameSettings.WINDOW_WIDTH - 1
            || currentMouse.Y <= 0
            || currentMouse.Y >= GameSettings.WINDOW_HEIGHT - 1)
        {
            Mouse.SetPosition(GameSettings.WINDOW_WIDTH / 2, GameSettings.WINDOW_HEIGHT / 2);
            _prevMouse = Mouse.GetState();
        }
        else
            _prevMouse = currentMouse;

        _prevPlayerPos = playerPos.Position;
    }
}