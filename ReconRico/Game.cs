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
    private SpriteBatch _spriteBatch;

    private RenderSystem _renderSystem;
    private MovementSystem _movementSystem;
    private PlayerControlSystem _playerControlSystem;
    private ColliderSystem _colliderSystem;
    private RigidbodySystem _rigidbodySystem;
    private GameCursorSystem _gameCursorSystem;
    private GunSystem _gunSystem;
    private ScriptSystem _scriptSystem;

    public Game()
    {
        var graphics = new GraphicsDeviceManager(this);
        graphics.PreferredBackBufferWidth = GameSettings.WINDOW_WIDTH;
        graphics.PreferredBackBufferHeight = GameSettings.WINDOW_HEIGHT;
        graphics.ApplyChanges();
        
        Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        Content.RootDirectory = "Content";
        IsMouseVisible = GameSettings.IS_MOUSE_VISIBLE;
    }

    protected override void Initialize()
    {
        _movementSystem = new MovementSystem();
        _playerControlSystem = new PlayerControlSystem();
        _colliderSystem = new ColliderSystem();
        _rigidbodySystem = new RigidbodySystem();
        _gameCursorSystem = new GameCursorSystem();
        _gunSystem = new GunSystem();
        _scriptSystem = new ScriptSystem();

        SDL_SetWindowGrab(this.Window.Handle, true);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _renderSystem = new RenderSystem(_spriteBatch);

        AssetsManager.Initialize(Content);

        var player = EntityManager.CreateEntity();
        player.RegisterComponent(new TransformComponent
        {
            Position = new Vector2(860, 720 / 2f),
        });
        player.RegisterComponent(new VelocityComponent
        {
            Velocity = new Vector2(1f, 0)
        });
        player.RegisterComponent(new SpriteComponent
        {
            Texture = AssetsManager.Ball,
        });
        player.RegisterComponent(new ColliderComponent()
        {
            Collider = new Vector2(32, 16),
        });
        player.RegisterComponent(new PlayerComponent());
        player.RegisterComponent(new RigidbodyComponent());
        player.RegisterComponent(new GunComponent(20, 100, 5)
        {
            Ammo = 9099
        });

        var wall = EntityManager.CreateEntity();
        wall.RegisterComponent(new TransformComponent
        {
            Position = new Vector2(0, 720 / 2f),
        });
        wall.RegisterComponent(new SpriteComponent
        {
            Texture = AssetsManager.Ball,
        });
        wall.RegisterComponent(new ColliderComponent()
        {
            Collider = new Vector2(50, 720),
        });
        wall.RegisterComponent(new RigidbodyComponent());

        var wall2 = EntityManager.CreateEntity();
        wall2.RegisterComponent(new TransformComponent
        {
            Position = new Vector2(600, 600),
        });
        wall2.RegisterComponent(new SpriteComponent
        {
            Texture = AssetsManager.Ball,
        });
        wall2.RegisterComponent(new ColliderComponent()
        {
            Collider = new Vector2(20, 600),
        });
        wall2.RegisterComponent(new RigidbodyComponent());

        var cursor = EntityManager.CreateEntity();
        cursor.RegisterComponent(new TransformComponent() { Position = new Vector2(300, 300) });
        cursor.RegisterComponent(new SpriteComponent()
        {
            Texture = AssetsManager.Cursor
        });
        cursor.RegisterComponent(new GameCursorComponent());
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _playerControlSystem.Update(this);
        _movementSystem.Update(gameTime);
        _colliderSystem.Update(gameTime);
        _rigidbodySystem.Update(gameTime);
        _gameCursorSystem.Update(this);
        _gunSystem.Update(gameTime);
        _scriptSystem.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _renderSystem.Render(gameTime);

        base.Draw(gameTime);
    }

    [System.Runtime.InteropServices.DllImport("SDL2.dll",
        CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowGrab")]
    public static extern int SDL_SetWindowGrab(IntPtr window, bool grabbed);
}