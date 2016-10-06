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
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
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
    private Texture2D _ship, _solarSystem, _background;
    private Song _bgSong;
    private SpriteFont _bender;
    private int _seed, _progressRemaining, _maxProgress, _currentProgress;
    private bool _loading;
    private GalaxyBuilder _galaxy;
    private GalaxyHud _hud;

    public GalaxyScene(EntityMap map) : base(map)
    {
      //TODO: Load from file here
      _seed = 100; //HACK: Hardcoded seed value for galaxy
      _loading = true;
      _currentProgress = 0;
      _maxProgress = _progressRemaining = 40;
    }

    public override void Initialize()
    {
      _galaxy = new GalaxyBuilder(Content, 2500, _seed);
      _ship = Content.Load<Texture2D>("Images/playership_blue");
      _solarSystem = Content.Load<Texture2D>("Images/starsystem");
      _background = Content.Load<Texture2D>("Images/stars");
      _bender = Content.Load<SpriteFont>("Bender");
      _bgSong = Content.Load<Song>("Audio/galaxy");
      _hud = new GalaxyHud(Content);

      BuildPlayerShip();
      BuildGalaxy();

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

      if ( MediaPlayer.GameHasControl ) {
        MediaPlayer.Play(_bgSong);
        MediaPlayer.IsRepeating = true;
      }

      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      if ( !_loading ) {
        GameObjects.GetSystem<NavigationInputSystem>().Run();
        GameObjects.GetSystem<InputSystem>().Run();
        GameObjects.GetSystem<ShipInputSystem>().Run();
      } else if ( MBGame.FPS >= 59 ) {
        _progressRemaining--;
        if ( _progressRemaining <= 0 ) {
          _loading = false;
        }
      }
    }

    public override void Update()
    {
      GameObjects.GetSystem<CollisionSystem>().Run();
      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();
      GameObjects.GetSystem<GalaxySystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();
      _hud.Update();
      var inventory = GameObjects["player"].GetComponent<Inventory>();
      if ( inventory != null && !_loading ) {
        _hud.Refresh(inventory);
      }
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( _loading ) {
        var center = MBGame.Camera.GetBoundingRectangle().Center;
        var start = (center.X - ((_maxProgress / 2) * (_solarSystem.Width * 0.5f)));
        _currentProgress++;
        spriteBatch.DrawString(
          _bender,
          "Loading...",
          center,
          Color.White
        );
        for ( int i = 0; i < _currentProgress / 2; i++ ) {
          spriteBatch.Draw(
            _solarSystem,
            new Vector2(start + (i * _solarSystem.Width * 0.5f), center.Y),
            scale: new Vector2(0.5f, 0.5f)
          );
        }
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
      var inventory = player.Attach<Inventory>() as Inventory;
      inventory.Items.Add(typeof(Fuel), new Fuel(1000));
      player.Attach<Movement>(30.0f, 0.05f);

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
      }
    }

  }
}
