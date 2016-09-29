//
// 	UIControlElement.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 29/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MidnightBlue.Engine.UI
{
  public enum UIState
  {
    Normal,
    Selected,
    Pressed
  }

  public class UIControlElement : UIElement
  {

    private UIState _currentState;

    public UIControlElement(
      int rows, int cols, Texture2D normal, Texture2D selected, Texture2D pressed
    ) : base(rows, cols)
    {
      NormalTexture = normal;
      SelectedTexture = selected;
      PressedTexture = pressed;
      _currentState = UIState.Normal;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      var drawnTexture = NormalTexture;
      switch ( _currentState ) {
        case UIState.Selected:
          drawnTexture = SelectedTexture;
          break;
        case UIState.Pressed:
          drawnTexture = PressedTexture;
          break;
      }
      var textureSize = drawnTexture.Bounds.Size.ToVector2();
      var pos = new Vector2(Bounds.Rect.X, Bounds.Rect.Y);
      spriteBatch.Draw(
        drawnTexture,
        scale: Bounds.Bounds.CellSize / textureSize,
        position: pos
      );
      textureSize = Font.MeasureString(TextContent);

      spriteBatch.DrawString(
        spriteFont: Font,
        text: TextContent,
        position: pos,
        color: TextColor,
        scale: Bounds.Bounds.CellSize / textureSize,
        rotation: 0,
        origin: new Vector2(0, 0),
        effects: SpriteEffects.None,
        layerDepth: 0
      );
    }

    public Texture2D NormalTexture { get; set; }
    public Texture2D SelectedTexture { get; set; }
    public Texture2D PressedTexture { get; set; }
  }
}
