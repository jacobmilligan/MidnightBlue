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
  public static class UIColors
  {
    public static Color HUD
    {
      get { return new Color(40, 107, 159); }
    }

    public static Color Background
    {
      get { return HUD * 0.2f; }
    }

    public static Color Border
    {
      get { return HUD * 0.8f; }
    }
  }
}
