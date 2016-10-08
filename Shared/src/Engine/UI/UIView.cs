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
  /// <summary>
  /// A single context for all UI elements and layouts.
  /// </summary>
  public class UIView
  {
    /// <summary>
    /// The content and grid representation of the View
    /// </summary>
    private UIContent _grid;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.UI.UIView"/> class.
    /// Divides itself into the number of rows and columns evenly based on the size of the
    /// current viewport.
    /// </summary>
    /// <param name="rows">Number of rows in the view</param>
    /// <param name="cols">Number of columns in the view</param>
    public UIView(int rows, int cols)
    {
      _grid = new UIContent(rows, cols, MBGame.Graphics.Viewport.Bounds);
    }

    /// <summary>
    /// Updates and handles input for all elements in the View.
    /// </summary>
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

    /// <summary>
    /// Draws the View and its elements to the window
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw to.</param>
    public void Draw(SpriteBatch spriteBatch)
    {
      if ( BackgroundTexture != null ) {
        // Draws the background image if it exists
        spriteBatch.Draw(
          BackgroundTexture,
          position: new Vector2(0, 0)
        );
      }

      // DEBUG: Draws the Views grid to the window
      if ( (bool)MBGame.Console.Vars["drawGrids"] ) {
        spriteBatch.DrawGrid(_grid.Grid, new Point(0, 0), Color.Yellow);
      }

      var rowLen = Content.GetLength(0);
      var colLen = Content.GetLength(1);

      // Draw all elements to the window
      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = _grid.Elements[row, col];
          if ( currGrid != null ) {
            currGrid.Draw(spriteBatch);
          }
        }
      }
    }

    /// <summary>
    /// Adds a new UIElement to the View
    /// </summary>
    /// <param name="element">Element to add.</param>
    /// <param name="atRow">Row position in the View.</param>
    /// <param name="atCol">Column position in the View.</param>
    /// <param name="rowSpan">Number of rows the element takes up.</param>
    /// <param name="colSpan">Number of columns the element takes up.</param>
    public void Add(UIElement element, int atRow, int atCol, int rowSpan, int colSpan)
    {
      element.SetRelativeSize(_grid, atRow, atCol, rowSpan, colSpan);
      _grid.Elements[atRow, atCol] = element;
    }

    /// <summary>
    /// Gets the elements this View contains in a 2D array
    /// </summary>
    /// <value>All UIElements.</value>
    public UIElement[,] Content
    {
      get { return _grid.Elements; }
    }

    /// <summary>
    /// Gets or sets the background texture of the view.
    /// </summary>
    /// <value>The background texture.</value>
    public Texture2D BackgroundTexture { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the background image shoudl stretch to fit
    /// the window.
    /// </summary>
    /// <value><c>true</c> if background should be stretched; otherwise, <c>false</c>.</value>
    public bool StretchBackground { get; set; }
  }
}
