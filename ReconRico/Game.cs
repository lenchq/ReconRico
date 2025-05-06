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
    private EnemySystem _enemySystem;
    private UiSystem _uiSystem;
    private LevelEditorSystem _levelEditorSystem;
    private DoorSystem _doorSystem;

    private GameState _gameState = GameState.Playing;
    private KeyboardState _previousKeyboardState;
    private int _currentLevel = 1;

    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }

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
        _enemySystem = new EnemySystem();
        _levelEditorSystem = new LevelEditorSystem();
        _doorSystem = new DoorSystem(this);

        _previousKeyboardState = Keyboard.GetState();

        if (GameSettings.GRAB_MOUSE)
            SDL_SetWindowGrab(this.Window.Handle, true);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _renderSystem = new RenderSystem(_spriteBatch);
        _uiSystem = new UiSystem(_spriteBatch);

        AssetsManager.Initialize(Content);
        SfxManager.Initialize(Content);

        LoadCurrentLevel();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var state = Keyboard.GetState();

        if (IsKeyDown(Keys.R))
        {
            LoadCurrentLevel();
            _gameState = GameState.Playing;
            return;
        }

        if (EntityManager.GetEntitiesWithComponent<PlayerComponent>().FirstOrDefault() is null)
            _gameState = GameState.GameOver;
        else if (IsKeyDown(Keys.Tab))
            _gameState = _gameState == GameState.Playing ? GameState.Paused : GameState.Playing;

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
            _enemySystem.Update(gameTime);
            _levelEditorSystem.Update(Window.Position, gameTime);
            _doorSystem.Update(gameTime);
        }

        base.Update(gameTime);
        return;

        bool IsKeyDown(Keys key)
        {
            return state.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        _renderSystem.Render(gameTime);
        _uiSystem.Render(gameTime, _gameState);

        base.Draw(gameTime);
    }

    public void LoadCurrentLevel()
    {
        var levelName = $"level{_currentLevel}";
        try
        {
            SfxManager.SoundsEnabled = false;
            EntityManager.DestroyEntities();
            LevelManager.LoadLevel(levelName);
            SfxManager.SoundsEnabled = true;
            SfxManager.PlayResume();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load level {levelName}: {ex.Message}");
        }
    }

    [System.Runtime.InteropServices.DllImport("SDL2.dll",
        CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl, EntryPoint = "SDL_SetWindowGrab")]
    public static extern int SDL_SetWindowGrab(IntPtr window, bool grabbed);
}