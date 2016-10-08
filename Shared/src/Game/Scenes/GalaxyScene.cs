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
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace MidnightBlue.Engine
{
  public class GalaxyScene : Scene
  {
    private int _seed, _animFrame, _animTime;
    private bool _loading;
    private SpriteFont _bender, _benderLarge;
    private Texture2D _ship, _solarSystem, _background;
    private Song _bgSong;
    private SoundEffect _thrusterSound;
    private SoundEffectInstance _thrusterInstance;
    private List<Texture2D> _loadingTextures;
    private GalaxyBuilder _galaxy;
    private GalaxyHud _hud;
    private Thread _galaxyBuildThread;

    public GalaxyScene(EntityMap map, ContentManager content) : base(map, content)
    {
      //TODO: Load from file here
      _seed = 1005; //HACK: Hardcoded seed value for galaxy
      _loading = true;
      _animTime = _animFrame = 0;

      _ship = content.Load<Texture2D>("Images/playership_blue");
      _solarSystem = content.Load<Texture2D>("Images/starsystem");
      _background = content.Load<Texture2D>("Images/stars");
      _bender = content.Load<SpriteFont>("Fonts/Bender");
      _benderLarge = content.Load<SpriteFont>("Fonts/Bender Large");

      _bgSong = content.Load<Song>("Audio/galaxy");
      _thrusterSound = content.Load<SoundEffect>("Audio/engine");
      _thrusterInstance = _thrusterSound.CreateInstance();
      _thrusterInstance.IsLooped = true;

      _loadingTextures = new List<Texture2D>();
      var animDir = content.RootDirectory + "/Images/loading_galaxy";
      var files = Directory.GetFiles(animDir);
      for ( int i = 0; i < files.Length; i++ ) {
        var fileName = files[i].Replace(".xnb", "").Replace("Content/", "");
        _loadingTextures.Add(
          content.Load<Texture2D>(fileName)
        );
      }
      _hud = new GalaxyHud(content);
      _galaxyBuildThread = new Thread(new ThreadStart(BuildGalaxy));
    }

    public override void Initialize()
    {
      if ( MediaPlayer.GameHasControl ) {
        MediaPlayer.Play(_bgSong);
        MediaPlayer.IsRepeating = true;
      }

      if ( _galaxy == null ) {
        _galaxy = new GalaxyBuilder(Content, 4000, _seed);
        _galaxyBuildThread.Start();
        BuildPlayerShip();

        var physics = GameObjects.GetSystem<PhysicsSystem>();
        if ( physics != null ) {
          (physics as PhysicsSystem).Environment = PhysicsEnvironement.Galaxy;
        }

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

      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      if ( !_loading ) {
        GameObjects.GetSystem<NavigationInputSystem>().Run();
        GameObjects.GetSystem<InputSystem>().Run();
        GameObjects.GetSystem<ShipInputSystem>().Run();
      }
    }

    public override void Update()
    {
      var inventory = GameObjects["player"].GetComponent<Inventory>();

      if ( inventory != null && !_loading ) {
        _hud.Update();

        GameObjects.GetSystem<CollisionSystem>().Run();

        UpdateSounds(GameObjects["player"]);

        GameObjects.GetSystem<PhysicsSystem>().Run();
        GameObjects.GetSystem<MovementSystem>().Run();
        GameObjects.GetSystem<DepthSystem>().Run();

        _hud.Refresh(inventory);
      }
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( _loading ) {
        AnimateLoading(spriteBatch);
      } else {
        spriteBatch.Draw(_background, MBGame.Camera.Position);

        GameObjects.GetSystem<RenderSystem>().Run();
        GameObjects.GetSystem<GalaxyRenderSystem>().Run();
        _hud.Draw(uiSpriteBatch);
      }
    }

    public override void Exit()
    {
      TransitionState = TransitionState.Null;
    }

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

    public override void Resume()
    {
      TransitionState = TransitionState.None;
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

    private void AnimateLoading(SpriteBatch spriteBatch)
    {
      var center = MBGame.Camera.GetBoundingRectangle().Center;
      var loadingStr = "PLEASE BE PATIENT WHILE YOUR UNIVERSE IS CREATED...";
      var measureStr = _benderLarge.MeasureString(loadingStr);
      center.Y -= 200;
      center.X -= measureStr.X / 2;

      spriteBatch.DrawString(
        _benderLarge,
        loadingStr,
        center,
        Color.White
      );

      var loadingTexture = _loadingTextures[_animFrame];

      center = MBGame.Camera.GetBoundingRectangle().Center;
      center.X -= loadingTexture.Width / 2;
      center.Y -= loadingTexture.Height / 2;

      spriteBatch.Draw(_loadingTextures[_animFrame], center);

      _animTime++;

      if ( _animTime > 2 ) {
        _animTime = 0;
        _animFrame++;
        if ( _animFrame > _loadingTextures.Count - 1 ) {
          _animFrame = 0;
        }
      }
    }

    private void BuildPlayerShip()
    {
      var player = GameObjects["player"];

      var sprite = player.Attach<SpriteComponent>(
        _ship,
        new Vector2(MBGame.Camera.Position.X + 100, MBGame.Camera.Position.Y + 100),
        new Vector2(0.3f, 0.3f)
      ) as SpriteComponent;
      sprite.Z = 1;
      player.Attach<CollisionComponent>(new RectangleF[] { sprite.Target.GetBoundingRectangle() });
      player.Attach<ShipController>();
      player.Attach<PhysicsComponent>();
      var inventory = player.Attach<Inventory>() as Inventory;
      inventory.Items.Add(typeof(Fuel), new Fuel(10000));
      player.Attach<Movement>(1.0f, 0.02f);

      MBGame.Camera.LookAt(sprite.Target.Origin);
      GameObjects.UpdateSystems(player);
    }

    private void BuildGalaxy()
    {
      var systems = _galaxy.Generate(_galaxy.Size * 3);
      var scale = 0.5f;
      foreach ( var s in systems ) {

        var newSystem = new Entity(GameObjects);
        var sprite = newSystem.Attach<SpriteComponent>(
          _solarSystem,
          new Vector2(s.Bounds.X, s.Bounds.Y),
          new Vector2(scale, scale)
        ) as SpriteComponent;
        newSystem.Attach<CollisionComponent>(new RectangleF[] {
          sprite.Target.GetBoundingRectangle()
        });
        sprite.Target.Color = s.Color;
        newSystem.Attach(s);

        GameObjects.UpdateSystems(newSystem);
      }

      _loading = false;
    }

  }
}
