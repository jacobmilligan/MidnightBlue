//
// 	CollisionMap.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine.Collision
{
  public class CollisionMap
  {
    private CollisionCell[,] _cells;
    private int _cellSize;

    public CollisionMap(int width, int height, int cellSize)
    {
      if ( cellSize > 0 ) {
        if ( width > 0 ) {
          width /= cellSize;
        }
        if ( height > 0 ) {
          height /= cellSize;
        }
      }
      _cells = new CollisionCell[width, height];
      _cellSize = cellSize;
    }

    public Point IndexOf(Vector2 position)
    {
      return new Point {
        X = (int)(position.X / _cellSize),
        Y = (int)(position.Y / _cellSize)
      };
    }

    public void Insert(Rectangle bounds, List<CollisionCell> cellList)
    {
      var sides = GetSides(bounds);

      foreach ( var side in sides ) {
        var index = IndexOf(side);
        var cell = _cells[index.X, index.Y];
        if ( cell == null ) {
          _cells[index.X, index.Y] = new CollisionCell();
          cell = _cells[index.X, index.Y];
        }
        if ( !cell.Contains(bounds) ) {
          cell.Add(bounds);
        }
        if ( !cellList.Contains(cell) ) {
          cellList.Add(cell);
        }
      }
    }

    public List<Rectangle> GetCollisions(Rectangle bounds)
    {
      var result = new List<Rectangle>();
      var sides = GetSides(bounds);

      foreach ( var side in sides ) {
        var index = IndexOf(side);
        var cell = _cells[index.X, index.Y];
        if ( cell != null ) {
          foreach ( var r in cell.Items ) {
            if ( !result.Contains(r) && r.Center != bounds.Center ) {
              result.Add(r);
            }
          }
        }
      }

      return result;
    }

    public void Clear()
    {
      foreach ( var c in _cells ) {
        if ( c != null && c.Items.Count > 0 ) {
          c.Items.Clear();
        }
      }
    }

    private Vector2[] GetSides(Rectangle bounds)
    {
      return new Vector2[8]
      {
        // Top left
        new Vector2(bounds.Left, bounds.Top),
        // Top middle
        new Vector2(bounds.Center.X, bounds.Top),
        // Top right
        new Vector2(bounds.Right, bounds.Top),
        // Right middle
        new Vector2(bounds.Right, bounds.Center.Y),
        // Bottom right
        new Vector2(bounds.Right, bounds.Bottom),
        // Bottom middle
        new Vector2(bounds.Center.X, bounds.Bottom),
        // Bottom left
        new Vector2(bounds.Left, bounds.Bottom),
        // Left middle
        new Vector2(bounds.Left, bounds.Center.Y)
      };
    }
  }
}
