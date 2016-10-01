//
// 	UIGrid.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using MidnightBlue.Engine.Geometry;

namespace MidnightBlue.Engine.UI
{
  public class UIGrid
  {
    private UIElement[,] _grid;
    private int _rowSpan;
    private int _colSpan;
    private Rectangle _rect;

    public UIGrid(int rows, int cols, Rectangle parent)
    {
      _grid = new UIElement[rows - 1, cols - 1];
      _rowSpan = parent.Height / (rows);
      _colSpan = parent.Width / (cols);
      _rect = new Rectangle(parent.X, parent.Y, _colSpan, _rowSpan);
    }

    public UIElement[,] Content
    {
      get { return _grid; }
    }

    public Grid Bounds
    {
      get
      {
        return new Grid(_grid.GetLength(0) + 1, _grid.GetLength(1) + 1, _rowSpan, _colSpan);
      }
    }

    public Rectangle Rect
    {
      get { return _rect; }
    }
  }
}
