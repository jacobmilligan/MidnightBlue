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
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.Collision;

namespace MidnightBlue.Engine.EntityComponent
{
  public class CollisionSystem : EntitySystem
  {
    private CollisionMap _map;
    private int _comparisons;

    public CollisionSystem() : base(typeof(CollisionComponent), typeof(SpriteComponent))
    {
      ResetGrid(0, 0, 0, 0, 1);
    }

    public void ResetGrid(int xMin, int xMax, int yMin, int yMax, int cellSize)
    {
      _map = new CollisionMap(xMin, xMax, yMin, yMax, cellSize);
    }

    protected override void PreProcess()
    {
      _comparisons = 0;

      _map.Clear();

      var maxEntities = AssociatedEntities.Count;

      for ( int e = 0; e < maxEntities; e++ ) {
        var collision = AssociatedEntities[e].GetComponent<CollisionComponent>();

        if ( collision != null ) {
          collision.Event = false;
          var numBoxes = collision.Boxes.Count;
          for ( int box = 0; box < numBoxes; box++ ) {
            _map.Insert(collision.Boxes[box]);
          }
        }
      }
    }

    protected override void Process(Entity entity)
    {
      var collision = entity.GetComponent<CollisionComponent>();
      var sprite = entity.GetComponent<SpriteComponent>();
      var center = Vector2.Zero;

      if ( sprite != null ) {
        center = sprite.Target.Origin;
      }

      if ( collision != null ) {
        var boxCount = collision.Boxes.Count;
        for ( int b = 0; b < boxCount; b++ ) {
          var box = collision.Boxes[b];

          // Move the box to align with the players new position
          if ( sprite != null ) {
            box.X += sprite.DeltaPosition.X;
            box.Y += sprite.DeltaPosition.Y;
            box.Width += sprite.DeltaSize.X;
            box.Height += sprite.DeltaSize.Y;
            collision.Boxes[b] = box;
          }

          var neighbours = _map.GetCollisions(box);
          var neighbourCount = neighbours.Count;
          for ( int n = 0; n < neighbourCount; n++ ) {
            if ( box.Intersects(neighbours[n]) ) {
              collision.Event = true;
            }
          }
          _comparisons++;
        }
      }
    }

    protected override void PostProcess()
    {
      //TODO: Make console var
      //MBGame.Console.Write("Collision comparisons: {0}", _comparisons);
    }

    public CollisionMap CurrentMap
    {
      get { return _map; }
    }

  }
}
