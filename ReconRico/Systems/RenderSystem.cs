using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.Components;
using ReconRico.General;
using System.Linq;

namespace ReconRico.Systems;

public class RenderSystem(SpriteBatch spriteBatch)
{
    private TextureCreator _textureCreator = new TextureCreator(spriteBatch);

    public void Render(GameTime gameTime)
    {
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        RenderWalls();
        RenderSprites();
        RenderDebugTransform();
        RenderDebugCollider();

        spriteBatch.End();
    }

    private void RenderWalls()
    {
        foreach (var entity in EntityManager.GetEntitiesWithAll(
                     typeof(ObstacleComponent),
                     typeof(TransformComponent)))
        {
            var obstacle = entity.GetComponent<ObstacleComponent>();
            var transform = entity.GetComponent<TransformComponent>();
            var collider = entity.GetComponent<ColliderComponent>();

            var offset = new Vector2(collider.Collider.X / 2f, collider.Collider.Y / 2f);

            var texture = CreateWallTexture(obstacle, collider);

            spriteBatch.Draw(texture,
                transform.Position,
                null,
                Color.White,
                transform.Rotation,
                offset,
                transform.Scale,
                SpriteEffects.None,
                0);
        }
    }

    private Texture2D CreateWallTexture(ObstacleComponent obstacle, ColliderComponent collider)
    {
        var size = collider.Collider.ToPoint();

        if (obstacle.IsBreakable)
            return obstacle.BulletCollisionType switch
            {
                // Glass
                BulletCollisionType.Pass =>
                    _textureCreator.CreateBorderedRectangle(size.X, size.Y, new Color(173, 216, 230), 4,
                        Color.Azure),
                // Wood
                BulletCollisionType.Absorb
                    => _textureCreator.CreateBorderedRectangle(size.X, size.Y, new Color(245, 200, 117), 8,
                        Color.Brown),
                // Something...
                _ => _textureCreator.CreateBorderedRectangle(size.X, size.Y, Color.Gray)
            };

        // Non-breakable walls
        // Reflectable wall 
        if (obstacle.BulletCollisionType == BulletCollisionType.Reflect)
            return _textureCreator.CreateBorderedRectangle(size.X, size.Y, Color.Gray);

        // Non-breakable & non-reflectable = solid wall
        return _textureCreator.CreateBorderedRectangle(size.X, size.Y, Color.DarkGray);
    }

    private void RenderDebugCollider()
    {
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
    }

    private void RenderDebugTransform()
    {
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
    }

    private void RenderSprites()
    {
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
    }

    private void DrawBorderedRectangle(Point position, Point size, Color color, int border, float rotation)
    {
        var rect = _textureCreator.CreateBorderedRectangle(size.X, size.Y, border: border, borderColor: color);

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