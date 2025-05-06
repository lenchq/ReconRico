using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.Components;
using ReconRico.General;

namespace ReconRico.Systems;

public class RenderSystem(SpriteBatch spriteBatch)
{
    private TextureCreator _creator = new TextureCreator(spriteBatch);

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

    private void DrawBorderedRectangle(Point position, Point size, Color color, int border, float rotation)
    {
        var rect = _creator.CreateBorderedRectangle(size.X, size.Y, border: border, borderColor: color);

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