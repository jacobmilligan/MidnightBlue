//
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
  public class Label : UIElement
  {
    public Label() : base(1, 1) { }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if ( BorderDisplayed ) {
        DrawBorder(spriteBatch);
      }

      var pos = new Vector2(Bounds.Rect.X, Bounds.Rect.Y);

      if ( TextContent.Length > 0 ) {
        var scale = FitChildVectorToParent(Font.MeasureString(TextContent), Bounds.Grid.CellSize);

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

    public override void Update()
    {
    }
  }
}
