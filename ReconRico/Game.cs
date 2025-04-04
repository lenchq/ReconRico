﻿using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReconRico.Components;
using ReconRico.Entity;
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

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _movementSystem = new MovementSystem();
        _playerControlSystem = new PlayerControlSystem();
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
        entity.RegisterComponent(new HitBoxComponent()
        {
            HitBox = new Vector2(32, 16),
        });
        entity.RegisterComponent(new PlayerComponent());
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _playerControlSystem.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        // TODO: Add your drawing code here
        _renderSystem.Render(gameTime);
        _movementSystem.Update(gameTime);

        base.Draw(gameTime);
    }
}