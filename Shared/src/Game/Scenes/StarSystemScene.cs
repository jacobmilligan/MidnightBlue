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
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace MidnightBlue
{
  public class StarSystemScene : Scene
  {
    private bool _loading;
    private int _animFrame, _animTime;
    private Texture2D _ship, _background, _star;
    private List<Texture2D> _loadingTextures;
    private SoundEffect _thrusterSound;
    private SoundEffectInstance _thrusterInstance;
    private StarSystem _starSystem;
    private Planet[] _planets;
    private Thread _planetLoader;
    private SpriteFont _bender;
    private StarSystemHud _hud;
    private Random _rand;
    private Rectangle _bounds;

    public StarSystemScene(
      EntityMap map, ContentManager content, StarSystem starSystem, Dictionary<string, Planet> cache, int seed
    ) : base(map, content)
    {
      _rand = new Random(seed);
      _loading = true;
      _ship = content.Load<Texture2D>("Images/playership_blue");
      _background = content.Load<Texture2D>("Images/stars");
      _thrusterSound = content.Load<SoundEffect>("Audio/engine");
      _bender = content.Load<SpriteFont>("Fonts/Bender Large");
      _star = content.Load<Texture2D>("Images/star");
      _thrusterInstance = _thrusterSound.CreateInstance();
      _thrusterInstance.IsLooped = true;

      _starSystem = starSystem;
      _planets = new Planet[starSystem.Planets.Count];

      var player = GameObjects["player"];
      var movement = player.GetComponent<Movement>();
      movement.Speed = 25.0f;
      movement.RotationSpeed = 0.05f;

      var maxPlanetDistance = 1000;
      if ( _starSystem.Planets.Count > 0 ) {
        maxPlanetDistance = (
          _starSystem.Planets.Max(meta => meta.StarDistance.RelativeKilometers)
        );
      }

      var star = GameObjects.CreateEntity(_starSystem.Name);
      var starSprite = star.Attach<SpriteTransform>(
        _star,
        new Vector2(
          movement.Position.X - maxPlanetDistance,
          movement.Position.Y
        ), new Vector2(2, 2)
      ) as SpriteTransform;
      star.Attach<CollisionComponent>(starSprite.Bounds);

      //TODO: Replace with for loop
      var p = 0;
      foreach ( var planet in _starSystem.Planets ) {
        if ( !cache.ContainsKey(planet.Name) ) {
          _planets[p] = new Planet(planet, seed);
          cache.Add(planet.Name, _planets[p]);
        } else {
          _planets[p] = cache[planet.Name];
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

    public override void Initialize()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      if ( !_loading ) {
        GameObjects.GetSystem<InputSystem>().Run();
        var shipInput = GameObjects.GetSystem<ShipInputSystem>() as ShipInputSystem;
        shipInput.Run();
      }
      GameObjects.GetSystem<NavigationInputSystem>().Run();
    }

    public override void Update()
    {
      if ( !_loading ) {
        GameObjects.GetSystem<CollisionSystem>().Run();

        UpdateSounds(GameObjects["player"]);

        GameObjects.GetSystem<PhysicsSystem>().Run();
        GameObjects.GetSystem<MovementSystem>().Run();
        GameObjects.GetSystem<DepthSystem>().Run();

        _hud.Update();

        var shipInput = GameObjects.GetSystem<ShipInputSystem>() as ShipInputSystem;
        if ( shipInput.WillEnter ) {
          var collision = GameObjects["player"].GetComponent<CollisionComponent>();
          if ( collision != null && collision.Collider != null ) {
            var planet = collision.Collider.GetComponent<PlanetComponent>();

            SceneController.Push(new PlanetScene(GameObjects, Content, planet.Data));
          }
        }
      }
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( TransitionState != TransitionState.Pausing ) {
        DrawNormal(spriteBatch, uiSpriteBatch);
      }
    }

    private void DrawNormal(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( !_loading && !_planetLoader.IsAlive ) {
        spriteBatch.Draw(_background, MBGame.Camera.Position);

        foreach ( var p in _planets ) {
          if ( p.GetMapLayer("planet map") == null ) {
            UpdateDrawSpace(p);
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

    private void UpdateDrawSpace(Planet p)
    {
      p.CreateMapTexture(Content);
      var sign = _rand.Next(-1, 1);
      p.Position = new Vector2(
        sign * p.Meta.StarDistance.RelativeKilometers,
        sign * p.Meta.StarDistance.RelativeKilometers
      );

      var planetEntity = new Entity(GameObjects);
      var planetSprite = planetEntity.Attach<SpriteTransform>(
        p.GetMapLayer("planet map"), p.Position, new Vector2(1, 1)
      ) as SpriteTransform;
      var planet = planetEntity.Attach<PlanetComponent>() as PlanetComponent;
      planet.Data = p;
      planetEntity.Attach<CollisionComponent>(new RectangleF[] { planetSprite.Target.GetBoundingRectangle() });

      GameObjects.UpdateSystems(planetEntity);

      if ( p.Position.X > _bounds.Right ) {
        _bounds.Inflate(p.Position.X - _bounds.Right, 0);
      }
      if ( p.Position.X < _bounds.Left ) {
        _bounds.Inflate(_bounds.Right - p.Position.X, 0);
      }
      if ( p.Position.Y > _bounds.Bottom ) {
        _bounds.Inflate(0, p.Position.Y - _bounds.Bottom);
      }
      if ( p.Position.Y < _bounds.Top ) {
        _bounds.Inflate(0, _bounds.Top - p.Position.Y);
      }
      var collision = GameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
      if ( collision != null ) {
        //HACK: Hardcoded cell size
        collision.ResetGrid(_bounds.Left, _bounds.Right, _bounds.Top, _bounds.Bottom, 180);
      }
    }

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

    public override void Exit()
    {
      // End transition instantly
      TransitionState = TransitionState.Null;
    }

    public override void Pause()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    public override void Resume()
    {
      (GameObjects.GetSystem<PhysicsSystem>() as PhysicsSystem).Environment = PhysicsEnvironement.System;
      // End transition instantly
      TransitionState = TransitionState.None;
    }

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

    private void LoadPlanets()
    {
      var movement = GameObjects["player"].GetComponent<Movement>();

      var closestPlanet = _planets.OrderBy(
        planet => Vector2.Distance(movement.Position, planet.Position)
      ).Last();

      closestPlanet.Generate();

      _loading = false;

      for ( int i = 0; i < _planets.Length; i++ ) {
        if ( !_planets[i].Generated ) {
          _planets[i].Generate();
        }
      }
    }

    private void UpdateSounds(Entity player)
    {
      const float fadeSpeed = 0.05f;
      const float maxVolume = 0.5f;

      var physics = player.GetComponent<PhysicsComponent>();

      if ( physics != null && (physics.Power > 0 || physics.Power < 0) ) {
        if ( _thrusterInstance.State == SoundState.Stopped ) {
          _thrusterInstance.Play();
          _thrusterInstance.Volume = 0.0f;
        }
        if ( _thrusterInstance.Volume < maxVolume ) {
          _thrusterInstance.Volume += fadeSpeed;
        }
      } else {
        if ( _thrusterInstance.Volume > fadeSpeed ) {
          _thrusterInstance.Volume -= fadeSpeed;
        } else {
          _thrusterInstance.Stop();
        }
      }
    }

  }
}
