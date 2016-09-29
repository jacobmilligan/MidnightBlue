//
// 	Grid.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using System;
using Microsoft.Xna.Framework;

namespace MidnightBlue.Engine.Geometry
{
  public class Grid
  {
    public Grid(int rows, int cols, int rowSize, int colSize)
    {
      RowCount = rows;
      ColCount = cols;
      RowSize = rowSize;
      ColSize = colSize;
    }

    public int RowCount { get; set; }
    public int ColCount { get; set; }
    public int RowSize { get; set; }
    public int ColSize { get; set; }

    public Vector2 CellSize
    {
      get { return new Vector2(ColSize, RowSize); }
    }
  }
}
