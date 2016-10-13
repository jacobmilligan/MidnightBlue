using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.IO;
using MidnightBlue.Engine.Testing;
using MonoGame.Extended.Shapes;
using System;
using MidnightBlue.Testing;

namespace MidnightBlue.Engine
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class MBGame : Game
  {
    private static GraphicsDeviceManager _graphics;
    private static FramesPerSecondCounter _fps;
    private static float _dt;
    private static Camera2D _camera;

    private SpriteBatch _spriteBatch, _uiSpriteBatch;
    private Color _bgColor;

    /// <summary>
    /// The games debug console. Static so as to be accessed globally for adding functions/variables
    /// to be tracked and altered via the console
    /// </summary>
    private static MBConsole _debugConsole;

    /// <summary>
    /// The scene stack in the current game. The scene at the top
    /// is always drawn and updated from the main loop
    /// </summary>
    private SceneStack _scenes;

    private EntityMap _gameObjects;

    public MBGame()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      _scenes = new SceneStack();
      ForceQuit = false;
      _gameObjects = new EntityMap();
      _fps = new FramesPerSecondCounter();

      _graphics.PreferredBackBufferWidth = 1280;
      _graphics.PreferredBackBufferHeight = 720;
      _graphics.ApplyChanges();
    }

    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {

      this.IsMouseVisible = true;

      _graphics.GraphicsDevice.Viewport = new Viewport {
        X = 0,
        Y = 0,
        Width = _graphics.PreferredBackBufferWidth,
        Height = _graphics.PreferredBackBufferHeight,
      };

      _camera = new Camera2D(Graphics);
      _camera.LookAt(new Vector2(0, 0));
      base.Initialize();

      _bgColor = Color.MidnightBlue;
      Window.Title = "Midnight Blue";

      SetUpDebugVals();
      RegisterSystems();

      Entity player = _gameObjects.CreateEntity("player");
      player.Attach<UtilityController>();
      player.Persistant = true;

      _scenes.ResetTo(new TitleScene(_gameObjects, Content));
      _fps.Reset();
    }

    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
      // Create a new SpriteBatch, which can be used to draw textures.
      _spriteBatch = new SpriteBatch(GraphicsDevice);
      _uiSpriteBatch = new SpriteBatch(GraphicsDevice);

      _debugConsole = new MBConsole(Color.Black, Color.Yellow, Content.Load<SpriteFont>("Fonts/SourceCode"));
      _debugConsole.InitWindow(Graphics);
    }

    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
      // For Mobile devices, this logic will close the Game when the Back button is pressed
      // Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
      if ( GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) )
        Exit();
#endif

      if ( _scenes.Top != null ) {
        _bgColor = _scenes.Top.WindowBackgroundColor;
        _scenes.Top.DeltaTime = _dt;
      }

      if ( ForceQuit ) {
        Exit();
      }

      _scenes.Update();

      _debugConsole.Update();

      IOUtil.UpdateKeyState();
      IOUtil.UpdateMouseState();

      var playerMovement = _gameObjects["player"].GetComponent<Movement>();
      if ( playerMovement != null ) {
        _camera.LookAt(
          playerMovement.Position
        );
      }

      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      _graphics.GraphicsDevice.Clear(_bgColor);

      _uiSpriteBatch.Begin();

      _spriteBatch.Begin(
        transformMatrix: _camera.GetViewMatrix(),
        samplerState: SamplerState.PointClamp
      );

      if ( _scenes.Top != null ) {
        _scenes.Top.Draw(_spriteBatch, _uiSpriteBatch);
      }

      if ( (bool)_debugConsole.Vars["showFramerate"] ) {
        _uiSpriteBatch.DrawString(
          Content.Load<SpriteFont>("Fonts/SourceCode"),
          _fps.AverageFramesPerSecond.ToString("0"),
          new Vector2(0, 0),
          Color.White
        );

        _uiSpriteBatch.DrawString(
          Content.Load<SpriteFont>("Fonts/SourceCode"),
          _dt.ToString(),
          new Vector2(50, 0),
          Color.White
        );
      }

      if ( (bool)_debugConsole.Vars["showCameraPos"] ) {
        _uiSpriteBatch.DrawString(
          Content.Load<SpriteFont>("Fonts/SourceCode"),
          Camera.Position.ToString(),
          new Vector2(200, 0),
          Color.White
        );
      }

      if ( (bool)_debugConsole.Vars["collisionChecks"] ) {
        var collision = _gameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
        _uiSpriteBatch.DrawString(
          Content.Load<SpriteFont>("Fonts/SourceCode"),
          "Collision checks: " + collision.NumberOfChecks,
          new Vector2(200, 0),
          Color.White
        );
      }

      if ( (bool)_debugConsole.Vars["drawCollision"] ) {
        _gameObjects.GetSystem<CollisionRenderSystem>().Run();
        var collisionMap = (_gameObjects.GetSystem<CollisionSystem>() as CollisionSystem).CurrentMap;
        if ( collisionMap != null ) {
          _spriteBatch.DrawGrid(collisionMap.Grid, Camera.Position.ToPoint(), Color.White);
        }
      }

      _debugConsole.Draw(_uiSpriteBatch);

      _spriteBatch.End();
      _uiSpriteBatch.End();

      _dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
      _fps.Update(_dt);

      base.Draw(gameTime);
    }

    private void SetUpDebugVals()
    {
      _debugConsole.AddVar("showFramerate", true);
      _debugConsole.AddVar("drawBorders", false);
      _debugConsole.AddVar("drawGrids", false);
      _debugConsole.AddVar("drawCollision", false);
      _debugConsole.AddVar("collisionChecks", true);
      _debugConsole.AddVar("systemRuntime", false);
      _debugConsole.AddVar("showCameraPos", false);

      _debugConsole.AddFunc("ToggleFullscreen", (string[] args) => _graphics.ToggleFullScreen());
      _debugConsole.AddFunc("PopScene", (string[] args) => _scenes.Pop());
      _debugConsole.AddFunc("SetSpeed", (string[] args) => {
        if ( args.Length > 0 ) {
          var newSpeed = 0.0f;
          var movement = _gameObjects["player"].GetComponent<Movement>();
          if ( float.TryParse(args[0], out newSpeed) && movement != null ) {
            movement.Speed = newSpeed;
          }
        }
      });

      _debugConsole.AddFunc("TestMap", (string[] args) => {
        var seed = 1090;
        var length = new Length((ulong)(Length.AstronomicalUnit * 5.9) * 1000);
        var planet = new Planet(
            new PlanetMetadata {
              Radius = 400000,
              SurfaceTemperature = 20,
              Type = PlanetType.Terrestrial,
              StarDistance = new Length(length.Kilometers)
            }, seed);
        planet.Generate(new Random(seed));
        _scenes.Push(new PlanetScene(_gameObjects, Content, planet));
      });

      _debugConsole.AddFunc("TestUI", (string[] args) => _scenes.Push(new UITest(_gameObjects, Content)));
    }

    private void RegisterSystems()
    {
      _gameObjects.AddSystem<InputSystem>();
      _gameObjects.AddSystem<ShipInputSystem>();
      _gameObjects.AddSystem<MovementSystem>();
      _gameObjects.AddSystem<CollisionSystem>();
      _gameObjects.AddSystem<PhysicsSystem>();
      _gameObjects.AddSystem<DepthSystem>();
      _gameObjects.AddSystem<RenderSystem>(_spriteBatch);
      _gameObjects.AddSystem<GalaxyRenderSystem>(_spriteBatch, Content);
      _gameObjects.AddSystem<CollisionRenderSystem>(_spriteBatch);
    }

    public static GraphicsDevice Graphics
    {
      get { return _graphics.GraphicsDevice; }
    }

    public static Camera2D Camera
    {
      get { return _camera; }
    }

    public static MBConsole Console
    {
      get { return _debugConsole; }
    }

    public static float DeltaTime
    {
      get { return _dt; }
    }

    public static float FPS
    {
      get { return _fps.AverageFramesPerSecond; }
    }

    public static bool ForceQuit { get; set; }
  }
}

