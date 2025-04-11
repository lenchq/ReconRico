using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReconRico.Components;
using Color = Microsoft.Xna.Framework.Color;

namespace ReconRico;

public class EntityBuilder
{
    private Entity _entity;

    public EntityBuilder WithTransform(Vector2? position = null, float rotation = 0f,
        Vector2? scale = null, int layer = 0)
    {
        _entity.RegisterComponent(new TransformComponent
        {
            Position = position ?? Vector2.Zero,
            Rotation = rotation,
            Scale = scale ?? Vector2.One,
            Layer = layer,
        });
        return this;
    }

    public EntityBuilder WithSprite(Texture2D sprite, SpriteEffects effects = SpriteEffects.None,
        Color? colorMask = null)
    {
        _entity.RegisterComponent(new SpriteComponent
        {
            Texture = sprite,
            SpriteEffects = effects,
            ColorMask = colorMask ?? Color.White,
        });
        return this;
    }

    public EntityBuilder WithVelocity(Vector2? startVelocity = null)
    {
        _entity.RegisterComponent(new VelocityComponent
        {
            Velocity = startVelocity ?? Vector2.Zero,
        });
        return this;
    }

    public EntityBuilder WithCollider(Vector2 collider)
    {
        _entity.RegisterComponent(new ColliderComponent
        {
            Collider = collider,
            Shape = ColliderShape.Rectangle,
        });
        return this;
    }

    public EntityBuilder WithPlayer()
    {
        _entity.RegisterComponent(new PlayerComponent());
        return this;
    }

    public void Reset()
    {
        _entity = EntityManager.CreateEntity();
    }

    public Entity Build()
    {
        var entity = _entity;
        Reset();
        return entity;
    }

    public Entity Create()
    {
        var entity = _entity;
        Reset();
        EntityManager.AddEntity(entity);
        return entity;
    }
}