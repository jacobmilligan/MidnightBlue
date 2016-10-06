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
    private MovementSystem _movement;

    public GalaxyScene(EntityMap map) : base(map)
    {
      //TODO: Load from file here
      _seed = 10; //HACK: Hardcoded seed value for galaxy
    }

    public override void Initialize()
    {
      _galaxy = new GalaxyBuilder(Content, 2500, _seed);
      _ship = Content.Load<Texture2D>("Images/playership_blue");
      _solarSystem = Content.Load<Texture2D>("Images/starsystem");

      BuildGalaxy();
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
      _movement = GameObjects.GetSystem<MovementSystem>() as MovementSystem;
      _movement.Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();
      GameObjects.GetSystem<GalaxySystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      var render = GameObjects.GetSystem<RenderSystem>();
      render.AssociatedEntities = _movement.VisibleList;
      render.Run();
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
      sprite.Z = 1;
      player.Attach<CollisionComponent>(new RectangleF[] { sprite.Target.GetBoundingRectangle() });
      player.Attach<ShipController>();
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
