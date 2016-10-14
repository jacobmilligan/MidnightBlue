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
using MidnightBlue.Engine.Tiles;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.EntityComponent
{
  public class CollisionSystem : EntitySystem
  {
    private CollisionMap _map;

    private TileMap _tileMap;

    private int _comparisons;

    public CollisionSystem() : base(typeof(CollisionComponent))
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
        var entity = AssociatedEntities[e];
        var movement = entity.GetComponent<Movement>();
        if ( movement != null ) {
          Process(AssociatedEntities[e]);
        }
      }
    }

    protected override void Process(Entity entity)
    {
      var collision = entity.GetComponent<CollisionComponent>();
      var sprite = entity.GetComponent<SpriteTransform>();

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
            box.Width += sprite.Bounds.Width - box.Width;
            box.Height += sprite.Bounds.Height - box.Height;
            box.X += sprite.Target.Position.X - box.X - (sprite.Bounds.Width / 2);
            box.Y += sprite.Target.Position.Y - box.Y - (sprite.Bounds.Height / 2);

            collision.Boxes[b] = box;
          }

          foreach ( var n in neighbours ) {
            var neighbourCollision = n.GetComponent<CollisionComponent>();
            collision.Event = HandleCollisions(box, neighbourCollision);
            if ( collision.Event ) {
              collision.Collider = n;
              neighbourCollision.Event = true;
              neighbourCollision.Collider = entity;
              Console.WriteLine("collision");
            }
          }

          if ( _tileMap != null ) {
            //HandleTileCollision(entity, box.X, box.Y);
            //HandleTileCollision(entity, box.X + box.Width, box.Y);
            //HandleTileCollision(entity, box.X + box.Width, box.Y + box.Height);
            //HandleTileCollision(entity, box.X, box.Y + box.Height);
            var physics = entity.GetComponent<PhysicsComponent>();
            var movement = entity.GetComponent<Movement>();
            if ( physics != null && movement != null ) {

              var top = (int)Math.Floor(box.Top / _tileMap.TileSize.Y);
              var left = (int)Math.Floor(box.Left / _tileMap.TileSize.X);
              var right = Math.Ceiling(box.Right / _tileMap.TileSize.X) - 1;
              var bottom = Math.Ceiling(box.Bottom / _tileMap.TileSize.X) - 1;

              int xSide = 1;
              int ySide = 1;

              for ( int x = left; x <= right; x++ ) {
                for ( int y = top; y <= bottom; y++ ) {
                  var tileRect = new Rectangle(
                    x * _tileMap.TileSize.X,
                    y * _tileMap.TileSize.Y,
                    _tileMap.TileSize.X,
                    _tileMap.TileSize.Y
                  );
                  if ( _tileMap[x, y].Flag == TileFlag.Impassable && box.Intersects(tileRect) ) {
                    physics.Velocity = new Vector2(xSide * 5, ySide * 5);
                    movement.Position = movement.LastPosition;
                  }
                  ySide -= 2;
                }
                xSide -= 2;
              }
            }
          }
        }

      }
    }

    private void HandleTileCollision(Entity entity, float x, float y)
    {
      var physics = entity.GetComponent<PhysicsComponent>();
      if ( physics != null ) {
        x += physics.Velocity.X * MBGame.DeltaTime;
        y += physics.Velocity.Y * MBGame.DeltaTime;

        var tilePos = new Point(
          (int)(x / _tileMap.TileSize.X),
          (int)(y / _tileMap.TileSize.Y)
        );

        if ( _tileMap[tilePos.X, tilePos.Y].Flag == TileFlag.Impassable ) {
          physics.Velocity = -physics.Velocity;
          physics.Power = 0;
          physics.Acceleration = new Vector2(0, 0);
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

    public void SetTileMap(TileMap tileMap)
    {
      _tileMap = tileMap;
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
