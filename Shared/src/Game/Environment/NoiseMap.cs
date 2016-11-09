//
// 	MapTest3.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

//
// Generates a Noise map that wraps on both axis' using
// the 4D Simplex Noise class provided as part of Nikolaj Mariager's
// C# port of the excellent, AccidentalNoise Library by
// Joshua Tippetts.
// Port source: https://github.com/TinkerWorX/AccidentalNoiseLibrary
// AccidentalNoise original source: http://accidentalnoise.sourceforge.net/
//

using System;
using MB2D;
using TinkerWorX.AccidentalNoiseLibrary;

namespace MidnightBlue
{
  /// <summary>
  /// Generates a fractal 2D map using Simplex Noise
  /// </summary>
  public class NoiseMap
  {
    /// <summary>
    /// The width of the noise map
    /// </summary>
    private int _width,
    /// <summary>
    /// The height of the noise map
    /// </summary>
    _height,
    /// <summary>
    /// The seed used to generate the map
    /// </summary>
    _seed;

    /// <summary>
    /// The maximum value in the current noise map
    /// </summary>
    double _max,
    /// <summary>
    /// The minimum value in the current noise map
    /// </summary>
    _min;

    /// <summary>
    /// The collection of noise values
    /// </summary>
    double[,] _values;

    /// <summary>
    /// The fractal generator
    /// </summary>
    ImplicitModuleBase _fractal;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.NoiseMap"/> class.
    /// Initializes the fractal generator to use Simplex Noise
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    /// <param name="seed">Seed to use in generating the noise map.</param>
    public NoiseMap(ImplicitModuleBase fractal, int width, int height, int seed)
    {
      _seed = seed;
      _width = width;
      _height = height;

      _values = new double[width, height];

      _fractal = fractal;

      // Assign seed, otherwise uses the default random seed provided
      // by AccidentalNoise
      if ( seed > 0 ) {
        _fractal.Seed = _seed;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.NoiseMap"/> class.
    /// Initializes the fractal generator to use Simplex Noise
    /// </summary>
    /// <param name="width">Width of the noise map.</param>
    /// <param name="height">Height of the noise map.</param>
    public NoiseMap(ImplicitModuleBase fractal, int width, int height) : this(fractal, width, height, 0) { }

    /// <summary>
    /// Gets a noise value at the specified x and y coordinates. Returned as a
    /// normalized value in the range of 0 - 1
    /// </summary>
    /// <returns>The noise value.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public double GetValue(int x, int y)
    {
      return MBMath.Normalize(_values[x, y], 0, 1, _min, _max);
    }

    /// <summary>
    /// Sets a noise value at the specified x and y coordinates. Assigned as a
    /// normalized value in the range of 0 - 1
    /// </summary>
    /// <returns>The noise value.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public void SetValue(int x, int y, double value)
    {
      // Keep track of the max and min values found
      // to use for normalization
      if ( value > _max ) { _max = value; }
      if ( value < _min ) { _min = value; }

      _values[x, y] = value;
    }

    /// <summary>
    /// Gets the width of the noise map.
    /// </summary>
    /// <value>The width.</value>
    public int Width
    {
      get { return _width; }
    }

    /// <summary>
    /// Gets the height of the noise map.
    /// </summary>
    /// <value>The height.</value>
    public int Height
    {
      get { return _height; }
    }

    /// <summary>
    /// Gets the maximum value found in the currently generated noise map.
    /// </summary>
    /// <value>The max value.</value>
    public double MaxValue
    {
      get { return _max; }
    }

    /// <summary>
    /// Gets the minimum value found in the currently generated noise map.
    /// </summary>
    /// <value>The max value.</value>
    public double MinValue
    {
      get { return _min; }
    }

    /// <summary>
    /// Gets the internal map.
    /// </summary>
    /// <value>The map.</value>
    public ImplicitModuleBase Map
    {
      get { return _fractal; }
    }
  }
}
