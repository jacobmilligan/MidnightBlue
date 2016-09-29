using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.IO;
using MidnightBlue.Engine.Testing;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class MBGame : Game
  {
    private static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Color _bgColor;
    private FramesPerSecondCounter _fps;
    private float _dt;

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
      base.Initialize();

      this.IsMouseVisible = true;

      _graphics.GraphicsDevice.Viewport = new Viewport {
        X = 0,
        Y = 0,
        Width = _graphics.PreferredBackBufferWidth,
        Height = _graphics.PreferredBackBufferHeight,
      };

      _bgColor = Color.MidnightBlue;
      Window.Title = "Midnight Blue";

      SetUpDebugVals();

      _gameObjects.AddSystem<InputSystem>();

      Entity player = _gameObjects.CreateEntity("player");
      player.Attach<PlayerController>();
      player.Persistant = true;

      _scenes.ResetTo(new TitleScene(_gameObjects), Content);
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

      _debugConsole = new MBConsole(Color.Black, Color.Yellow, Content.Load<SpriteFont>("SourceCode"));
      _debugConsole.InitWindow(Graphics);

      //TODO: use this.Content to load your game content here 
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
        _scenes.Top.HandleInput();
        _scenes.Top.Update();
      }
      _debugConsole.Update();

      IOUtil.UpdateKeyState();
      base.Update(gameTime);
    }

    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
      _graphics.GraphicsDevice.Clear(_bgColor);

      _spriteBatch.Begin();

      if ( _scenes.Top != null ) {
        _scenes.Top.Draw(_spriteBatch);
      }

      _debugConsole.Draw(_spriteBatch);

      _dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
      _fps.Update(_dt);

      if ( (bool)_debugConsole.Vars["showFramerate"] ) {
        _spriteBatch.DrawString(Content.Load<SpriteFont>("SourceCode"), _fps.AverageFramesPerSecond.ToString("0"), new Vector2(0, 0), Color.White);
      }

      _spriteBatch.End();

      base.Draw(gameTime);
    }

    private void SetUpDebugVals()
    {
      _debugConsole.AddVar("showFramerate", false);
      _debugConsole.AddVar("drawBorders", false);
      _debugConsole.AddVar("drawGrids", false);

      _debugConsole.AddFunc("ToggleFullscreen", (string[] args) => _graphics.ToggleFullScreen());

      _debugConsole.AddFunc("GalaxyTest", (string[] args) => {
        int size, radius, seed = 0;
        int.TryParse(args[0], out size);
        int.TryParse(args[1], out radius);

        if ( args.Length > 2 ) {
          int.TryParse(args[2], out seed);
        }
        _scenes.Push(new GalaxyGenTest(_gameObjects, size, radius, seed), Content);
      });

      _debugConsole.AddFunc(
        "UITest",
        (string[] args) => _scenes.Push(new UITest(_gameObjects), Content)
      );

      _debugConsole.AddFunc("EndCurrentScene", (string[] args) => _scenes.Pop());
    }

    public static GraphicsDevice Graphics
    {
      get { return _graphics.GraphicsDevice; }
    }

    public static MBConsole Console
    {
      get { return _debugConsole; }
    }

    public static bool ForceQuit { get; set; }
  }
}

