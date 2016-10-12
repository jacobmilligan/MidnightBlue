//
// 	UnitConverter.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 8/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
namespace MidnightBlue
{
  /// <summary>
  /// Defines a measurement of length in meters
  /// </summary>
  public class Length
  {
    /// <summary>
    /// A single Astronomical Unit in kilometers
    /// </summary>
    public const float AstronomicalUnit = 149597870.7f; //kms

    private ulong _meters;

    public Length(ulong meters)
    {
      _meters = meters;
    }

    public ulong Meters
    {
      get { return _meters; }
    }

    public ulong Centimeters
    {
      get { return _meters * 100; }
    }

    public ulong Kilometers
    {
      get { return (ulong)(_meters * 0.001f); }
    }

    public int RelativeKilometers
    {
      get { return (int)Kilometers / 100000; }
    }

    public float AU
    {
      get { return Kilometers / AstronomicalUnit; }
    }
  }
}
