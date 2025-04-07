using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReconRico.Components;
using ReconRico.General;
using ReconRico.Systems;

namespace ReconRico;

public class Game : Microsoft.Xna.Framework.Game
{
    Texture2D ballTexture;
    (int ballX, int ballY) ballPos = (50, 50);

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private RenderSystem _renderSystem;
    private MovementSystem _movementSystem;
    private PlayerControlSystem _playerControlSystem;
    private ColliderSystem _colliderSystem;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = GameSettings.WINDOW_WIDTH;
        _graphics.PreferredBackBufferHeight = GameSettings.WINDOW_HEIGHT;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _movementSystem = new MovementSystem();
        _playerControlSystem = new PlayerControlSystem();
        _colliderSystem = new ColliderSystem();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _renderSystem = new RenderSystem(_spriteBatch);

        ballTexture = Content.Load<Texture2D>("sprites/ball");

        var entity = EntityManager.CreateEntity();
        entity.RegisterComponent(new TransformComponent
        {
            Position = new Vector2(ballPos.ballX, ballPos.ballY),
        });
        entity.RegisterComponent(new VelocityComponent
        {
            Velocity = new Vector2(1f, 0)
        });
        entity.RegisterComponent(new SpriteComponent
        {
            Texture = ballTexture,
        });
        entity.RegisterComponent(new ColliderComponent()
        {
            Collider = new Vector2(32, 16),
        });
        entity.RegisterComponent(new PlayerComponent());

        var entity1 = EntityManager.CreateEntity();
        entity1.RegisterComponent(new TransformComponent
        {
            Position = new Vector2(200, 200),
        });
        entity1.RegisterComponent(new SpriteComponent
        {
            Texture = ballTexture,
        });
        entity1.RegisterComponent(new ColliderComponent()
        {
            Collider = new Vector2(32, 32),
        });
        entity.RegisterComponent(new ColliderResponse
        {
            OnCollision = (e) =>
            {
                Console.WriteLine($"Collision {e.SourceId} {e.TargetId} {e.ContactPoint}");
                var target = EntityManager.Entities[e.TargetId];
                if (target.TryGetComponent<VelocityComponent>(out var velocity)
                    && target.TryGetComponent<TransformComponent>(out var transform))
                {
                    
                }
            }
        });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _playerControlSystem.Update(gameTime);
        _colliderSystem.Update(gameTime);
        _movementSystem.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        // TODO: Add your drawing code here
        _renderSystem.Render(gameTime);

        base.Draw(gameTime);
    }
}