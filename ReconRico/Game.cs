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

        EntityManager.ClearEntities();

        try
        {
            LevelManager.LoadLevel("level1");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load level: " + ex.Message);
        }
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