//
// 	MovementSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
namespace MidnightBlue.Engine.EntityComponent
{
  public class MovementSystem : EntitySystem
  {
    public MovementSystem() : base(typeof(Movement), typeof(SpriteComponent)) { }

    protected override void Process(Entity entity)
    {
      var movement = entity.GetComponent<Movement>();
      var sprite = entity.GetComponent<SpriteComponent>();
      if ( movement != null && sprite != null ) {

        if ( entity.Tag == "player" ) {
          //MBGame.Graphics.Viewport.
        }

        sprite.Target.Position += movement.Velocity * MBGame.DeltaTime;

        switch ( movement.RotationDirection ) {
          case RotationDirection.Right:
            sprite.Rotation += movement.RotationAcceleration;
            break;
          case RotationDirection.Left:
            sprite.Rotation -= movement.RotationAcceleration;
            break;
          default:
            sprite.Rotation += 0.0f;
            break;
        }
      }
    }
  }
}
