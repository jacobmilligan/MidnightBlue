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
  /// Defines a measurement of length in meters able to be converted to other
  /// measurements.
  /// </summary>
  public class Length
  {
    /// <summary>
    /// A single Astronomical Unit in kilometers
    /// </summary>
    public const float AstronomicalUnit = 149597870.7f; //kms

    /// <summary>
    /// The base length
    /// </summary>
    private ulong _meters;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Length"/> class.
    /// </summary>
    /// <param name="meters">Initial length to set in meters.</param>
    public Length(ulong meters)
    {
      _meters = meters;
    }

    /// <summary>
    /// Gets the length in meters
    /// </summary>
    /// <value>The length in meters.</value>
    public ulong Meters
    {
      get { return _meters; }
    }

    /// <summary>
    /// Gets the length in centimeters
    /// </summary>
    /// <value>The length in centimeters.</value>
    public ulong Centimeters
    {
      get { return _meters * 100; }
    }

    /// <summary>
    /// Gets the length in kilometers
    /// </summary>
    /// <value>The length in kilometers.</value>
    public ulong Kilometers
    {
      get { return (ulong)(_meters * 0.001f); }
    }

    /// <summary>
    /// Gets the length in kilometers represented as a smaller value used for
    /// calculations.
    /// </summary>
    /// <value>The length in relative kilometers.</value>
    public int RelativeKilometers
    {
      get { return (int)Kilometers / 100000; }
    }

    /// <summary>
    /// Gets the length in Astronomical Unites
    /// </summary>
    /// <value>The length in astronomical units.</value>
    public float AU
    {
      get { return Kilometers / AstronomicalUnit; }
    }
  }
}
