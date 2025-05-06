using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.General;

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
            {
                DrawPauseScreen();

                if (_prevGameState != GameState.Paused)
                    SfxManager.PlayPause();
                break;
            }
            case GameState.Playing when _prevGameState == GameState.Paused:
                SfxManager.PlayPause();
                break;
            case GameState.GameOver:
            {
                // TODO game over screen
                break;
            }
        }

        spriteBatch.End();
        _prevGameState = gameState;
    }

    private void DrawUserInterface()
    {
        spriteBatch.Draw(AssetsManager.Bullet,
            new Vector2(0, 0),
            null,
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            1);
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
}