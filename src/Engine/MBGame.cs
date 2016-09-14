using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace MidnightBlueMono
{
  /// <summary>
  /// This is the main type for your game.
  /// </summary>
  public class MBGame : Game
  {
    private static GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Color _bgColor;
    private Dictionary<string, SpriteFont> _fonts;
    private FramesPerSecondCounter _fps;
    private GameTime _gameTimer;
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

    private ECSMap _gameObjects;

    public MBGame()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      _fonts = new Dictionary<string, SpriteFont>();

      _scenes = new SceneStack();
      ForceQuit = false;
      _gameObjects = new ECSMap();
      _fps = new FramesPerSecondCounter();
      _gameTimer = new GameTime();
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

      //_graphics.IsFullScreen = true;
      _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
      _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
      _graphics.ApplyChanges();

      _bgColor = Color.MidnightBlue;
      Window.Title = "Midnight Blue";

      _debugConsole.AddVar("showFramerate", true);

      _debugConsole.AddFunc("GalaxyTest", (string[] args) => {
        int size, radius, seed = 0;
        int.TryParse(args[0], out size);
        int.TryParse(args[1], out radius);

        if ( args.Length > 2 ) {
          int.TryParse(args[2], out seed);
        }
        _scenes.Push(new GalaxyGenTest(_gameObjects, size, radius, seed), Content);
      });

      _debugConsole.AddFunc("EndCurrentScene", (string[] args) => _scenes.Pop());

      _gameObjects.AddSystem<InputSystem>();

      Entity player = new Entity("player");
      player.Attach<PlayerController>();
      player.Persistant = true;
      _gameObjects.AddEntity(player);

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

      _dt = _gameTimer.ElapsedGameTime.Seconds;
      _fps.Update(_dt);

      if ( (bool)_debugConsole.Vars["showFramerate"] ) {
        _spriteBatch.DrawString(Content.Load<SpriteFont>("SourceCode"), _fps.AverageFramesPerSecond.ToString("0"), new Vector2(0, 0), Color.White);
      }

      _spriteBatch.End();

      base.Draw(gameTime);
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

