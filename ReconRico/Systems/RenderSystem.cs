using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.Components;
using ReconRico.General;

namespace ReconRico.Systems;

public class RenderSystem(SpriteBatch spriteBatch)
{
    private readonly Dictionary<(Point size, Color? fillColor, int border, Color? borderColor), Texture2D> _gizmoCache =
        new();

    public void Render(GameTime gameTime)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        foreach (var entity in EntityManager.GetEntitiesWithAll(
                     typeof(SpriteComponent),
                     typeof(TransformComponent)))
        {
            var sprite = entity.GetComponent<SpriteComponent>();
            var transform = entity.GetComponent<TransformComponent>();

            var offset = new Vector2(sprite.Texture.Width / 2f, sprite.Texture.Height / 2f);

            spriteBatch.Draw(sprite.Texture,
                transform.Position,
                null,
                sprite.ColorMask,
                transform.Rotation,
                offset,
                transform.Scale,
                sprite.SpriteEffects,
                transform.Layer);
        }

        if (GameSettings.TRANSFORM_GIZMO)
        {
            foreach (var entity in EntityManager.GetEntitiesWithAll(
                         typeof(TransformComponent),
                         typeof(ColliderComponent)))
            {
                var transform = entity.GetComponent<TransformComponent>();

                DrawBorderedRectangle(transform.Position.ToPoint(), new Point(4), Color.Lime, 2, transform.Rotation);
            }
        }

        if (GameSettings.COLLIDER_GIZMO)
        {
            foreach (var entity in EntityManager.GetEntitiesWithAll(
                         typeof(TransformComponent),
                         typeof(ColliderComponent)))
            {
                var collider = entity.GetComponent<ColliderComponent>();
                var transform = entity.GetComponent<TransformComponent>();

                DrawBorderedRectangle((transform.Position + collider.Offset).ToPoint(), collider.Collider.ToPoint(),
                    Color.Lime, 2,
                    transform.Rotation);
            }
        }

        spriteBatch.End();
    }

    private Texture2D CreateRectangle(int width, int height, Color? fillColor = null, int border = 0,
        Color? borderColor = null)
    {
        var cacheKey = (new Point(width, height), fillColor, border, borderColor);
        if (_gizmoCache.TryGetValue(cacheKey, out var texture))
            return texture;

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

        _gizmoCache[cacheKey] = rect;

        return rect;
    }

    private void DrawBorderedRectangle(Point position, Point size, Color color, int border, float rotation)
    {
        var rect = CreateRectangle(size.X, size.Y, border: border, borderColor: color);

        var originToCenter = (size / new Point(2)).ToVector2();
        spriteBatch.Draw(rect,
            position.ToVector2(),
            null,
            Color.White,
            rotation,
            new Vector2(size.X / 2f, size.Y / 2f),
            1,
            SpriteEffects.None,
            1);
    }
}