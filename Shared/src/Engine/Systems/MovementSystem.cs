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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

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
        var lastPos = sprite.Target.Position;
        var lastSize = sprite.Bounds.Size;

        sprite.Target.Position = movement.Position;

        sprite.Rotation = movement.Angle;
        sprite.Bounds = sprite.Target.GetBoundingRectangle();

        sprite.DeltaSize = sprite.Bounds.Size - lastSize;
        sprite.DeltaPosition = sprite.Target.Position - lastPos;
        movement.Heading = sprite.Direction;
      }
    }
  }
}
