//
// 	MBMath.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MidnightBlue.Engine
{
  public static class MBMath
  {
    public static Point WrapGrid(int x, int y, int width, int height)
    {
      var newX = (x % width + width) % width;
      var newY = (y % height + height) % height;
      return new Point((int)newX, (int)newY);
    }

    public static float Normalize(float value, float low, float high, float dataMin, float dataMax)
    {
      return (high - low) * ((value - dataMin) / (dataMax - dataMin)) + low;
    }

    public static double Normalize(double value, double low, double high, double dataMin, double dataMax)
    {
      return (high - low) * ((value - dataMin) / (dataMax - dataMin)) + low;
    }
  }
}
