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
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Shapes;
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
      //TODO: Load from file here
      _seed = 10; //HACK: Hardcoded seed value for galaxy
    }

    public override void Initialize()
    {
      _galaxy = new GalaxyBuilder(Content, 1000, _seed);
      _ship = Content.Load<Texture2D>("Images/playership_blue");
      _solarSystem = Content.Load<Texture2D>("Images/starsystem");

      BuildGalaxy();
      BuildPlayerShip();

      var physics = GameObjects.GetSystem<PhysicsSystem>();
      if ( physics != null ) {
        (physics as PhysicsSystem).Environment = PhysicsEnvironement.Galaxy;
      }

      var collision = GameObjects.GetSystem<CollisionSystem>();
      if ( collision != null ) {
        ((CollisionSystem)collision).ResetGrid(
          _galaxy.Bounds.Left,
          _galaxy.Bounds.Right,
          _galaxy.Bounds.Top,
          _galaxy.Bounds.Bottom,
          90
        ); //HACK: Hardcoded collision cell size
      }

      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<NavigationInputSystem>().Run();
      GameObjects.GetSystem<InputSystem>().Run();
      GameObjects.GetSystem<ShipInputSystem>().Run();
    }

    public override void Update()
    {
      GameObjects.GetSystem<CollisionSystem>().Run();
      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();
      GameObjects.GetSystem<GalaxySystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      GameObjects.GetSystem<RenderSystem>().Run();
      GameObjects.GetSystem<GalaxyRenderSystem>().Run();
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
      var player = GameObjects["player"];

      var sprite = player.Attach<SpriteComponent>(
        _ship,
        new Vector2(MBGame.Camera.Position.X + 100, MBGame.Camera.Position.Y + 100),
        new Vector2(0.3f, 0.3f)
      ) as SpriteComponent;
      player.Attach<CollisionComponent>(new RectangleF[] { sprite.Target.GetBoundingRectangle() });
      player.GetComponent<SpriteComponent>().Z = 1;
      player.Attach<ShipController>();
      player.Attach<Depth>();
      player.Attach<Movement>(30.0f, 0.05f);

      GameObjects.UpdateSystems(player);
    }

    private void BuildGalaxy()
    {
      var systems = _galaxy.Generate(3000);
      var scale = 0.5f;
      foreach ( var s in systems ) {

        var newSystem = new Entity(GameObjects);
        var sprite = newSystem.Attach<SpriteComponent>(
          _solarSystem,
          new Vector2(s.Bounds.X, s.Bounds.Y),
          new Vector2(scale, scale)
        ) as SpriteComponent;
        var sysComponent = newSystem.Attach<StarSystemComponent>() as StarSystemComponent;
        sysComponent.Name = s.Name;
        sprite.Target.Color = s.Color;
        newSystem.Attach<CollisionComponent>(new RectangleF[] {
          sprite.Target.GetBoundingRectangle()
        });

      }
    }

  }
}
