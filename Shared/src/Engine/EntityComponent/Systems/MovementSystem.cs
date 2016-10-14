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
    public MovementSystem() : base(typeof(Movement), typeof(SpriteTransform)) { }

    protected override void Process(Entity entity)
    {
      var movement = entity.GetComponent<Movement>();
      var sprite = entity.GetComponent<SpriteTransform>();

      if ( movement != null && sprite != null ) {
        sprite.Target.Position = movement.Position;

        sprite.Rotation = movement.Angle;
        sprite.Bounds = sprite.Target.GetBoundingRectangle();

        movement.Heading = sprite.Direction;
      }
    }
  }
}
