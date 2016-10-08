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
  public class ListControl : UIControlElement
  {
    private SpriteFont _font;
    private int _offset, _startItem;
    private Rectangle _upArrow, _downArrow;

    public ListControl(SpriteFont font)
      : this(font, null, null, null)
    { }

    public ListControl(SpriteFont font, Texture2D normal, Texture2D selected, Texture2D pressed)
      : base(normal, selected, pressed)
    {
      Content = new List<string>();
      _font = font;
      SeperaterColor = Color.LightGray;
      ControlSize = 10;
      ItemSpan = 1;
      _startItem = 0;
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
        _offset = BoundingBox.Top;
      }

      HandleScroll();
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

      var nextEntry = 0;
      var itemY = 0;
      for ( int item = 0; item < Count && nextEntry < BoundingBox.Bottom; item++ ) {
        nextEntry = _offset + (ItemSpan * itemY);
        var listEntry = new Vector2(BoundingBox.X, nextEntry);
        spriteBatch.DrawString(
          _font,
          Content[item],
          listEntry,
          SeperaterColor
        );
        itemY++;
      }

      spriteBatch.End();
      spriteBatch.Begin();
      spriteBatch.GraphicsDevice.ScissorRectangle = MBGame.Graphics.Viewport.Bounds;
    }

    private void HandleScroll()
    {
      var mouse = Mouse.GetState();
      if ( _upArrow.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed ) {
        _offset++;
        if ( _offset < BoundingBox.Top ) {
          _startItem--;
        }
      }
      if ( _downArrow.Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed ) {
        _offset--;
        if ( _offset > BoundingBox.Bottom ) {
          _startItem++;
        }
      }

      if ( _startItem < 0 ) {
        _startItem = 0;
      }
      if ( _offset > Count - 1 ) {
        _startItem = Count - 1;
      }
    }


    public List<string> Content { get; set; }
    public int Count
    {
      get { return Content.Count; }
    }
    public Color SeperaterColor { get; set; }
    public int ControlSize { get; set; }

    public int ItemSpan { get; set; }
  }
}
