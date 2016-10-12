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
    private Sprite _starSprite;
    private List<Texture2D> _loadingTextures;
    private SoundEffect _thrusterSound;
    private SoundEffectInstance _thrusterInstance;
    private StarSystem _starSystem;
    private Planet[] _planets;
    private Thread _planetLoader;
    private SpriteFont _bender;
    private StarSystemHud _hud;

    public StarSystemScene(
      EntityMap map, ContentManager content, StarSystem starSystem, Dictionary<string, Planet> cache, int seed
    ) : base(map, content)
    {
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
      _starSprite = new Sprite(_star);

      var player = GameObjects["player"];
      var movement = player.GetComponent<Movement>();
      movement.Speed = 25.0f;
      movement.RotationSpeed = 0.05f;

      var maxPlanetDistance = 1000;
      if ( _starSystem.Planets.Count > 0 ) {
        maxPlanetDistance = (int)(_starSystem.Planets.Max(meta => meta.StarDistance).Kilometers / 100000);
      }
      _starSprite.Position = new Vector2(
        movement.Position.X - maxPlanetDistance,
        movement.Position.Y
      );
      _starSprite.Scale = new Vector2(2, 2);

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

      _planetLoader = new Thread(new ThreadStart(LoadPlanets));
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
      GameObjects.GetSystem<NavigationInputSystem>().Run();
      GameObjects.GetSystem<InputSystem>().Run();
      var shipInput = GameObjects.GetSystem<ShipInputSystem>() as ShipInputSystem;
      shipInput.Run();
    }

    public override void Update()
    {
      GameObjects.GetSystem<CollisionSystem>().Run();

      UpdateSounds(GameObjects["player"]);

      GameObjects.GetSystem<PhysicsSystem>().Run();
      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();

      _hud.Update();
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      spriteBatch.Draw(_background, MBGame.Camera.Position);

      if ( _loading ) {
        var loadingTexture = _loadingTextures[_animFrame];

        var viewRect = MBGame.Graphics.Viewport.Bounds;
        var pos = new Vector2(viewRect.Width - loadingTexture.Width, viewRect.Height - loadingTexture.Height);

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

      if ( !_loading && !_planetLoader.IsAlive ) {
        foreach ( var p in _planets ) {
          if ( p.GetMapLayer("planet map") == null ) {
            p.CreateMapTexture(uiSpriteBatch, Content);
            p.Position = MBGame.Camera.Position;
          }
          spriteBatch.Draw(p.GetMapLayer("planet map"), p.Position);
        }
      }

      spriteBatch.Draw(_starSprite);
      GameObjects.GetSystem<RenderSystem>().Run();

      _hud.Draw(spriteBatch, uiSpriteBatch);
      DrawMap(uiSpriteBatch);
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

      normalizedPos = (_starSprite.Position / hudBounds.Width);
      uiSpriteBatch.FillRectangle(
        hudBounds.Center.ToVector2() + normalizedPos,
        new Vector2(3, 3),
        Color.Red
      );

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
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    private void LoadPlanets()
    {
      for ( int i = 0; i < _planets.Length; i++ ) {
        _planets[i].Generate();
      }
      _loading = false;
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
