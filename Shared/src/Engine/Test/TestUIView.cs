//
// 	TestUIView.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 3/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.UI;

namespace MidnightBlue.Engine
{
  public class TestUIView : UIView
  {
    public TestUIView(ContentManager content, int rows, int cols) : base(rows, cols)
    {
      var normal = content.Load<Texture2D>("Images/blue_button00");
      var down = content.Load<Texture2D>("Images/blue_button01");
      var font = content.Load<SpriteFont>("Bender");
      var btn = new Button(normal, down, down) {
        Font = font,
        TextColor = Color.White,
        TextContent = "Test Button"
      };
      var layout1 = new Layout(20, 20) {
        BorderColor = Color.Black,
        BorderWidth = 2,
      };
      this.Add(layout1, 2, 2, 10, 10);
      layout1.Add(btn, 1, 1, 4, 4);
    }
  }
}
