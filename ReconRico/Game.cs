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

    private GameState _gameState = GameState.Playing;
    private KeyboardState _previousKeyboardState;

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

        _previousKeyboardState = Keyboard.GetState();
        
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

        KeyboardState state = Keyboard.GetState();
        if (state.IsKeyDown(Keys.P) && _previousKeyboardState.IsKeyUp(Keys.P))
        {
            _gameState = _gameState == GameState.Playing ? GameState.Paused : GameState.Playing;
        }

        _previousKeyboardState = state;

        if (_gameState == GameState.Playing)
        {
            _playerControlSystem.Update(this);
            _movementSystem.Update(gameTime);
            _colliderSystem.Update(gameTime);
            _rigidbodySystem.Update(gameTime);
            _gameCursorSystem.Update(this);
            _gunSystem.Update(gameTime);
            _scriptSystem.Update(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _renderSystem.Render(gameTime);

        if (_gameState == GameState.Paused)
        {
            _spriteBatch.Begin();

            var font = AssetsManager.DefaultFont;
            string pauseText = "Paused\nPress P to Resume";
            Vector2 textSize = font.MeasureString(pauseText);
            Vector2 position = new Vector2(
                (GameSettings.WINDOW_WIDTH - textSize.X) / 2,
                (GameSettings.WINDOW_HEIGHT - textSize.Y) / 2
            );

            _spriteBatch.DrawString(font, pauseText, position, Color.Black);

            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    [System.Runtime.InteropServices.DllImport("SDL2.dll",
        CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowGrab")]
    public static extern int SDL_SetWindowGrab(IntPtr window, bool grabbed);
}