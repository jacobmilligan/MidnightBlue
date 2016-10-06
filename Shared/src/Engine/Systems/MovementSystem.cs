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
    private RectangleF _cameraRect;
    private List<Entity> _visibleEntities;

    public MovementSystem() : base(typeof(Movement), typeof(SpriteComponent))
    {
      _visibleEntities = new List<Entity>();
    }

    protected override void PreProcess()
    {
      _cameraRect = MBGame.Camera.GetBoundingRectangle();
    }

    protected override void Process(Entity entity)
    {
      var movement = entity.GetComponent<Movement>();
      var sprite = entity.GetComponent<SpriteComponent>();
      if ( movement != null && sprite != null ) {
        var lastPos = sprite.Target.Position;
        var lastSize = sprite.Bounds.Size;

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

        sprite.Bounds = sprite.Target.GetBoundingRectangle();

        sprite.DeltaSize = sprite.Bounds.Size - lastSize;
        sprite.DeltaPosition = sprite.Target.Position - lastPos;
      }

      if ( sprite != null && _cameraRect.Intersects((Rectangle)sprite.Bounds) ) {
        if ( !_visibleEntities.Contains(entity) ) {
          _visibleEntities.Add(entity);
        }
      }
    }

    public List<Entity> VisibleList
    {
      get
      {
        return _visibleEntities;
      }
    }
  }
}
