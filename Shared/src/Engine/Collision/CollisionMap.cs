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
    private LinkedList<ulong> _checkedIDs;
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

      _checkedIDs = new LinkedList<ulong>();
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

    public void Insert(Entity entity, CollisionComponent collision)
    {

      var boxes = collision.Boxes;
      var numBoxes = boxes.Count;
      for ( int b = 0; b < numBoxes; b++ ) {
        var corners = GetCorners(boxes[b]);

        foreach ( var corner in corners ) {
          var index = IndexOf(corner);
          if ( IndexExists(index) ) {
            var cell = _cells[index.X, index.Y];
            if ( cell == null ) {
              _cells[index.X, index.Y] = new CollisionCell();
              cell = _cells[index.X, index.Y];
            }
            if ( !cell.Contains(entity) ) {
              cell.Add(entity);
              _nonEmptyCells.Add(cell);
            }
          }
        }
      }
    }

    public List<Entity> GetCollisions(Entity entity, CollisionComponent collision)
    {
      var result = new List<Entity>();
      var boxes = collision.Boxes;
      Vector2[] corners = new Vector2[4];
      for ( int b = 0; b < boxes.Count; b++ ) {
        corners = GetCorners(boxes[b]);
      }

      //TODO: Remove this once tested for larger collision amounts
      _checkedIDs.Clear();

      foreach ( var corner in corners ) {
        var index = IndexOf(corner);
        if ( IndexExists(index) ) {
          var cell = _cells[index.X, index.Y];
          if ( cell != null ) {
            var maxItems = cell.Items.Count;
            foreach ( var e in cell.Items ) {
              if ( !_checkedIDs.Contains(e.ID) && e.ID != entity.ID ) {
                result.Add(e);
                _checkedIDs.AddFirst(e.ID);
              }
            }
          }
        }
      }

      return result;
    }

    public void UpdatePosition(int x, int y)
    {
      _min.X = x;
      _min.Y = y;
      _max.X = _max.X + x;
      _max.Y = _max.Y + y;
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
