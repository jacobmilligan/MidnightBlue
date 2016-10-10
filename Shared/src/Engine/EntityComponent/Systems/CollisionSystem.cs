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
using MonoGame.Extended.Shapes;

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
          collision.Collider = null;
          _map.Insert(AssociatedEntities[e], collision);
        }
      }
    }

    protected override void ProcessingLoop()
    {
      var entityCount = AssociatedEntities.Count;
      for ( int e = 0; e < entityCount; e++ ) {
        var movement = AssociatedEntities[e].GetComponent<Movement>();
        if ( movement != null ) {
          Process(AssociatedEntities[e]);
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
        var neighbours = _map.GetCollisions(entity, collision);

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

          foreach ( var n in neighbours ) {
            var neighbourCollision = n.GetComponent<CollisionComponent>();
            collision.Event = HandleCollisions(box, neighbourCollision);
            if ( collision.Event ) {
              collision.Collider = n;
              neighbourCollision.Event = true;
              neighbourCollision.Collider = entity;
            }
          }
        }

      }
    }

    /// <summary>
    /// Handles collisions between a single Collision box and each of another
    /// entitys collision boxes
    /// </summary>
    /// <returns><c>true</c>, if a collision ocurred, <c>false</c> otherwise.</returns>
    /// <param name="box">Collision box.</param>
    /// <param name="neighbourCollision">Neighbours collision boxes to check against.</param>
    private bool HandleCollisions(RectangleF box, CollisionComponent neighbourCollision)
    {
      var hasCollision = false;
      var neighbourBoxes = neighbourCollision.Boxes;
      for ( int n = 0; n < neighbourBoxes.Count; n++ ) {
        if ( box.Intersects(neighbourBoxes[n]) ) {
          hasCollision = true;
        }
        _comparisons++;
      }
      return hasCollision;
    }

    public CollisionMap CurrentMap
    {
      get { return _map; }
    }

    public int NumberOfChecks
    {
      get { return _comparisons; }
    }


  }
}
