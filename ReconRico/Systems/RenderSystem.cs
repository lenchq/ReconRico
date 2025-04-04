using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.Components;
using ReconRico.Entity;
using ReconRico.General;

namespace ReconRico.Systems;

public class RenderSystem(SpriteBatch spriteBatch)
{
    public void Render(GameTime gameTime)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        foreach (var entity in EntityManager.GetEntitiesWithAll(
                     typeof(SpriteComponent),
                     typeof(TransformComponent)))
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            var transform = entity.GetComponent<TransformComponent>();

            var transformCenter = transform.Position
                                  - new Vector2(sprite.Texture.Width / 2f, sprite.Texture.Height / 2f);

            spriteBatch.Draw(sprite.Texture,
                transformCenter,
                null,
                sprite.ColorMask,
                transform.Rotation,
                Vector2.Zero,
                transform.Scale,
                sprite.SpriteEffects,
                transform.Layer);
        }

        if (GameSettings.DEBUG_DRAW_TRANSFORM)
        {
            foreach (var entity in EntityManager.GetEntitiesWithAll(
                         typeof(TransformComponent),
                         typeof(HitBoxComponent)))
            {
                var transform = entity.GetComponent<TransformComponent>();

                DrawBorderedRectangle(transform.Position.ToPoint(), new Point(4), Color.Lime, 2);
            }
        }
        // Draw transform

        if (GameSettings.DEBUG_DRAW_HITBOX)
        {
            foreach (var entity in EntityManager.GetEntitiesWithAll(
                         typeof(TransformComponent),
                         typeof(HitBoxComponent)))
            {
                var hitbox = entity.GetComponent<HitBoxComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                DrawBorderedRectangle(transform.Position.ToPoint(), hitbox.HitBox.ToPoint(), Color.Lime, 2);
            }
        }

        spriteBatch.End();
    }

    private Texture2D CreateRectangle(int width, int height, Color? fillColor = null, int border = 0,
        Color? borderColor = null)
    {
        fillColor ??= Color.Transparent;
        borderColor ??= Color.Transparent;

        var rect = new Texture2D(spriteBatch.GraphicsDevice, width, height);
        var colors = new Color[height][];
        for (var i = 0; i < height; i++)
            colors[i] = Enumerable.Repeat((Color)fillColor, width).ToArray();

        for (var i = 0; i < height; i++)
        for (var j = 0; j < width; j++)
            if (i < border || i >= height - border ||
                j < border || j >= width - border)
                colors[i][j] = (Color)borderColor;

        rect.SetData(colors
            .SelectMany(x => x)
            .ToArray());

        return rect;
    }

    private void DrawBorderedRectangle(Point position, Point size, Color color, int border)
    {
        var rect = CreateRectangle(size.X, size.Y, border: border, borderColor: color);

        spriteBatch.Draw(rect,
            new Rectangle((int)(position.X - size.X / 2f),
                (int)(position.Y - size.Y / 2f), size.X,
                size.Y),
            Color.GreenYellow);
    }
}