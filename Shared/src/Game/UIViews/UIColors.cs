//
// 	UIColors.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 12/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using Microsoft.Xna.Framework;

namespace MidnightBlue
{
  /// <summary>
  /// The main color scheme for the UI
  /// </summary>
  public static class UIColors
  {
    /// <summary>
    /// Color to use for all basic hud elements
    /// </summary>
    /// <value>The hud color.</value>
    public static Color HUD
    {
      get { return new Color(40, 107, 159); }
    }

    /// <summary>
    /// Color to use for all basic UI backgrounds
    /// </summary>
    /// <value>The background color.</value>
    public static Color Background
    {
      get { return HUD * 0.2f; }
    }

    /// <summary>
    /// Color to use for all basic borders in the UI
    /// </summary>
    /// <value>The border color.</value>
    public static Color Border
    {
      get { return HUD * 0.8f; }
    }
  }
}
