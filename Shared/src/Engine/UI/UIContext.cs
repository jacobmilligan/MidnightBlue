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
      var rowLen = Content.Length;
      var colLen = Content.GetLength(1);
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          _grid.Content[row, col].Draw(spriteBatch);
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
