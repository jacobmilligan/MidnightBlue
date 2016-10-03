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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MidnightBlue.Engine.UI
{
  public class UIView
  {
    private UIContent _grid;

    public UIView(int rows, int cols)
    {
      _grid = new UIContent(rows, cols, MBGame.Graphics.Viewport.Bounds);
    }

    public void Update()
    {
      var rowLen = Content.GetLength(0);
      var colLen = Content.GetLength(1);
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = _grid.Elements[row, col];
          if ( currGrid != null ) {
            currGrid.Update();
          }
        }
      }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      if ( BackgroundTexture != null ) {

        spriteBatch.Draw(
          BackgroundTexture,
          position: new Vector2(MBGame.Graphics.Viewport.X, MBGame.Graphics.Viewport.Y)
        );
      }

      if ( (bool)MBGame.Console.Vars["drawGrids"] ) {
        spriteBatch.DrawGrid(_grid.Grid, new Point(_grid.Rect.X, _grid.Rect.Y), Color.Yellow);
      }

      var rowLen = Content.GetLength(0);
      var colLen = Content.GetLength(1);
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = _grid.Elements[row, col];
          if ( currGrid != null ) {
            currGrid.Draw(spriteBatch);
          }
        }
      }
    }

    public void Add(UIElement element, int atRow, int atCol, int rowSpan, int colSpan)
    {
      element.SetRelativeSize(_grid, atRow, atCol, rowSpan, colSpan);
      _grid.Elements[atRow, atCol] = element;
    }

    public UIElement[,] Content
    {
      get { return _grid.Elements; }
    }

    public Texture2D BackgroundTexture { get; set; }
  }
}
