//
// 	GalaxyScene.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.UI;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  /// <summary>
  /// The scene displayed at the galaxy view - handles the control over all systems, loading, and
  /// content management for the scene.
  /// </summary>
  public class GalaxyScene : Scene
  {
    /// <summary>
    /// The seed to use in generating the galaxy and star systems
    /// </summary>
    private int _seed,
    /// <summary>
    /// The current frame in the loading animation
    /// </summary>
    _animFrame,
    /// <summary>
    /// Amount of time to display each frame in the animation
    /// </summary>
    _animTime;

    /// <summary>
    /// Indicates whether the galaxy scene is still loading or not
    /// </summary>
    private bool _loading;

    private Vector2 _lastPos;

    /// <summary>
    /// The last results of the players scan
    /// </summary>
    private List<string> _scanResults;

    /// <summary>
    /// Main menu and HUD font
    /// </summary>
    private SpriteFont _benderLarge;

    /// <summary>
    /// The ships texture
    /// </summary>
    private Texture2D _ship,
    /// <summary>
    /// Texture used for each star system
    /// </summary>
    _solarSystem,
    /// <summary>
    /// The star field background
    /// </summary>
    _background;

    /// <summary>
    /// Song to play in the background
    /// </summary>
    private Song _bgSong;

    /// <summary>
    /// The thruster sound trigger played when the player moves
    /// </summary>
    private SoundTrigger _thrusterSound;

    /// <summary>
    /// The textures to use in the loading animation
    /// </summary>
    private List<Texture2D> _loadingTextures;

    /// <summary>
    /// Builds the galaxy and contains all data related to it.
    /// </summary>
    private GalaxyBuilder _galaxy;

    /// <summary>
    /// The HUD to display in the galaxy view.
    /// </summary>
    private GalaxyHud _hud;

    /// <summary>
    /// Thread to use when building the galaxy for the first time. Allows loading animation and
    /// input handling.
    /// </summary>
    private Thread _galaxyBuildThread;

    /// <summary>
    /// The cache of all planets visited - cleaned automatically when a planet hasn't been visited
    /// for a while. Allows fast loading of planets.
    /// </summary>
    private Dictionary<string, Planet> _planetCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.GalaxyScene"/> class.
    /// Loads all resources and sets up the galaxy for generation.
    /// </summary>
    /// <param name="map">Game objects to use.</param>
    /// <param name="content">Content manager to use.</param>
    public GalaxyScene(EntityMap map, ContentManager content) : base(map, content)
    {
      //TODO: Load from file here
      _seed = 7822800; //HACK: Hardcoded seed value for galaxy
      _loading = true;
      _animTime = _animFrame = 0;

      _hud = new GalaxyHud(content);
      _scanResults = new List<string>();
      _planetCache = new Dictionary<string, Planet>();
      _galaxyBuildThread = new Thread(new ThreadStart(BuildGalaxy));

      _ship = content.Load<Texture2D>("Images/playership_blue");
      _solarSystem = content.Load<Texture2D>("Images/starsystem");
      _background = content.Load<Texture2D>("Images/stars");
      _loadingTextures = new List<Texture2D>();

      _benderLarge = content.Load<SpriteFont>("Fonts/Bender Large");

      _bgSong = content.Load<Song>("Audio/galaxy");
      _thrusterSound = new SoundTrigger(content.Load<SoundEffect>("Audio/engine"));
      _thrusterSound.IsLooped = true;

      // Get all animation textures for animating the loading sprite
      var animDir = content.RootDirectory + "/Images/loading_galaxy";
      var files = Directory.GetFiles(animDir);
      for ( int i = 0; i < files.Length; i++ ) {
        var fileName = files[i].Replace(".xnb", "").Replace("Content/", "");
        _loadingTextures.Add(
          content.Load<Texture2D>(fileName)
        );
      }

    }

    /// <summary>
    /// Initializes the galaxy generation and background music - sets
    /// up the players ship and the collision bounds.
    /// </summary>
    public override void Initialize()
    {
      // Play BG music
      if ( MediaPlayer.GameHasControl ) {
        MediaPlayer.Play(_bgSong);
        MediaPlayer.IsRepeating = true;
      }

      // Check if galaxy has been generated and generate it on another thread
      // if it hasn't
      if ( _galaxy == null ) {

        _galaxy = new GalaxyBuilder(Content, 4000, _seed);
        _galaxyBuildThread.Start();

        // Setup the player and physics environment
        BuildPlayerShip(0, 0);

        var physics = GameObjects.GetSystem<PhysicsSystem>();
        if ( physics != null ) {
          (physics as PhysicsSystem).Environment = new PhysicsEnvironment {
            Inertia = 0.999f,
            RotationInertia = 0.98f
          };
        }

        // Setup the collision grid to use for the galaxy
        var collision = GameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
        if ( collision != null ) {
          var collisionSize = _galaxy.Bounds;
          collision.ResetGrid(
            _galaxy.Bounds.Left,
            _galaxy.Bounds.Right,
            _galaxy.Bounds.Top,
            _galaxy.Bounds.Bottom,
            180
          ); //HACK: Hardcoded collision cell size
        }
      }

      if ( !_loading ) {
        _galaxyBuildThread.Join();
      }

      // End transition and go to Update() and Draw() methods.
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Handles all input for the player ship and moves between galaxy and star system
    /// scenes if the player enters a star system.
    /// </summary>
    public override void HandleInput()
    {
      // Only handle input systems if finished loading
      if ( !_loading ) {

        GameObjects.GetSystem<InputSystem>().Run();
        GameObjects.GetSystem<ShipInputSystem>().Run();

        var player = GameObjects["player"];
        var shipController = player.GetComponent<ShipController>();

        // Check for request to enter star system and transition scenes if they have
        if ( shipController.State == ShipState.Landing && player.HasComponent<CollisionComponent>() ) {
          _lastPos = player.GetComponent<Movement>().Position;

          var collision = player.GetComponent<CollisionComponent>();
          var sys = collision.Collider.GetComponent<StarSystem>();

          shipController.State = ShipState.Normal;
          // Go to star system
          SceneController.Push(new StarSystemScene(GameObjects, Content, sys, _planetCache, _seed));
        }

      }
    }

    /// <summary>
    /// Updates the galaxy view and fuel consumption if not loading.
    /// </summary>
    public override void Update()
    {
      if ( GameObjects["player"].HasComponent<Inventory>() && !_loading ) {
        _hud.Update();

        GameObjects.GetSystem<CollisionSystem>().Run();

        UpdateSounds(GameObjects["player"]); // updates the players thruster sound

        GameObjects.GetSystem<PhysicsSystem>().Run();
        GameObjects.GetSystem<MovementSystem>().Run();
        GameObjects.GetSystem<DepthSystem>().Run();

        _hud.Refresh(
          GameObjects["player"].GetComponent<Inventory>()
        );
      }
    }

    /// <summary>
    /// Draw the game world and UI to the specified spriteBatch and uiSpriteBatch.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch for world-based entities.</param>
    /// <param name="uiSpriteBatch">User interface sprite batch.</param>
    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      // Only render the world if finished loading
      if ( _loading ) {
        AnimateLoading(spriteBatch);
      } else {
        spriteBatch.Draw(_background, MBGame.Camera.Position);

        GameObjects.GetSystem<RenderSystem>().Run();
        var galaxyRenderer = GameObjects.GetSystem<GalaxyRenderSystem>() as GalaxyRenderSystem;
        galaxyRenderer.Run();

        // Show scan results in the list control if the player collided with a star system
        if ( galaxyRenderer.InfoList.Count > 0 ) {
          _scanResults = galaxyRenderer.InfoList;
          (_hud["scan results"] as ListControl).Elements = _scanResults;
        }

        _hud.Draw(spriteBatch, uiSpriteBatch);
      }
    }

    /// <summary>
    /// Exit this scene instantly.
    /// </summary>
    public override void Exit()
    {
      TransitionState = TransitionState.Null;
    }

    /// <summary>
    /// Fades the sound out when transitioning to another scene.
    /// </summary>
    public override void Pause()
    {
      var fadeSpeed = 0.8f;
      if ( MediaPlayer.Volume > 0 ) {
        MediaPlayer.Volume -= fadeSpeed;
      } else {
        MediaPlayer.Pause();
        TransitionState = TransitionState.None;
      }
    }

    /// <summary>
    /// Resets the physics environment when returning to the scene and rebuilds the
    /// galaxy from its cache.
    /// </summary>
    public override void Resume()
    {
      GameObjects.Clear();

      var physics = GameObjects.GetSystem<PhysicsSystem>() as PhysicsSystem;
      physics.Environment = new PhysicsEnvironment {
        Inertia = 0.999f,
        RotationInertia = 0.98f
      };
      BuildPlayerShip((int)_lastPos.X, (int)_lastPos.Y);
      BuildGalaxy();

      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Updates the players thruster sounds.
    /// </summary>
    /// <param name="player">Player entity.</param>
    private void UpdateSounds(Entity player)
    {
      var physics = player.GetComponent<PhysicsComponent>();

      // Fade the sound up if any thrust was applied otherwise fade out
      if ( physics != null && (physics.Power > 0 || physics.Power < 0) ) {
        _thrusterSound.FadeUp();
      } else {
        _thrusterSound.FadeDown();
      }
    }

    /// <summary>
    /// Animates the loading screen
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw the animation to.</param>
    private void AnimateLoading(SpriteBatch spriteBatch)
    {
      var center = MBGame.Camera.GetBoundingRectangle().Center;
      var loadingStr = "PLEASE BE PATIENT WHILE YOUR UNIVERSE IS CREATED...";

      // Get the pos of the text to draw
      var measureStr = _benderLarge.MeasureString(loadingStr);
      center.Y -= 200;
      center.X -= measureStr.X / 2;

      // Draw the text
      spriteBatch.DrawString(
        _benderLarge,
        loadingStr,
        center,
        Color.White
      );

      // Render the next frame in the animation
      var loadingTexture = _loadingTextures[_animFrame];

      // At the center pos
      center = MBGame.Camera.GetBoundingRectangle().Center;
      center.X -= loadingTexture.Width / 2;
      center.Y -= loadingTexture.Height / 2;

      spriteBatch.Draw(_loadingTextures[_animFrame], center);

      // Go to next frame if current cells finished animation
      _animTime++;
      if ( _animTime > 2 ) {

        _animTime = 0;
        _animFrame++;

        if ( _animFrame > _loadingTextures.Count - 1 ) {
          _animFrame = 0;
        }
      }
    }

    /// <summary>
    /// Builds the player entity as a ship.
    /// </summary>
    private void BuildPlayerShip(int x, int y)
    {
      var player = GameObjects["player"];
      GameObjects.UseBlueprint("galaxy playership", player);
      player.GetComponent<Movement>().Position = new Vector2(x, y);
      // Refocus camera
      MBGame.Camera.LookAt(player.GetComponent<SpriteTransform>().Target.Origin);

      GameObjects.UpdateSystems(player);
    }

    /// <summary>
    /// Builds the galaxy. If the galaxy doesn't exist yet, it will generate it from
    /// scratch, otherwise will just reassemble the parts.
    /// </summary>
    private void BuildGalaxy()
    {
      var systems = _galaxy.StarSystems;

      // generate the galaxy if this is the first time seeing it
      if ( !_galaxy.Done ) {
        systems = _galaxy.Generate(_galaxy.Size * 3);
      }

      var scale = 0.5f;

      // Build the star systems from the galaxy's metadata
      foreach ( var sysComponent in systems ) {

        var newSystem = new Entity(GameObjects);

        // Setup sprite
        var sprite = newSystem.Attach<SpriteTransform>(
          _solarSystem,
          new Vector2(sysComponent.Bounds.X, sysComponent.Bounds.Y),
          new Vector2(scale, scale)
        ) as SpriteTransform;
        sprite.Target.Color = sysComponent.Color;
        sprite.Target.Position = new Vector2(sysComponent.Bounds.X, sysComponent.Bounds.Y);

        // Setup AABB
        newSystem.Attach<CollisionComponent>(new RectangleF[] {
          sprite.Target.GetBoundingRectangle()
        });

        newSystem.Attach(sysComponent);

        GameObjects.UpdateSystems(newSystem);
      }

      // Reset the collision map with all the new star system entities
      var collision = GameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
      if ( collision != null ) {
        var collisionSize = _galaxy.Bounds;
        collision.ResetGrid(
          _galaxy.Bounds.Left,
          _galaxy.Bounds.Right,
          _galaxy.Bounds.Top,
          _galaxy.Bounds.Bottom,
          180
        ); //HACK: Hardcoded collision cell size
      }

      _loading = false;
    }

  }
}
