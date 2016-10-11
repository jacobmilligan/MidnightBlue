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
// C# port of the excellent, original AccidentalNoise Library by
// Joshua Tippetts.
// Port source: https://github.com/TinkerWorX/AccidentalNoiseLibrary
// AccidentalNoise original source: http://accidentalnoise.sourceforge.net/
//

using System;
using MidnightBlue.Engine;
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
    /// Generates a new noise map with a specified number of layers
    /// and density
    /// </summary>
    /// <param name="octaves">Number of layer iterations to make</param>
    /// <param name="frequency">The density of the resulting noise.</param>
    public void Generate()
    {
      //
      // Uses a 2D -> 4D mapping of each axis.
      // Adapted from Jon Gallants original technique.
      // Source: http://www.jgallant.com/procedurally-generating-wrapping-world-maps-in-unity-csharp-part-2/#wrap2
      //
      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {

          //Noise range
          var x1 = 0;
          var x2 = 2;
          var y1 = 0;
          var y2 = 2;
          var dx = x2 - x1;
          var dy = y2 - y1;

          //Sample noise at smaller intervals
          var s = x / (float)_width;
          var t = y / (float)_height;

          // Calculate our 4D coordinates
          var nx = x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI);
          var ny = y1 + Math.Cos(t * 2 * Math.PI) * dy / (2 * Math.PI);
          var nz = x1 + Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI);
          var nw = y1 + Math.Sin(t * 2 * Math.PI) * dy / (2 * Math.PI);

          var noise = _fractal.Get(nx, ny, nz, nw);

          // Keep track of the max and min values found
          // to use for normalization
          if ( noise > _max ) { _max = noise; }
          if ( noise < _min ) { _min = noise; }

          _values[x, y] = noise;
        }
      }

    }

    /// <summary>
    /// Gets a noise value at the specified x and y coordinates. Returned as a
    /// normalized value in the range of 0 - 1
    /// </summary>
    /// <returns>The noise value.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public double GetValue(int x, int y)
    {
      var pos = MBMath.WrapGrid(x, y, _width, _height);
      var noise = _values[pos.X, pos.Y];
      return MBMath.Normalize(noise, 0, 1, _min, _max);
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
      _values[x, y] = MBMath.Normalize(value, 0, 1, _min, _max);
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

    public ImplicitModuleBase Map
    {
      get { return _fractal; }
    }
  }
}
