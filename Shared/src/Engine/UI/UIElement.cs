//
// 	UIElement.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.UI
{
  public abstract class UIElement
  {
    private UIGrid _grid;
    private int _numRows, _numCols;

    protected UIElement(int rows, int cols)
    {
      BorderDisplayed = false;
      BorderColor = Color.Transparent;
      BorderWidth = 0;

      TextColor = Color.Black;
      TextContent = string.Empty;
      _numRows = rows;
      _numCols = cols;
    }

    public void SetRelativeSize(UIGrid parent, Point at, Point span, Point size)
    {
      var x = parent.Rect.X + (parent.Bounds.ColSize * at.X);
      var y = parent.Rect.Y + (parent.Bounds.RowSize * at.Y);
      var width = parent.Bounds.ColSize * span.X;
      var height = parent.Bounds.RowSize * span.Y;

      var rect = new Rectangle(x, y, width, height);

      _grid = new UIGrid(size.X, size.Y, rect);
    }

    public void SetRelativeSize(UIGrid parent, int atRow, int atCol, int rowSpan, int colSpan)
    {
      SetRelativeSize(
        parent, new Point(atCol, atRow), new Point(colSpan, rowSpan), new Point(_numCols, _numRows)
      );
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    protected void DrawBorder(SpriteBatch spriteBatch)
    {
      // Draw the border
      spriteBatch.DrawRectangle(
        _grid.Rect, BorderColor, BorderWidth
      );
    }

    public UIGrid Bounds
    {
      get { return _grid; }
    }

    public string TextContent { get; set; }
    public Color TextColor { get; set; }
    public SpriteFont Font { get; set; }

    public bool BorderDisplayed { get; set; }
    public Color BorderColor { get; set; }
    public int BorderWidth { get; set; }
  }
}
