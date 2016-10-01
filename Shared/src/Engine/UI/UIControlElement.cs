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

    public override void Update()
    {
      var mousePos = Mouse.GetState().Position;
      if ( Bounds.Rect.Contains(mousePos) ) {
        _currentState = UIState.Selected;
      } else {
        _currentState = UIState.Normal;
      }
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
      var pos = new Vector2(Bounds.Rect.X, Bounds.Rect.Y);

      var scale = FitChildVectorToParent(
        drawnTexture.Bounds.Size.ToVector2(), Bounds.Bounds.CellSize
      );

      spriteBatch.Draw(
        drawnTexture,
        scale: scale,
        position: pos
      );

      scale = FitChildVectorToParent(
        Font.MeasureString(TextContent), Bounds.Bounds.CellSize
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

    /// <summary>
    /// Gets a scale vector that fits exactly inside a parent vector
    /// </summary>
    /// <returns>The size fitting vector</returns>
    /// <param name="child">Child size vector.</param>
    /// <param name="parent">Parent size vector.</param>
    private Vector2 FitChildVectorToParent(Vector2 child, Vector2 parent)
    {
      var scale = new Vector2(1, 1);
      if ( child.X > parent.X ) {
        scale.X = parent.X / child.X;
      }
      if ( child.Y > parent.Y ) {
        scale.Y = parent.Y / child.Y;
      }
      return scale;
    }

    public Texture2D NormalTexture { get; set; }
    public Texture2D SelectedTexture { get; set; }
    public Texture2D PressedTexture { get; set; }
  }
}
