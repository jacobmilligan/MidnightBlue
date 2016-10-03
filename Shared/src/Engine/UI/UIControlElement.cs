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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

    protected UIState _currentState;
    protected UIState _previousState;

    public UIControlElement(Texture2D normal, Texture2D selected, Texture2D pressed) : base(1, 1)
    {
      NormalTexture = normal;
      HighlightedTexture = selected;
      PressedTexture = pressed;

      NormalTextColor = HighlightedTextColor = TextColor;
      _currentState = UIState.Normal;
    }

    public UIControlElement() : this(null, null, null) { }

    public override void Update()
    {
      var mousePos = Mouse.GetState().Position;

      _previousState = _currentState;

      if ( Bounds.Rect.Contains(mousePos) ) {
        _currentState = UIState.Selected;
        TextColor = HighlightedTextColor;

        if ( HighlightedSound != null && _previousState == UIState.Normal ) {
          HighlightedSound.Play();
        }
      } else {
        _currentState = UIState.Normal;
        TextColor = NormalTextColor;
      }

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      var drawnTexture = NormalTexture;
      switch ( _currentState ) {
        case UIState.Selected:
          drawnTexture = HighlightedTexture;
          break;
        case UIState.Pressed:
          drawnTexture = PressedTexture;
          break;
      }

      var pos = new Vector2(Bounds.Rect.X, Bounds.Rect.Y);

      if ( drawnTexture != null ) {

        var scale = FitChildVectorToParent(
          drawnTexture.Bounds.Size.ToVector2(), Bounds.Grid.CellSize
        );

        spriteBatch.Draw(
          drawnTexture,
          scale: scale,
          position: pos
        );
      }

      if ( TextContent.Length > 0 ) {
        var scale = FitChildVectorToParent(
          Font.MeasureString(TextContent), Bounds.Grid.CellSize
        );

        spriteBatch.DrawString(
          spriteFont: Font,
          text: TextContent,
          position: pos,
          color: TextColor,
          scale: scale,
          rotation: 0,
          origin: new Vector2(0, 0),
          effects: SpriteEffects.None,
          layerDepth: 0
        );
      }
    }

    public Color NormalTextColor { get; set; }
    public Color HighlightedTextColor { get; set; }

    public Texture2D NormalTexture { get; set; }
    public Texture2D HighlightedTexture { get; set; }
    public Texture2D PressedTexture { get; set; }

    public SoundEffect HighlightedSound { get; set; }
  }
}
