//
//  UIContext.cs
//  Midnight Blue
//
//  --------------------------------------------------------------
//
//  Created by Jacob Milligan on 27/09/2016.
//  Copyright (c) Jacob Milligan All rights reserved
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MidnightBlue.Engine.UI
{
  public class UIContext
  {
    private UIGrid _grid;

    public UIContext(int rows, int cols)
    {
      _grid = new UIGrid(rows, cols, MBGame.Graphics.Viewport.Bounds);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if ( (bool)MBGame.Console.Vars["drawGrids"] ) {
        spriteBatch.DrawGrid(_grid.Bounds, new Point(_grid.Rect.X, _grid.Rect.Y), Color.Yellow);
      }

      var rowLen = Content.GetLength(0);
      var colLen = Content.GetLength(1);
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = _grid.Content[row, col];
          if ( currGrid != null ) {
            currGrid.Draw(spriteBatch);
          }
        }
      }
    }

    public void Add(UIElement element, int atRow, int atCol, int rowSpan, int colSpan)
    {
      element.SetRelativeSize(_grid, atRow, atCol, rowSpan, colSpan);
      _grid.Content[atRow, atCol] = element;
    }

    public UIElement[,] Content
    {
      get { return _grid.Content; }
    }

  }
}
