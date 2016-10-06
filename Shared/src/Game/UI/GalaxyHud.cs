//
// 	GalaxyHud.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 7/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.UI;

namespace MidnightBlue.Engine
{
  public class GalaxyHud : UIView
  {
    private Label _fuel;
    private SpriteFont _bender;

    public GalaxyHud(ContentManager content) : base(25, 25)
    {
      _bender = content.Load<SpriteFont>("Bender");

      var bars = new Layout(2, 10) {
        BorderColor = Color.White,
        BorderWidth = 1
      };
      this.Add(bars, 22, 1, 1, 10);

      _fuel = new Label {
        Font = _bender,
        TextColor = Color.Yellow,
      };
      bars.Add(_fuel, 1, 1, 2, 2);
    }

    public void Refresh(Inventory inventory)
    {
      if ( inventory.Items.ContainsKey(typeof(Fuel)) ) {
        _fuel.TextContent = "Fuel levels: " + inventory.Items[typeof(Fuel)].Count.ToString();
      }
    }
  }
}
