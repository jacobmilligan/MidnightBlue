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
using MidnightBlue.Engine.Geometry;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.Collision
{
  public class CollisionMap
  {
    private CollisionCell[,] _cells;
    private HashSet<RectangleF> _checkedRects;
    private List<CollisionCell> _nonEmptyCells;
    private int _cellSize;
    private Grid _grid;
    private Vector2 _min;
    private Vector2 _max;

    public CollisionMap(int xMin, int xMax, int yMin, int yMax, int cellSize)
    {
      var width = (xMax - xMin) / cellSize;
      var height = (yMax - yMin) / cellSize;

      _cells = new CollisionCell[width, height];
      _grid = new Grid(height, width, cellSize, cellSize);

      _min = new Vector2(xMin, yMin);
      _max = new Vector2(xMax, yMax);

      _cellSize = cellSize;

      _checkedRects = new HashSet<RectangleF>();
      _nonEmptyCells = new List<CollisionCell>();
    }

    public Point IndexOf(Vector2 position)
    {
      return new Point {
        X = (int)((position.X - _min.X) / _cellSize) - 1,
        Y = (int)((position.Y - _min.Y) / _cellSize) - 1
      };
    }

    public bool IndexExists(int x, int y)
    {
      var result = false;
      var xSize = _cells.GetLength(0);
      var ySize = _cells.GetLength(1);
      if ( x >= 0 && x < xSize && y >= 0 && y < ySize ) {
        result = true;
      }
      return result;
    }

    public bool IndexExists(Point index)
    {
      return IndexExists(index.X, index.Y);
    }

    public void Insert(RectangleF bounds)
    {
      var corners = GetCorners(bounds);

      foreach ( var corner in corners ) {
        var index = IndexOf(corner);
        if ( IndexExists(index) ) {
          var cell = _cells[index.X, index.Y];
          if ( cell == null ) {
            _cells[index.X, index.Y] = new CollisionCell();
            cell = _cells[index.X, index.Y];
          }
          if ( !cell.Contains(bounds) ) {
            cell.Add(bounds);
            _nonEmptyCells.Add(cell);
          }
        }
      }
    }

    public List<RectangleF> GetCollisions(RectangleF bounds)
    {
      var result = new List<RectangleF>();
      var corners = GetCorners(bounds);

      _checkedRects.Clear();

      foreach ( var corner in corners ) {
        var index = IndexOf(corner);
        if ( IndexExists(index) ) {
          var cell = _cells[index.X, index.Y];
          if ( cell != null ) {
            var maxItems = cell.Items.Count;
            foreach ( var i in cell.Items ) {
              if ( !_checkedRects.Contains(i) && i.Center != bounds.Center ) {
                result.Add(i);
                _checkedRects.Add(i);
              }
            }
          }
        }
      }

      return result;
    }

    public void Clear()
    {
      var nonEmptySize = _nonEmptyCells.Count;
      for ( int c = 0; c < nonEmptySize; c++ ) {
        _nonEmptyCells[c].Clear();
      }
      _nonEmptyCells.Clear();
    }

    private Vector2[] GetCorners(RectangleF bounds)
    {
      return new Vector2[4]
      {
        // Top left
        new Vector2(bounds.Left, bounds.Top),
        // Top right
        new Vector2(bounds.Right, bounds.Top),
        // Bottom right
        new Vector2(bounds.Right, bounds.Bottom),
        // Bottom left
        new Vector2(bounds.Left, bounds.Bottom),
      };
    }

    public Grid Grid
    {
      get { return _grid; }
    }

    public Vector2 Position
    {
      get { return _min; }
    }

    public Vector2 Max
    {
      get { return _max; }
    }
  }
}
