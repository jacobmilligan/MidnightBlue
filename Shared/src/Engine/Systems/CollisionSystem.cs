//
// 	CollisionSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
namespace MidnightBlue.Engine.EntityComponent
{
  public class CollisionSystem : EntitySystem
  {
    public CollisionSystem() : base(typeof(Collision), typeof(SpriteComponent))
    {
    }

    protected override void Process(Entity entity)
    {
      var collision = entity.GetComponent<Collision>();
      var sprite = entity.GetComponent<SpriteComponent>();

      if ( collision != null ) {
        var boxAmt = collision.Boxes.Count;
        for ( int i = 0; i < boxAmt; i++ ) {
          if ( sprite != null ) {
            var box = collision.Boxes[i];
            var spriteRect = sprite.Target.GetBoundingRectangle();
            box.X = (int)(spriteRect.Left);
            box.Y = (int)(spriteRect.Top);
            box.Width += (int)(spriteRect.Width - box.Width);
            box.Height += (int)(spriteRect.Height - box.Height);
            collision.Boxes[i] = box;
          }
        }
      }
    }
  }
}
