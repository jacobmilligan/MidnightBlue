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
      _bender = content.Load<SpriteFont>("Fonts/Bender");

      var bars = new Layout(this, 10, 7) {
        BackgroundColor = UIColors.Background,
        BorderTopColor = UIColors.Border,
        BorderBottomColor = UIColors.Border,
        BorderWidth = 3,
        BorderDisplayed = true
      };
      var scan = new Layout(this, 10, 7) {
        BackgroundColor = UIColors.Background,
        BorderTopColor = UIColors.Border,
        BorderBottomColor = UIColors.Border,
        BorderWidth = 3,
        BorderDisplayed = true
      };

      this.Add(bars, 1, 19, 5, 4);
      this.Add(scan, 7, 19, 5, 4);

      _fuel = new Label {
        Font = _bender,
        TextColor = Color.Yellow,
      };
      bars.Add(_fuel, 1, 1, 2, 7);

      var scanHeader = new Label {
        TextContent = "Scan results",
        Font = _bender
      };
      var scanResults = new ListControl(_bender) {
        BorderColor = Color.Tomato,
        BorderDisplayed = true,
        BorderWidth = 2,
        ItemSpan = 200,
        Tag = "scan results"
      };
      scan.Add(scanHeader, 1, 1, 2, 10);
      scan.Add(scanResults, 3, 1, 7, 6);
    }

    public void Refresh(Inventory inventory)
    {
      if ( inventory.Items.ContainsKey(typeof(Fuel)) ) {
        _fuel.TextContent = "Fuel levels: " + inventory.Items[typeof(Fuel)].Count.ToString();
      }
    }
  }
}
