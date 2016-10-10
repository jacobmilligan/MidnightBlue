//
// 	DiamondSquare.cs
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
  /// <summary>
  /// Generates a fractal 2D map using Diamond Square (Random Midpoint Displacement)
  /// </summary>
  public class DiamondSquare
  {
    /// <summary>
    /// The map size
    /// </summary>
    private int _size;
    /// <summary>
    /// Seed to use in generating the map
    /// </summary>
    private int _seed;
    /// <summary>
    /// The random number generator used in the process
    /// </summary>
    private Random _rand;
    /// <summary>
    /// The generated values map
    /// </summary>
    private double[,] _values;

    /// <summary>
    /// Initialize the values and random generator with the specified size.
    /// </summary>
    /// <param name="size">Size of the fractal map.</param>
    /// <param name="willWrap">Set to true to allow wrapping of the fractal for tiling</param>
    private void Initialize(int size, bool willWrap)
    {
      _size = size;
      _rand = new Random(_seed);
      _values = new double[_size, _size];

      if ( willWrap ) {
        _values[0, 0]
          = _values[size - 1, 0]
          = _values[size - 1, size - 1]
          = _values[0, size - 1]
          = -1;
        _values[size / 2, size / 2] = 1;
      } else {
        _values[0, 0] = RandomDouble();
        _values[size - 1, 0] = RandomDouble();
        _values[size - 1, size - 1] = RandomDouble();
        _values[0, size - 1] = RandomDouble();
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.DiamondSquare"/> class.
    /// Uses the current time in ticks as the seed.
    /// </summary>
    /// <param name="size">Size of the fractal map - <b>must be a power of 2 to be valid</b>.</param>
    /// <param name="willWrap">Set to true to allow wrapping of the fractal for tiling</param>
    public DiamondSquare(int size, bool willWrap)
    {
      if ( !MathHelper.IsPowerOfTwo(size) ) {
        throw new ArgumentOutOfRangeException(nameof(size), "'" + size + "' is not a power of 2");
      }

      _seed = (int)DateTime.Now.Ticks;
      Initialize(size, willWrap);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.DiamondSquare"/> class.
    /// </summary>
    /// <param name="size">Size of the map - <b>must be a power of 2 to be valid</b>.</param>
    /// <param name="willWrap">Set to true to allow wrapping of the fractal for tiling</param>
    /// <param name="seed">Seed to use for random values.</param>
    public DiamondSquare(int size, bool willWrap, int seed)
    {
      if ( !MathHelper.IsPowerOfTwo(size) ) {
        throw new ArgumentOutOfRangeException(nameof(size), "'" + size + "' is not a power of 2");
      }

      _seed = seed;
      Initialize(size, willWrap);
    }

    /// <summary>
    /// Gets a value from the generated fractal map.
    /// </summary>
    /// <returns>The value between -1 and 1.</returns>
    /// <param name="x">The x coordinate to get - wraps if larger or smaller than the map size.</param>
    /// <param name="y">The y coordinate to get  - wraps if larger or smaller than the map size.</param>
    public double GetValue(int x, int y)
    {
      var pos = GeometryHelper.WrapGrid(x, y, _size, _size);
      return _values[pos.X, pos.Y];
    }

    /// <summary>
    /// Generates the fractal using Diamond-Square (Random Midpoint Displacement).
    /// </summary>
    public void Generate(int featureSize)
    {
      var instanceSize = featureSize;
      var scale = 3.0; //FIXME: HArdcoded value

      while ( instanceSize > 1 ) {

        Iterate(instanceSize, scale);

        instanceSize /= 2;
        scale /= 2.0;
      }
    }

    /// <summary>
    /// Returns a random double in the range 
    /// </summary>
    /// <returns>The random double value.</returns>
    private double RandomDouble()
    {
      return _rand.NextDouble() * 2 - 1;
    }

    /// <summary>
    /// Iterates the Diamond Square process once.
    /// </summary>
    /// <param name="step">Value of the current step in the process.</param>
    /// <param name="currentScale">The scales current value in the process.</param>
    private void Iterate(int step, double currentScale)
    {
      var halfStep = step / 2;

      // Handle squares
      for ( int y = halfStep; y < halfStep + _size; y += step ) {
        for ( int x = halfStep; x < halfStep + _size; x += step ) {
          HandleSquare(x, y, step, RandomDouble() * currentScale);
        }
      }

      // Handle diamonds
      for ( int y = 0; y < _size; y += step ) {
        for ( int x = 0; x < _size; x += step ) {
          HandleDiamond(x + halfStep, y, step, RandomDouble() * currentScale);
          HandleDiamond(x, y + halfStep, step, RandomDouble() * currentScale);
        }
      }
    }

    /// <summary>
    /// Handles the square step. Sums surrounding points equidistant from
    /// the current midpoint at (x, y) in a square shape and assigns it to the midpoint.
    /// </summary>
    /// <param name="x">The x coordinate to handle.</param>
    /// <param name="y">The y coordinate handle.</param>
    /// <param name="step">Current step in the process.</param>
    /// <param name="newValue">New value to assign to the midpoint.</param>
    private void HandleSquare(int x, int y, int step, double newValue)
    {
      var halfStep = step / 2;

      var a = GetValue(x - halfStep, y - halfStep);
      var b = GetValue(x + halfStep, y - halfStep);
      var c = GetValue(x - halfStep, y + halfStep);
      var d = GetValue(x + halfStep, y + halfStep);

      var pos = GeometryHelper.WrapGrid(x, y, _size, _size);
      _values[pos.X, pos.Y] = ((a + b + c + d) / 4.0) + newValue;
    }

    /// <summary>
    /// Handles the diamond step. Sums the points in a diamond shape around midpoint at (x, y)
    /// and assigns it the new value
    /// </summary>
    /// <param name="x">The x coordinate to handle.</param>
    /// <param name="y">The y coordinate handle.</param>
    /// <param name="step">Current step in the process.</param>
    /// <param name="newValue">New value to assign to the midpoint.</param>
    private void HandleDiamond(int x, int y, int step, double newValue)
    {
      var halfStep = step / 2;

      var b = GetValue(x + halfStep, y);
      var d = GetValue(x - halfStep, y);
      var a = GetValue(x, y - halfStep);
      var c = GetValue(x, y + halfStep);

      var pos = GeometryHelper.WrapGrid(x, y, _size, _size);
      _values[pos.X, pos.Y] = ((a + b + c + d) / 4.0) + newValue;
    }

    /// <summary>
    /// Gets the size of the fractal map
    /// </summary>
    /// <value>The maps size.</value>
    public int Size
    {
      get { return _size; }
    }
  }
}
