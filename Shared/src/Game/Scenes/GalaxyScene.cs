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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Sprites;

namespace MidnightBlue.Engine
{
  public class GalaxyScene : Scene
  {
    private Texture2D _ship, _solarSystem;
    private int _seed;
    private GalaxyBuilder _galaxy;

    public GalaxyScene(EntityMap map) : base(map)
    {
      _seed = 0;
    }

    public override void Initialize()
    {
      _galaxy = new GalaxyBuilder(Content, 1000);
      _ship = Content.Load<Texture2D>("Images/playership_blue");
      _solarSystem = Content.Load<Texture2D>("Images/starsystem");

      BuildSystem();

      BuildPlayerShip();

      var physics = GameObjects.GetSystem<PhysicsSystem>();
      if ( physics != null ) {
        (physics as PhysicsSystem).Environment = PhysicsEnvironement.Galaxy;
      }


      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      GameObjects.GetSystem<RenderSystem>().Run();
    }

    public override void Exit()
    {
      TransitionState = TransitionState.Null;
    }

    public override void Pause()
    {
      TransitionState = TransitionState.None;
    }

    public override void Resume()
    {
      TransitionState = TransitionState.None;
    }

    private void BuildPlayerShip()
    {
      var player = new Entity(GameObjects, "player") {
        Persistant = true
      };

      player.Attach<SpriteComponent>(
        _ship,
        new Vector2(MBGame.Graphics.Viewport.X + 100, MBGame.Graphics.Viewport.Y + 100),
        new Vector2(0.3f, 0.3f)
      );
      player.Attach<ShipController>();
      player.Attach<Movement>(30.0f, 0.05f);
    }

    private void BuildSystem()
    {
      var systems = _galaxy.Generate(5000);
      foreach ( var s in systems ) {
        var newSystem = new Entity(GameObjects);
        newSystem.Attach<SpriteComponent>(
          _solarSystem,
          new Vector2(s.Bounds.X, s.Bounds.Y),
          new Vector2(0.5f, 0.5f)
        );
      }
    }

  }
}
