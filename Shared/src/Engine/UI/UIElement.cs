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
using MidnightBlue.Engine.Geometry;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.UI
{
  public abstract class UIElement
  {
    private UIContent _grid;
    private Rectangle _borderRect;
    private Color _baseBorderColor;
    private Line _borderTop, _borderRight, _borderBottom, _borderLeft;
    private int _numRows, _numCols;

    protected UIElement(int rows, int cols)
    {
      BorderDisplayed = false;
      BorderColor = Color.White;
      BorderTopColor = BorderRightColor = BorderBottomColor = BorderLeftColor = BorderColor;
      BorderWidth = 0;

      TextColor = Color.White;
      TextContent = string.Empty;

      StretchToFit = false;

      _numRows = rows;
      _numCols = cols;
    }

    public void SetRelativeSize(UIContent parent, Point at, Point span, Point size)
    {
      var x = parent.Rect.X + (parent.Grid.ColSize * at.X);
      var y = parent.Rect.Y + (parent.Grid.RowSize * at.Y);
      var width = parent.Grid.ColSize * span.X;
      var height = parent.Grid.RowSize * span.Y;

      var rect = new Rectangle(x, y, width, height);

      _grid = new UIContent(_numRows, _numCols, rect);

      _borderRect = rect;
      _borderRect.Width += parent.Grid.ColSize;
      _borderRect.Height += parent.Grid.RowSize;

      _borderTop = new Line(
        new Vector2(_borderRect.X, _borderRect.Y),
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y)
      );
      _borderRight = new Line(
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y),
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y + _borderRect.Height)
      );
      _borderBottom = new Line(
        new Vector2(_borderRect.X + _borderRect.Width, _borderRect.Y + _borderRect.Height),
        new Vector2(_borderRect.X, _borderRect.Y + _borderRect.Height)
      );
      _borderLeft = new Line(
        new Vector2(_borderRect.X, _borderRect.Y + _borderRect.Height),
        new Vector2(_borderRect.X, _borderRect.Y)
      );
    }

    public void SetRelativeSize(UIContent parent, int atRow, int atCol, int rowSpan, int colSpan)
    {
      SetRelativeSize(
        parent, new Point(atCol, atRow), new Point(colSpan, rowSpan), new Point(_numCols, _numRows)
      );
    }

    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Update();

    /// <summary>
    /// Gets a scale vector that fits exactly inside a parent vector
    /// </summary>
    /// <returns>The size fitting vector</returns>
    /// <param name="child">Child size vector.</param>
    /// <param name="parent">Parent size vector.</param>
    protected Vector2 FitChildVectorToParent(Vector2 child, Vector2 parent)
    {
      var scale = new Vector2(1, 1);
      if ( child.X > parent.X || StretchToFit ) {
        scale.X = parent.X / child.X;
      }
      if ( child.Y > parent.Y || StretchToFit ) {
        scale.Y = parent.Y / child.Y;
      }
      return scale;
    }

    protected void DrawBorder(SpriteBatch spriteBatch)
    {
      if ( BorderTopColor != Color.Transparent ) {
        spriteBatch.DrawLine(_borderTop.Start, _borderTop.End, BorderTopColor, BorderWidth);
      }
      if ( BorderRightColor != Color.Transparent ) {
        spriteBatch.DrawLine(_borderRight.Start, _borderRight.End, BorderRightColor, BorderWidth);
      }
      if ( BorderBottomColor != Color.Transparent ) {
        spriteBatch.DrawLine(_borderBottom.Start, _borderBottom.End, BorderBottomColor, BorderWidth);
      }
      if ( BorderLeftColor != Color.Transparent ) {
        spriteBatch.DrawLine(_borderLeft.Start, _borderLeft.End, BorderLeftColor, BorderWidth);
      }
    }

    public UIContent Bounds
    {
      get { return _grid; }
    }

    public Vector2 Size
    {
      get { return new Vector2(_numCols, _numRows); }
    }

    public bool StretchToFit { get; set; }

    public string TextContent { get; set; }
    /// <summary>
    /// Gets or sets the current color of the text.
    /// </summary>
    /// <value>The text contents current color value.</value>
    public Color TextColor { get; set; }
    public SpriteFont Font { get; set; }

    public bool BorderDisplayed { get; set; }
    public int BorderWidth { get; set; }
    public Color BorderColor
    {
      get
      {
        return _baseBorderColor;
      }
      set
      {
        BorderTopColor = BorderRightColor = BorderBottomColor = BorderLeftColor = _baseBorderColor = value;
      }
    }
    public Color BorderTopColor { get; set; }
    public Color BorderRightColor { get; set; }
    public Color BorderBottomColor { get; set; }
    public Color BorderLeftColor { get; set; }
  }
}
