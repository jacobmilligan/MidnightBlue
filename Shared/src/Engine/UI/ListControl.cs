//
// 	ListView.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 9/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.IO;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.UI
{
  /// <summary>
  /// A scrollable list box. Items can be added and interactied with.
  /// </summary>
  public class ListControl : UIControlElement
  {
    /// <summary>
    /// Font to render in the list box
    /// </summary>
    private SpriteFont _font;

    /// <summary>
    /// The index in the List of the currently selected item
    /// </summary>
    private int _selectedItem;

    /// <summary>
    /// The up arrow box
    /// </summary>
    private Rectangle _upArrow,
    /// <summary>
    /// The down arrow box
    /// </summary>
    _downArrow;

    /// <summary>
    /// The rectangle encompassing the entire list. Culled by the outer, visible bounding box
    /// of the list.
    /// </summary>
    private Rectangle _listRect;

    /// <summary>
    /// All elements currently in the list.
    /// </summary>
    private List<string> _elements;

    public ListControl(SpriteFont font)
      : this(font, null, null, null)
    { }

    public ListControl(SpriteFont font, Texture2D normal, Texture2D selected, Texture2D pressed)
      : base(normal, selected, pressed)
    {
      _elements = new List<string>();

      SeperaterColor = Color.LightGray;
      ControlSize = 10;
      ItemSpan = 1;
      _selectedItem = -1;
      _font = font;

      _listRect = new Rectangle(
        BoundingBox.Location.X,
        BoundingBox.Location.Y,
        BoundingBox.Size.X - ControlSize,
        BoundingBox.Size.Y
      );
    }


    public override void Update()
    {
      if ( _upArrow == Rectangle.Empty || _downArrow == Rectangle.Empty ) {
        _upArrow = new Rectangle(
          BoundingBox.Right - ControlSize - BorderWidth,
          BoundingBox.Top + BorderWidth,
          ControlSize,
          ControlSize
        );

        _downArrow = new Rectangle(
          BoundingBox.Right - ControlSize - BorderWidth,
          BoundingBox.Bottom - ControlSize - BorderWidth,
          ControlSize,
          ControlSize
        );

        _listRect = new Rectangle(
          BoundingBox.Location.X,
          BoundingBox.Location.Y,
          BoundingBox.Size.X - ControlSize,
          BoundingBox.Size.Y
        );
      }

      HandleScroll();
      HandleClick();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      base.Draw(spriteBatch);

      spriteBatch.End();

      var rasterizeState = new RasterizerState() { ScissorTestEnable = true };

      spriteBatch.Begin(rasterizerState: rasterizeState);
      spriteBatch.GraphicsDevice.ScissorRectangle = BoundingBox;

      spriteBatch.DrawRectangle(_upArrow, SeperaterColor, 2);
      spriteBatch.DrawRectangle(_downArrow, SeperaterColor, 2);

      if ( _selectedItem >= 0 ) {
        spriteBatch.FillRectangle(
          BoundingBox.X,
          _listRect.Top + (_selectedItem * ItemSpan),
          BoundingBox.Width,
          ItemSpan,
          BackgroundColor
        );
      }

      var nextEntry = 0;
      var itemY = 0;

      for ( int item = 0; item < Count && nextEntry < BoundingBox.Bottom; item++ ) {

        nextEntry = _listRect.Location.Y + (ItemSpan * itemY);
        var listEntry = new Vector2(BoundingBox.X, nextEntry);

        spriteBatch.DrawString(
          _font,
          Elements[item],
          listEntry,
          SeperaterColor
        );

        itemY++;
      }


      spriteBatch.End();
      spriteBatch.Begin();
      spriteBatch.GraphicsDevice.ScissorRectangle = MBGame.Graphics.Viewport.Bounds;
    }

    private void HandleClick()
    {
      var mouse = Mouse.GetState();

      for ( int element = 0; element < Count; element++ ) {

        var listItemRect = new Rectangle(
          BoundingBox.X,
          _listRect.Y + (ItemSpan * element),
          _listRect.Width,
          ItemSpan
        );

        if ( IOUtil.LeftMouseClicked() && listItemRect.Contains(mouse.Position) ) {
          _selectedItem = element;
        }

      }
    }

    private void HandleScroll()
    {
      var mouse = Mouse.GetState();
      var listPos = _listRect.Location;

      if ( _upArrow.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed ) {
        if ( listPos.Y <= BoundingBox.Top ) {
          _listRect.Location = new Point(listPos.X, listPos.Y + 1);
        }
      }
      if ( _downArrow.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed ) {
        if ( listPos.Y + _listRect.Height > BoundingBox.Bottom ) {
          _listRect.Location = new Point(listPos.X, listPos.Y - 1);
        }
      }

    }

    public List<string> Elements
    {
      get
      {
        return _elements;
      }
      set
      {
        _elements = value;
        _listRect.Height = Count * ItemSpan;
      }
    }
    public int Count
    {
      get { return _elements.Count; }
    }
    public Color SeperaterColor { get; set; }
    public int ControlSize { get; set; }

    public int ItemSpan { get; set; }
  }
}
