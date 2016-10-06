﻿//
// 	Label.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 3/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.UI;

namespace MidnightBlue.Engine.UI
{
  /// <summary>
  /// A static UIElement with a TextContent, border and optional texture
  /// </summary>
  public class Label : UIElement
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.UI.Label"/> class.
    /// </summary>
    public Label() : base(1, 1) { }

    /// <summary>
    /// Draw the label to the window.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw to.</param>
    public override void Draw(SpriteBatch spriteBatch)
    {
      if ( BorderDisplayed ) {
        DrawBorder(spriteBatch);
      }

      var pos = Content.Rect.Location.ToVector2();

      if ( TextContent.Length > 0 ) {
        var scale = FitChildVectorToParent(Font.MeasureString(TextContent), Content.Grid.CellSize);

        // Draws the TextContent to the window
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

    /// <summary>
    /// Updates the labels state
    /// </summary>
    public override void Update()
    {
    }
  }
}
