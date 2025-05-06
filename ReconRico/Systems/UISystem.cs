using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.General;
using System.Linq;
using ReconRico.Components;

namespace ReconRico.Systems;

public class UiSystem(SpriteBatch spriteBatch)
{
    private GameState _prevGameState;
    private TextureCreator _textureCreator = new(spriteBatch);

    public void Render(GameTime gameTime, GameState gameState)
    {
        spriteBatch.Begin();

        DrawUserInterface();
        switch (gameState)
        {
            case GameState.Paused:
                DrawPauseScreen();

                if (_prevGameState != GameState.Paused)
                    SfxManager.PlayPause();
                break;
            case GameState.Playing when _prevGameState == GameState.Paused:
                SfxManager.PlayPause();
                break;
            case GameState.GameOver:
                DrawGameOverScreen();
                break;
        }

        spriteBatch.End();
        _prevGameState = gameState;
    }

    private void DrawUserInterface()
    {
        var player = EntityManager.GetEntitiesWithComponent<PlayerComponent>()
            .FirstOrDefault();
        if (player is null)
            return;
        var gun = player.GetComponent<GunComponent>();

        var windowSize = new Point(115, 70);
        var windowPos = new Vector2( // padding from bottom left corner
            GameSettings.WINDOW_WIDTH - windowSize.X - 20,
            GameSettings.WINDOW_HEIGHT - windowSize.Y - 20
        );

        var windowColor = gun.Ammo > 0
            ? Color.DarkGray
            : Color.Red;
        var windowRect =
            _textureCreator.CreateBorderedRectangle(windowSize.X, windowSize.Y, windowColor, 3, Color.Black);
        spriteBatch.Draw(windowRect, windowPos, Color.White);

        // bullet img
        var bulletPos = windowPos + new Vector2(25, windowSize.Y / 2f);
        spriteBatch.Draw(AssetsManager.Bullet,
            bulletPos,
            null,
            Color.White,
            0f,
            new Vector2(AssetsManager.Bullet.Width / 2f, AssetsManager.Bullet.Height / 2f),
            .8f,
            SpriteEffects.None,
            1f);

        // ammo count
        var ammoText = gun.Ammo.ToString();
        var textPos = windowPos + new Vector2(windowSize.X - 40, windowSize.Y / 2f);
        spriteBatch.DrawString(AssetsManager.DefaultFont,
            ammoText,
            textPos,
            Color.White,
            0f,
            AssetsManager.DefaultFont.MeasureString(ammoText) / 2f,
            1f,
            SpriteEffects.None,
            1f);
    }

    private void DrawPauseScreen()
    {
        var font = AssetsManager.DefaultFont;
        var pauseText = AssetsManager.PAUSE_TEXT;
        var textSize = font.MeasureString(pauseText);
        var position = new Vector2(
            (GameSettings.WINDOW_WIDTH - textSize.X) / 2,
            (GameSettings.WINDOW_HEIGHT - textSize.Y) / 2
        );

        var windowSize = new Point(500, 150);
        var rect = _textureCreator.CreateBorderedRectangle(windowSize.X, windowSize.Y, Color.DarkGray, 3, Color.Black);
        spriteBatch.Draw(rect,
            new Vector2(640 - windowSize.X / 2f,
                360 - windowSize.Y / 2f), Color.White);
        spriteBatch.DrawString(font, pauseText, position, Color.White);
    }

    private void DrawGameOverScreen()
    {
        var gameOverText = AssetsManager.GAME_OVER_TEXT;
        var font = AssetsManager.DefaultFont;
        var textSize = font.MeasureString(gameOverText);

        // Create 800x800 window centered on screen
        var windowSize = new Point(450, 100);
        var windowPos = new Vector2(
            (GameSettings.WINDOW_WIDTH - windowSize.X) / 2f,
            (GameSettings.WINDOW_HEIGHT - windowSize.Y) / 2f
        );

        // Draw red window with black border
        var windowRect = _textureCreator.CreateBorderedRectangle(windowSize.X, windowSize.Y, Color.Red, 3, Color.Black);
        spriteBatch.Draw(windowRect, windowPos, Color.White);

        // Draw "GAME OVER" text centered in window
        var textPos = windowPos + new Vector2(
            (windowSize.X - textSize.X) / 2f,
            (windowSize.Y - textSize.Y) / 2f
        );
        spriteBatch.DrawString(font, gameOverText, textPos, Color.White);
    }
}