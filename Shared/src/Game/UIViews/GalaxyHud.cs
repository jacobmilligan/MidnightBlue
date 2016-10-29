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

namespace MidnightBlue
{
  /// <summary>
  /// HUD to show in the galaxy view.
  /// </summary>
  public class GalaxyHud : UIView
  {
    /// <summary>
    /// Label used to display the current fuel amount
    /// </summary>
    private Label _fuel;
    /// <summary>
    /// Font to use in displaying elements.
    /// </summary>
    private SpriteFont _bender;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.GalaxyHud"/> class.
    /// </summary>
    /// <param name="content">Content to use in loading fonts and textures.</param>
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

    /// <summary>
    /// Refreshed the HUD with the specified inventory values.
    /// </summary>
    /// <param name="inventory">Inventory to use to refresh the display.</param>
    public void Refresh(Inventory inventory)
    {
      if ( inventory.Items.ContainsKey(typeof(Fuel)) ) {
        _fuel.TextContent = "Fuel levels: " + inventory.Items[typeof(Fuel)].Count.ToString();
      }
    }
  }
}
