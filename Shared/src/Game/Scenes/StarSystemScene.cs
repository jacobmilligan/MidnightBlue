//
// 	StarSystemScene.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 8/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MB2D;
using MB2D.EntityComponent;
using MB2D.Scenes;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  /// <summary>
  /// Scene to display a star system with planets and a star.
  /// </summary>
  public class StarSystemScene : Scene
  {
    /// <summary>
    /// Indicates whether the planets generation is loading or not.
    /// </summary>
    private bool _loading;
    /// <summary>
    /// The current frame in the loading animation sprite.
    /// </summary>
    private int _animFrame,
    /// <summary>
    /// The current time in the current animation frame
    /// </summary>
    _animTime;

    /// <summary>
    /// The background star field texture
    /// </summary>
    private Texture2D _background,
    /// <summary>
    /// The texture used for the systems star
    /// </summary>
    _star;

    /// <summary>
    /// All the textures in the current loading animation
    /// </summary>
    private List<Texture2D> _loadingTextures;

    /// <summary>
    /// The sound used for the ships thruster
    /// </summary>
    private SoundTrigger _thrusterSound;

    /// <summary>
    /// The current star systems information data.
    /// </summary>
    private StarSystem _starSystem;

    /// <summary>
    /// All the planet ojects in the star system after being generated.
    /// </summary>
    private Planet[] _planets;

    /// <summary>
    /// Used for generating and loading the planets.
    /// </summary>
    private Thread _planetLoader;

    /// <summary>
    /// The font used in the UI
    /// </summary>
    private SpriteFont _bender;

    /// <summary>
    /// The hud to display in the scene. Contains a map with all the planets, start, ship location.
    /// </summary>
    private StarSystemHud _hud;

    /// <summary>
    /// The random number generator used to generate the star systems planets.
    /// </summary>
    private Random _rand;

    /// <summary>
    /// The world-coordinates boundary of the star system
    /// </summary>
    private Rectangle _bounds;

    /// <summary>
    /// The las position of the player before entering a planet.
    /// </summary>
    private Vector2 _lasPos;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.StarSystemScene"/> class.
    /// 
    /// </summary>
    /// <param name="map">Entity map to load entities into.</param>
    /// <param name="content">Content manager for loading resources.</param>
    /// <param name="starSystem">Star system information to use for the scene.</param>
    /// <param name="cache">Planet cache used for quickly loading recently-visited star systems.</param>
    /// <param name="seed">Seed to use in random generation.</param>
    public StarSystemScene(
      EntityMap map, ContentManager content, StarSystem starSystem, Dictionary<string, Planet> cache, int seed
    ) : base(map, content)
    {
      _rand = new Random(seed);
      _loading = true;
      _background = content.Load<Texture2D>("Images/stars");
      _bender = content.Load<SpriteFont>("Fonts/Bender Large");
      _star = content.Load<Texture2D>("Images/star");
      _thrusterSound = new SoundTrigger(content.Load<SoundEffect>("Audio/engine"));
      _thrusterSound.IsLooped = true;

      _starSystem = starSystem;
      _planets = new Planet[starSystem.Planets.Count];


      var maxPlanetDistance = 1000;
      if ( _starSystem.Planets.Count > 0 ) {
        maxPlanetDistance = (
          _starSystem.Planets.Max(meta => meta.StarDistance.RelativeKilometers)
        );
      }

      BuildPlayer(maxPlanetDistance, maxPlanetDistance);

      var star = GameObjects.CreateEntity(_starSystem.Name);
      var starSprite = star.Attach<SpriteTransform>(
        _star, new Vector2(0, 0), new Vector2(2, 2)
      ) as SpriteTransform;
      star.Attach<CollisionComponent>(starSprite.Bounds);

      // Check the cache for recently visited planets before building new ones
      var p = 0;
      foreach ( var newPlanet in _starSystem.Planets ) {
        if ( !cache.ContainsKey(newPlanet.Name) ) {
          _planets[p] = new Planet(newPlanet, _rand.Next(seed));
          cache.Add(newPlanet.Name, _planets[p]);
        } else {
          _planets[p] = cache[newPlanet.Name];
          _planets[p].DisposeLayer("planet map");
        }
        p++;
      }

      _loadingTextures = new List<Texture2D>();
      var animDir = content.RootDirectory + "/Images/loading_planets";
      var files = Directory.GetFiles(animDir);
      for ( var i = 0; i < files.Length; i++ ) {
        var fileName = files[i].Replace(".xnb", "").Replace("Content/", "");
        _loadingTextures.Add(
          content.Load<Texture2D>(fileName)
        );
      }

      _planetLoader = new Thread(
        new ThreadStart(LoadPlanets)
      );
      _planetLoader.Start();

      _hud = new StarSystemHud(Content, GameObjects, SceneController);
    }

    /// <summary>
    /// Ends initializing instantly.
    /// </summary>
    public override void Initialize()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Handles moving the players ship.
    /// </summary>
    public override void HandleInput()
    {
      if ( !_loading ) {
        GameObjects.GetSystem<InputSystem>().Run();
        var shipInput = GameObjects.GetSystem<ShipInputSystem>() as ShipInputSystem;
        shipInput.Run();
      }
    }

    /// <summary>
    /// Updates all systems in the game and handles the ocurrance of the
    /// player entering a planet.
    /// </summary>
    public override void Update()
    {
      if ( !_loading && TransitionState != TransitionState.Pausing ) {
        GameObjects.GetSystem<CollisionSystem>().Run();

        UpdateSounds(GameObjects["player"]);

        GameObjects.GetSystem<PhysicsSystem>().Run();
        GameObjects.GetSystem<MovementSystem>().Run();
        GameObjects.GetSystem<DepthSystem>().Run();

        _hud.Update();

        var player = GameObjects["player"];
        var shipController = player.GetComponent<ShipController>();

        // Handle landing on planets
        if ( shipController.State == ShipState.Landing ) {
          var collision = player.GetComponent<CollisionComponent>();

          if ( collision != null ) {
            _lasPos = player.GetComponent<Movement>().Position;
            shipController.State = ShipState.Normal;

            ChangePlanetActive(false);

            var planet = collision.Collider.GetComponent<PlanetComponent>();
            SceneController.Push(new PlanetScene(GameObjects, Content, planet.Data));
          }

        } else if ( shipController.State == ShipState.LeavingScreen ) {
          SceneController.Pop();
        }
      }
    }

    /// <summary>
    /// Draws the star system to the sprite batch and the HUD to the UI spritebatch.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw world-based entities to.</param>
    /// <param name="uiSpriteBatch">User interface sprite batch.</param>
    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( TransitionState != TransitionState.Pausing ) {
        DrawNormal(spriteBatch, uiSpriteBatch);
      }
    }

    /// <summary>
    /// Draws the normal state of the star system when not loading. Handles drawing all entities.
    /// Updates the minimap based on players position.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw world-based entities to.</param>
    /// <param name="uiSpriteBatch">User interface sprite batch.</param>
    private void DrawNormal(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( !_loading && !_planetLoader.IsAlive ) {
        spriteBatch.Draw(_background, MBGame.Camera.Position);

        foreach ( var p in _planets ) {
          var map = p.GetMapLayer("planet map");
          if ( map == null ) {
            p.CreateMapTexture(Content);
            UpdateSpace(p);
          }
        }
      }

      if ( _loading || _planetLoader.IsAlive ) {
        AnimateLoading(uiSpriteBatch);
      } else {
        GameObjects.GetSystem<RenderSystem>().Run();

        _hud.Draw(spriteBatch, uiSpriteBatch);
        DrawMap(uiSpriteBatch);
      }
    }

    /// <summary>
    /// Updates the star system world space when planets finish being generated during gameplay
    /// and collision map and systems boundaries when finished loading.
    /// </summary>
    /// <param name="p">Planet to update.</param>
    private void UpdateSpace(Planet p)
    {
      // Get a random point on the planets orbit to place it at
      var randAngle = _rand.Next() * MathHelper.Pi * 2;
      var orbitX = (float)Math.Cos(randAngle) * p.Meta.StarDistance.RelativeKilometers;
      var orbitY = (float)Math.Sin(randAngle) * p.Meta.StarDistance.RelativeKilometers;
      // Get a random negative or positive 1
      var sign = _rand.Next(0, 1) * 2 - 1;
      p.Position = new Vector2(sign * orbitX, sign * orbitY);

      var planetEntity = GameObjects.CreateEntity(p.Name);
      var planetSprite = planetEntity.Attach<SpriteTransform>(
        p.GetMapLayer("planet map"), p.Position, new Vector2(1, 1)
      ) as SpriteTransform;
      var planetComponent = planetEntity.Attach<PlanetComponent>() as PlanetComponent;
      planetComponent.Data = p;
      planetEntity.Attach<CollisionComponent>(new RectangleF[] { planetSprite.Target.GetBoundingRectangle() });

      GameObjects.UpdateSystems(planetEntity);

      var x = _planets.Min(planet => planet.Position.X);
      var y = _planets.Min(planet => planet.Position.Y);
      var width = Math.Abs(_planets.Max(planet => planet.Position.X));
      var height = Math.Abs(_planets.Max(planet => planet.Position.Y));
      var collision = GameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
      if ( collision != null ) {
        // Cell size is hardcoded to be finely tuned for this specific scene
        collision.ResetGrid(
          (int)x, (int)(x + width), (int)y, (int)(y + height), 500
        );
      }
    }

    /// <summary>
    /// Draws the minimap to the window.
    /// </summary>
    /// <param name="uiSpriteBatch">User interface sprite batch to draw to.</param>
    public void DrawMap(SpriteBatch uiSpriteBatch)
    {
      var hudBounds = _hud["Map"].BoundingBox;

      var normalizedPos = new Vector2(0, 0);

      foreach ( var p in _planets ) {
        normalizedPos = (p.Position / hudBounds.Width);
        uiSpriteBatch.FillRectangle(
          hudBounds.Center.ToVector2() + normalizedPos,
          new Vector2(3, 3),
          Color.White
        );
      }

      var star = GameObjects[_starSystem.Name];
      if ( star != null ) {
        var starSprite = star.GetComponent<SpriteTransform>() as SpriteTransform;
        normalizedPos = (starSprite.Target.Position / hudBounds.Width);

        uiSpriteBatch.FillRectangle(
          hudBounds.Center.ToVector2() + normalizedPos,
          new Vector2(3, 3),
          Color.Red
        );
      }

      var player = GameObjects["player"].GetComponent<Movement>();
      normalizedPos = (player.Position / hudBounds.Width);

      uiSpriteBatch.FillRectangle(
        hudBounds.Center.ToVector2() + normalizedPos,
        new Vector2(3, 3),
        Color.Green
      );
    }

    /// <summary>
    /// Exits the scene instantly.
    /// </summary>
    public override void Exit()
    {
      // End transition instantly
      TransitionState = TransitionState.Null;
    }

    /// <summary>
    /// Pauses the scene instantly.
    /// </summary>
    public override void Pause()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Resumes the star system scene after leaving a planet. Handles resetting the physics
    /// environment, players ship settings and reactivates all planets and the star.
    /// </summary>
    public override void Resume()
    {
      if ( SceneController.LastSceneType == typeof(MenuScene) ) {
        TransitionState = TransitionState.None;
        return;
      }

      (GameObjects.GetSystem<PhysicsSystem>() as PhysicsSystem).Environment =
        new PhysicsEnvironment {
          Inertia = 0.999f,
          RotationInertia = 0.98f
        };

      BuildPlayer((int)_lasPos.X, (int)_lasPos.Y);

      ChangePlanetActive(true);

      GameObjects.EntitiesWithComponent<PlanetComponent>().First().Active = true;

      var collision = GameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
      if ( collision != null ) {
        collision.ResetGrid(_bounds.Left, _bounds.Right, _bounds.Top, _bounds.Bottom, 500);
      }

      // End transition instantly
      TransitionState = TransitionState.None;
    }

    private void ChangePlanetActive(bool activeState)
    {
      var planetEntities = GameObjects.EntitiesWithComponent<PlanetComponent>();

      foreach ( var p in planetEntities ) {
        p.Active = activeState;
        p.Persistent = !activeState;
      }

      GameObjects[_starSystem.Name].Active = activeState;
      GameObjects[_starSystem.Name].Persistent = !activeState;
    }

    /// <summary>
    /// Animates the loading sprite displayed when generating planets.
    /// </summary>
    /// <param name="uiSpriteBatch">User interface sprite batch to draw to.</param>
    private void AnimateLoading(SpriteBatch uiSpriteBatch)
    {
      var loadingTexture = _loadingTextures[_animFrame];

      var viewRect = MBGame.Graphics.Viewport.Bounds;
      var pos = new Vector2(viewRect.Width - loadingTexture.Width, viewRect.Height - loadingTexture.Height);

      if ( _loading ) {
        uiSpriteBatch.DrawString(
          _bender,
          "Loading...",
          new Vector2(0, 0),
          Color.White
        );
      }

      uiSpriteBatch.Draw(_loadingTextures[_animFrame], pos, scale: new Vector2(0.5f, 0.5f));

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
    /// Loads all planets for the star system. Checks if they've been generated already, and if
    /// not generates them and updates their position and collision components. Should be handled
    /// on a different thread as this is a lengthy, blocking process.
    /// </summary>
    private void LoadPlanets()
    {
      var movement = GameObjects["player"].GetComponent<Movement>();

      var planets = _planets.OrderBy(
        planet => Vector2.Distance(movement.Position, planet.Position)
      );

      if ( planets.Count() > 0 ) {
        var closestPlanet = planets.Last();

        if ( !closestPlanet.Generated ) {
          closestPlanet.Generate(new Random(_rand.Next()));
        }


        _loading = false;

        for ( int i = 0; i < _planets.Length; i++ ) {
          if ( !_planets[i].Generated ) {
            _planets[i].Generate(new Random(_rand.Next()));
          }
        }
      } else {
        _loading = false;
      }
    }

    /// <summary>
    /// Updates the player ships thruster sounds.
    /// </summary>
    /// <param name="player">Player to update.</param>
    private void UpdateSounds(Entity player)
    {
      var physics = player.GetComponent<PhysicsComponent>();

      if ( physics != null && (physics.Power > 0 || physics.Power < 0) ) {
        _thrusterSound.FadeUp();
      } else {
        _thrusterSound.FadeDown();
      }
    }

    /// <summary>
    /// Builds the player entity each time they need to be reset.
    /// </summary>
    /// <param name="x">The x coordinate to place the player at.</param>
    /// <param name="y">The y coordinate to place the player at.</param>
    private void BuildPlayer(int x, int y)
    {
      var player = GameObjects["player"];
      GameObjects.UseBlueprint("starsystem playership", player);
      player.GetComponent<Movement>().Position = new Vector2(x, y);
    }

  }
}
