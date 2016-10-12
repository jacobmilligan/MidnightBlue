//
// 	PlanetScene.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 12/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  public class PlanetScene : Scene
  {
    Planet _planet;

    public PlanetScene(EntityMap map, ContentManager content, Planet planet) : base(map, content)
    {
      _planet = planet;
      map.Clear();

      var collision = map.GetSystem<CollisionSystem>() as CollisionSystem;
      if ( collision != null ) {
        var width = _planet.Tiles.GetLength(0);
        var height = _planet.Tiles.GetLength(1);
        collision.ResetGrid(0, width, 0, height, 108);
      }
    }

    public override void Initialize()
    {
      var physicsSystem = GameObjects.GetSystem<PhysicsSystem>() as PhysicsSystem;
      physicsSystem.Environment = PhysicsEnvironement.Planet;

      var player = GameObjects["player"];
      var movement = player.GetComponent<Movement>();
      var physics = player.GetComponent<PhysicsComponent>();
      movement.Position = new Vector2(0, 0);
      movement.Speed = 350;
      physics.Velocity = new Vector2(0, 0);

      player.Detach<ShipController>();
      player.Attach<PlayerController>();

      // End transition instantly
      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
      GameObjects.GetSystem<CollisionSystem>().Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();
      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();

      HandleWrapping();
    }

    private void HandleWrapping()
    {
      var movement = GameObjects["player"].GetComponent<Movement>();
      if ( movement.Position.X < 0 ) {
        movement.Position = new Vector2(_planet.Size.X * 32, movement.Position.Y);
      }
      if ( movement.Position.X > _planet.Size.X * 32 ) {
        movement.Position = new Vector2(0, movement.Position.Y);
      }
      if ( movement.Position.Y > _planet.Size.Y * 32 ) {
        movement.Position = new Vector2(movement.Position.X, 0);
      }
      if ( movement.Position.Y < 0 ) {
        movement.Position = new Vector2(movement.Position.X, _planet.Size.Y * 32);
      }
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      var cameraBounds = MBGame.Camera.GetBoundingRectangle();
      var startX = (int)(-cameraBounds.Location.X / 32);
      var startY = (int)(-cameraBounds.Location.Y / 32);
      var endX = startX + (cameraBounds.Width / 32);
      var endY = startY + (cameraBounds.Height / 32);

      for ( int x = 0; x < _planet.Size.X; x++ ) {
        for ( int y = 0; y < _planet.Size.X; y++ ) {
          var drawPos = MBMath.WrapGrid((int)(x + cameraBounds.Width), (int)(y + cameraBounds.Height), _planet.Size.X, _planet.Size.Y);
          var viewRect = new RectangleF(x * 32, y * 32, 32, 32);
          if ( cameraBounds.Contains(viewRect) ) {
            spriteBatch.FillRectangle(
              viewRect,
              _planet.GetColor(_planet.Tiles[drawPos.X, drawPos.Y].Biome)
            );
          } else {

          }
        }
      }

      GameObjects.GetSystem<RenderSystem>().Run();
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

  }
}
