//
// 	GeometryHelper.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 10/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MidnightBlue.Engine
{
  public static class GeometryHelper
  {
    //public static int WrapGrid(int x, int y, int width, int height)
    //{
    //  return (x & (width - 1)) + (y & (height - 1)) * width;
    //}
    public static Point WrapGrid(int x, int y, int width, int height)
    {
      var xResult = 0;
      var yResult = 0;

      if ( x >= 0 ) {
        xResult = x % width; // wrap right
      } else {
        xResult = (width + x % width) % width; // wrap left
      }

      if ( y >= 0 ) {
        yResult = y % height; // wrap down
      } else {
        yResult = (height + y % height) % height; // wrap up
      }

      return new Point(xResult, yResult);
    }


  }
}
