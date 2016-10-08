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
    private Color _hudColor = new Color(40, 107, 159);
    private Color _backgroundColor;
    private Color _borderColor;

    public GalaxyHud(ContentManager content) : base(25, 25)
    {
      _backgroundColor = _hudColor * 0.2f;
      _borderColor = _hudColor * 0.8f;
      _bender = content.Load<SpriteFont>("Fonts/Bender");

      var bars = new Layout(10, 7) {
        BackgroundColor = _backgroundColor,
        BorderTopColor = _borderColor,
        BorderBottomColor = _borderColor,
        BorderWidth = 3,
        BorderDisplayed = true
      };
      this.Add(bars, 1, 20, 5, 4);

      _fuel = new Label {
        Font = _bender,
        TextColor = Color.Yellow,
      };
      bars.Add(_fuel, 1, 1, 2, 7);
    }

    public void Refresh(Inventory inventory)
    {
      if ( inventory.Items.ContainsKey(typeof(Fuel)) ) {
        _fuel.TextContent = "Fuel levels: " + inventory.Items[typeof(Fuel)].Count.ToString();
      }
    }
  }
}
