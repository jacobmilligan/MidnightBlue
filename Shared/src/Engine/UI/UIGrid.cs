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
      _grid = new UIElement[rows, cols];
      _rowSpan = parent.Width / rows;
      _colSpan = parent.Height / cols;
      _rect = new Rectangle(parent.X, parent.Y, _rowSpan, _colSpan);
    }

    public UIElement[,] Content
    {
      get { return _grid; }
    }

    public Grid Bounds
    {
      get
      {
        return new Grid(_grid.GetLength(0), _grid.GetLength(1), _rowSpan, _colSpan);
      }
    }

    public Rectangle Rect
    {
      get { return _rect; }
    }
  }
}
