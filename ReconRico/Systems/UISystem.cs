using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.General;

namespace ReconRico.Systems;

public class UISystem(SpriteBatch spriteBatch)
{
    public void Render(GameTime gameTime, GameState gameState)
    {
        spriteBatch.Begin();
        if (gameState == GameState.Paused)
        {
            var font = AssetsManager.DefaultFont;

            var pauseText = AssetsManager.PAUSE_TEXT;
            var textSize = font.MeasureString(pauseText);
            var position = new Vector2(
                (GameSettings.WINDOW_WIDTH - textSize.X) / 2,
                (GameSettings.WINDOW_HEIGHT - textSize.Y) / 2
            );

            spriteBatch.DrawString(font, pauseText, position, Color.Black);
        }
        else if (gameState == GameState.GameOver)
        {
            
        }

        spriteBatch.End();
    }
}