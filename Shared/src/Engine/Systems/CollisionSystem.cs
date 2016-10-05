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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.Collision;

namespace MidnightBlue.Engine.EntityComponent
{
  public class CollisionSystem : EntitySystem
  {
    private CollisionMap _grid;

    public CollisionSystem() : base(typeof(CollisionComponent), typeof(SpriteComponent))
    {
      ResetGrid(0, 0, 0);
    }

    public void ResetGrid(int width, int height, int cellSize)
    {
      _grid = new CollisionMap(width, height, cellSize);
    }

    protected override void Process(Entity entity)
    {
      var collision = entity.GetComponent<CollisionComponent>();
      var sprite = entity.GetComponent<SpriteComponent>();

      if ( collision != null ) {
        collision.Event = false;

        if ( collision.ContainingCells.Count > 0 ) {
          foreach ( var c in collision.ContainingCells ) {
            c.Items.Clear();
          }
        }

        var boxAmt = collision.Boxes.Count;

        for ( int i = 0; i < boxAmt; i++ ) {
          var box = collision.Boxes[i];

          _grid.Insert(box, collision.ContainingCells);

          List<Rectangle> neighbours = null;

          if ( entity.Tag == "player" ) {
            neighbours = _grid.GetCollisions(box);
          } else {
            neighbours = _grid.GetCollisions(box);
          }

          foreach ( var n in neighbours ) {
            if ( box.Intersects(n) ) {
              collision.Event = true;
            }
          }

          if ( sprite != null ) {
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
