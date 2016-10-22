//
// 	Planet.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine;
using MidnightBlue.Engine.Tiles;
using MonoGame.Extended.Shapes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TinkerWorX.AccidentalNoiseLibrary;

namespace MidnightBlue
{
  /// <summary>
  /// A fully-generated planet in a star system with associated texture maps.
  /// </summary>
  public class Planet
  {
    /// <summary>
    /// The size of each cell in the generated texture map - 1px x 1px
    /// </summary>
    private const int _cellSize = 1;

    /// <summary>
    /// The total width and height of the planets 2D map
    /// </summary>
    private int _width, _height;

    /// <summary>
    /// Determines whether this planet has been generated or only pre-setup
    /// </summary>
    private bool _generated;

    /// <summary>
    /// The name of the planet
    /// </summary>
    private string _name;

    /// <summary>
    /// Simplex Noise map used for generating elevation values
    /// </summary>
    private NoiseMap _elevation,
    /// <summary>
    /// Simplex Noise map used for generating temperature values with a bias to north and south for lower values
    /// </summary>
    _temperature,
    /// <summary>
    /// Simplex Noise map used for generating moisture values
    /// </summary>
    _moisture;

    /// <summary>
    /// Pre-generated metadata about the planets parameters gotten from the galaxy view
    /// </summary>
    private PlanetMetadata _meta;

    /// <summary>
    /// The generated noise map textures - height, moisture, and heat as well as the biome map
    /// </summary>
    private Dictionary<string, Texture2D> _layers;

    /// <summary>
    /// Gradient used to mask the drawing of the planet map to look like a sphere
    /// </summary>
    private Texture2D _planetMask;

    /// <summary>
    /// Each tile in the planet with a biome, temp etc.
    /// </summary>
    private PlanetTile[,] _tiles;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Planet"/> class
    /// and sets up all noise maps ready for generation.
    /// </summary>
    /// <param name="meta">Metadata received from the planet view to act as parameters for generation.</param>
    /// <param name="seed">Seed to use in generating the map.</param>
    public Planet(PlanetMetadata meta, int seed)
    {
      _meta = meta;
      _name = _meta.Name;
      _generated = false;

      var scaledRadius = (MathHelper.Pi * _meta.Radius * 2) / 10000;
      scaledRadius = scaledRadius + (scaledRadius / 2);
      // Limits max map size
      if ( scaledRadius > 1050 ) {
        scaledRadius = 1050;
      }
      _height = _width = (int)scaledRadius;

      _tiles = new PlanetTile[_width, _height];

      // Setup elevation map
      var elevationMap = new ImplicitFractal(
        FractalType.Multi,
        BasisType.Simplex,
        InterpolationType.Quintic
      );
      elevationMap.Octaves = 6;
      elevationMap.Frequency = 1.8;
      elevationMap.Seed = seed;
      _elevation = new NoiseMap(elevationMap, _width, _height);

      // Setup heat gradient
      var gradient = new ImplicitGradient(1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1);
      gradient.Seed = seed;

      // Set up temperature map
      var temperatureMap = new ImplicitFractal(
        FractalType.Multi,
        BasisType.Simplex,
        InterpolationType.Quintic
      );
      temperatureMap.Octaves = 6;
      temperatureMap.Frequency = 1.8;
      temperatureMap.Seed = seed;

      // Setup combination between gradient and temperature map
      var combiner = new ImplicitCombiner(CombinerType.Multiply);
      combiner.Seed = seed;
      combiner.AddSource(gradient);
      combiner.AddSource(temperatureMap);

      _temperature = new NoiseMap(combiner, _width, _height);

      // Setup moisture map
      var moistureMap = new ImplicitFractal(
        FractalType.Multi,
        BasisType.Simplex,
        InterpolationType.Quintic
      );
      moistureMap.Octaves = 6;
      moistureMap.Frequency = 1.8;
      moistureMap.Seed = seed;
      _moisture = new NoiseMap(moistureMap, _width, _height);

      _layers = new Dictionary<string, Texture2D>();
    }

    /// <summary>
    /// Generates the planet after setting up with pre-defined metadata parameters.
    /// </summary>
    /// <param name="rand">Random number generator from galaxy view to use in generating the planet.</param>
    public void Generate(Random rand)
    {
      GenerateNoiseData();
      //CreateNoiseMapTexture("elevation", _elevation);
      //CreateNoiseMapTexture("moisture", _temperature);
      //CreateNoiseMapTexture("temperature", _temperature);

      // Generate all tiles in the map
      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {
          var temperature = _temperature.GetValue(x, y);
          var moisture = _moisture.GetValue(x, y);
          var elevation = _elevation.GetValue(x, y);

          // Adjust values based off generated average surface temperature
          // from the pre-generated planet metadata in the galaxy view.
          if ( _meta.SurfaceTemperature < -100 ) {
            temperature -= temperature;
          } else if ( _meta.SurfaceTemperature < -80 ) {
            temperature -= 0.8f * temperature;
          } else if ( _meta.SurfaceTemperature < -50 ) {
            temperature -= 0.5f * temperature;
            moisture += 0.3f * moisture;
          } else if ( _meta.SurfaceTemperature < -30 ) {
            temperature -= 0.4f * temperature;
            moisture += 0.3f * moisture;
          } else if ( _meta.SurfaceTemperature < -10 ) {
            temperature -= 0.4f * temperature;
            moisture += 0.2f * moisture;
          } else if ( _meta.SurfaceTemperature < 40 ) {
            temperature -= 0.2f * temperature;
            moisture += 0.1f * moisture;
          } else if ( _meta.SurfaceTemperature < 60 ) {
            temperature -= 0.1f * temperature;
            moisture -= 0.2f * moisture;
          } else if ( _meta.SurfaceTemperature < 100 ) {
            temperature += 0.1f * temperature;
            moisture -= 0.4f * moisture;
          } else if ( _meta.SurfaceTemperature < 120 ) {
            temperature += 0.3f * temperature;
            moisture -= 0.7f * moisture;
          } else if ( _meta.SurfaceTemperature < 150 ) {
            temperature += 0.5f * temperature;
            moisture -= 0.9f * moisture;
          } else if ( _meta.SurfaceTemperature < 200 ) {
            temperature += 0.7f * temperature;
            moisture = 0;
          } else {
            moisture = 0;
            temperature += temperature;
          }

          // Adjust moisture based off planet type
          switch ( _meta.Type ) {
            case PlanetType.Water:
              moisture += 0.5f * moisture;
              elevation -= 0.3f * moisture;
              break;
            case PlanetType.Gas:
              moisture = 0;
              break;
          }

          // Generate the actual planet map
          _tiles[x, y] = new PlanetTile(elevation, moisture, temperature, rand);
        }
      }
      _generated = true; // finished
    }

    /// <summary>
    /// Creates the biome map texture and planet mask texture to use for rendering to star system view
    /// and to use as maps.
    /// </summary>
    /// <param name="content">Content manager for loading textures.</param>
    public void CreateMapTexture(ContentManager content)
    {
      // Check if already generated
      if ( _layers.ContainsKey("planet map") ) {
        return;
      }

      // Check if resources have been loaded
      if ( _planetMask == null ) {
        _planetMask = content.Load<Texture2D>("Images/planetmask");
      }

      // Generates a texture for the map
      var target = new RenderTarget2D(
        MBGame.Graphics,
        _width * _cellSize,
        _height * _cellSize
      );
      // Generates the 'sphere' version of the map using a texture gradient overlay
      var roundTarget = new RenderTarget2D(
        MBGame.Graphics,
        _width * _cellSize,
        _height * _cellSize
      );

      var spriteBatch = new SpriteBatch(MBGame.Graphics);

      spriteBatch.Begin();

      MBGame.Graphics.SetRenderTarget(target);
      MBGame.Graphics.Clear(Color.Transparent);

      // Draw the tiles to the regular square map
      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {
          var tile = _tiles[x, y];
          var clr = tile.TintColor;

          // Draw the tile
          spriteBatch.FillRectangle(
            x * _cellSize,
            y * _cellSize,
            _cellSize,
            _cellSize,
            clr
          );

        }
      }

      spriteBatch.End();

      MBGame.Graphics.SetRenderTarget(roundTarget);
      MBGame.Graphics.Clear(Color.Transparent);

      spriteBatch.Begin();

      // Setup clipping mask for drawing the 'sphere'
      var circleMask = new CircleF(
        target.Bounds.Center.ToVector2(),
        target.Bounds.Width / 2
      );

      // Draw the tiles to the 'sphere' version of the map
      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {
          var tile = _tiles[x, y];
          var clr = tile.TintColor;

          if ( circleMask.Contains(x * _cellSize, y * _cellSize) ) {
            // Draw the tile if within the circle
            spriteBatch.FillRectangle(
              x * _cellSize,
              y * _cellSize,
              _cellSize,
              _cellSize,
              clr
            );

          }

        }
      }

      // Setu pa temporary scale vector for drawing the planet
      var maskSize = target.Bounds.Size.ToVector2();
      maskSize.X += 5f;
      maskSize.Y += 5f;
      // Scale the actual texture to the mask
      var planetMaskScale = _planetMask.Bounds.Size.ToVector2().FitInto(maskSize);
      spriteBatch.Draw(_planetMask, new Vector2(-2f, -2f), scale: planetMaskScale);

      spriteBatch.End();

      MBGame.Graphics.SetRenderTarget(null);

      // Add layers for lookup
      _layers.Add("map", target);
      _layers.Add("planet map", roundTarget);
      //_map = target;
    }

    /// <summary>
    /// Generates a new noise map with a specified number of layers
    /// and density
    /// </summary>
    private void GenerateNoiseData()
    {
      //
      // Uses a 2D -> 4D mapping of each axis in the noise map to allow wrapping.
      // Adapted from Jon Gallants original technique.
      // Source: http://www.jgallant.com/procedurally-generating-wrapping-world-maps-in-unity-csharp-part-2/#wrap2
      //
      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {

          //Gets the noise range
          var x1 = 0;
          var x2 = 2;
          var y1 = 0;
          var y2 = 2;
          var dx = x2 - x1;
          var dy = y2 - y1;

          //Samples noise at smaller intervals for each iteration
          var s = x / (float)_width;
          var t = y / (float)_height;

          // Calculate 4D coordinates
          var nx = x1 + Math.Cos(s * 2 * Math.PI) * dx / (2 * Math.PI);
          var ny = y1 + Math.Cos(t * 2 * Math.PI) * dy / (2 * Math.PI);
          var nz = x1 + Math.Sin(s * 2 * Math.PI) * dx / (2 * Math.PI);
          var nw = y1 + Math.Sin(t * 2 * Math.PI) * dy / (2 * Math.PI);

          // Assign wrapped values to each noise map
          _elevation.SetValue(x, y, _elevation.Map.Get(nx, ny, nz, nw));
          _temperature.SetValue(x, y, _temperature.Map.Get(nx, ny, nz, nw));
          _moisture.SetValue(x, y, _moisture.Map.Get(nx, ny, nz, nw));
        }
      }

    }

    //private void CreateNoiseMapTexture(string layerName, NoiseMap map)
    //{
    //  if ( _layers.ContainsKey(layerName) ) {
    //    return;
    //  }

    //  _spriteBatch.Begin();
    //  // Generates a texture for the map
    //  var target = new RenderTarget2D(
    //    MBGame.Graphics,
    //    map.Width * _cellSize,
    //    map.Height * _cellSize
    //  );

    //  MBGame.Graphics.SetRenderTarget(target);
    //  MBGame.Graphics.Clear(Color.Black);

    //  for ( int x = 0; x < map.Width; x++ ) {
    //    for ( int y = 0; y < map.Height; y++ ) {
    //      var clr = Color.Lerp(Color.Black, Color.White, (float)map.GetValue(x, y));
    //      _spriteBatch.FillRectangle(
    //        x * _cellSize,
    //        y * _cellSize,
    //        _cellSize,
    //        _cellSize,
    //        clr
    //      );
    //    }
    //  }

    //  _spriteBatch.End();
    //  MBGame.Graphics.SetRenderTarget(null);

    //  _layers.Add(layerName, target);
    //  //_map = target;
    //}

    /// <summary>
    /// Gets one of the planets generated noise map textures
    /// </summary>
    /// <returns>The map layer.</returns>
    /// <param name="layerName">Layer name.</param>
    public Texture2D GetMapLayer(string layerName)
    {
      Texture2D result = null;
      if ( _layers.ContainsKey(layerName) ) {
        result = _layers[layerName];
      }
      return result;
    }

    /// <summary>
    /// Gets all the tiles in the generated planet
    /// </summary>
    /// <value>The tiles.</value>
    public PlanetTile[,] Tiles
    {
      get { return _tiles; }
    }

    /// <summary>
    /// Gets the rectangular size of the planets tile map
    /// </summary>
    /// <value>The size of the planet.</value>
    public Point Size
    {
      get { return new Point(_width, _height); }
    }

    /// <summary>
    /// Gets the name of the planet
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return _name; }
    }

    /// <summary>
    /// Gets the planets assigned metadata parameters
    /// </summary>
    /// <value>The metadata.</value>
    public PlanetMetadata Meta
    {
      get { return _meta; }
    }

    /// <summary>
    /// Gets or sets the planets position in the star system scene
    /// </summary>
    /// <value>The position.</value>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:MidnightBlue.Planet"/> is generated
    /// or only setup ready to be generated.
    /// </summary>
    /// <value><c>true</c> if generated; otherwise, <c>false</c>.</value>
    public bool Generated
    {
      get { return _generated; }
    }
  }
}
