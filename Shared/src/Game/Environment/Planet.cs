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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine;
using MonoGame.Extended.Shapes;
using TinkerWorX.AccidentalNoiseLibrary;

namespace MidnightBlue
{
  public class Planet
  {

    private const int _cellSize = 1;

    private Color _tundra = new Color(185, 214, 227);
    private Color _boreal = new Color(153, 180, 157);
    private Color _desert = new Color(250, 214, 156);
    private Color _shrubLand = new Color(194, 111, 80);
    private Color _woodland = new Color(53, 130, 71);
    private Color _temperateSeasonalForest = new Color(174, 210, 0);
    private Color _temperateRainforest = new Color(126, 162, 119);
    private Color _subtropicalDesert = new Color(222, 188, 114);
    private Color _savana = new Color(157, 156, 53);
    private Color _tropicalRainforest = new Color(55, 113, 71);
    private Color _temperateGrassland = new Color(126, 138, 50);
    private Color _ocean = new Color(17, 30, 82);
    private Color _shallows = new Color(11, 74, 130);

    private int _width, _height;
    private bool _generated;
    private string _name;
    private NoiseMap _elevation, _temperature, _moisture;
    private PlanetMetadata _meta;
    private Dictionary<string, Texture2D> _layers;
    private Texture2D _planetMask;

    private Tile[,] _tiles;

    public Planet(PlanetMetadata meta, int seed)
    {
      _meta = meta;
      _name = _meta.Name;
      _generated = false;

      var scaledRadius = (MathHelper.Pi * _meta.Radius * 2) / 10000;
      scaledRadius = scaledRadius + (scaledRadius / 2);
      if ( scaledRadius > 1050 ) {
        scaledRadius = 1050;
      }
      _height = _width = (int)scaledRadius;

      _tiles = new Tile[_width, _height];

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

      var combiner = new ImplicitCombiner(CombinerType.Multiply);
      combiner.Seed = seed;
      combiner.AddSource(gradient);
      combiner.AddSource(temperatureMap);

      _temperature = new NoiseMap(combiner, _width, _height);

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

    public void Generate()
    {
      GenerateNoiseData();
      //CreateNoiseMapTexture("elevation", _elevation);
      //CreateNoiseMapTexture("moisture", _temperature);
      //CreateNoiseMapTexture("temperature", _temperature);

      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {
          var temperature = _temperature.GetValue(x, y);
          var moisture = _moisture.GetValue(x, y);
          var elevation = _elevation.GetValue(x, y);

          // Adjust values based of generated average surface temperature
          // from galaxy view.
          if ( _meta.SurfaceTemperature < -40 ) {
            temperature -= temperature;
          } else if ( _meta.SurfaceTemperature < -20 ) {
            temperature -= 0.8f * temperature;
          } else if ( _meta.SurfaceTemperature < 0 ) {
            temperature -= 0.5f * temperature;
            moisture += 0.3f * moisture;
          } else if ( _meta.SurfaceTemperature < 5 ) {
            temperature -= 0.4f * temperature;
            moisture += 0.3f * moisture;
          } else if ( _meta.SurfaceTemperature < 10 ) {
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


          _tiles[x, y] = new Tile(elevation, moisture, temperature);
        }
      }
      _generated = true;
    }

    public void CreateMapTexture(ContentManager content)
    {
      if ( _layers.ContainsKey("planet map") ) {
        return;
      }

      if ( _planetMask == null ) {
        _planetMask = content.Load<Texture2D>("Images/planetmask");
      }

      // Generates a texture for the map
      var target = new RenderTarget2D(
        MBGame.Graphics,
        _width * _cellSize,
        _height * _cellSize
      );
      var roundTarget = new RenderTarget2D(
        MBGame.Graphics,
        _width * _cellSize,
        _height * _cellSize
      );

      var spriteBatch = new SpriteBatch(MBGame.Graphics);

      spriteBatch.Begin();

      MBGame.Graphics.SetRenderTarget(target);
      MBGame.Graphics.Clear(Color.Transparent);

      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {
          var tile = _tiles[x, y];
          var clr = GetColor(tile.Biome);
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

      var circleMask = new CircleF(
        target.Bounds.Center.ToVector2(),
        target.Bounds.Width / 2
      );

      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {
          var tile = _tiles[x, y];
          var clr = GetColor(tile.Biome);
          if ( circleMask.Contains(x * _cellSize, y * _cellSize) ) {
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

      var maskSize = target.Bounds.Size.ToVector2();
      maskSize.X += 5f;
      maskSize.Y += 5f;
      var planetMaskScale = _planetMask.Bounds.Size.ToVector2().FitInto(maskSize);
      spriteBatch.Draw(_planetMask, new Vector2(-2f, -2f), scale: planetMaskScale);

      spriteBatch.End();

      MBGame.Graphics.SetRenderTarget(null);

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

    public Texture2D GetMapLayer(string layerName)
    {
      Texture2D result = null;
      if ( _layers.ContainsKey(layerName) ) {
        result = _layers[layerName];
      }
      return result;
    }

    public Color GetColor(Biome biome)
    {
      Color clr = Color.Transparent;

      switch ( biome ) {
        case Biome.Tundra:
          clr = _tundra;
          break;
        case Biome.Taiga:
          clr = _boreal;
          break;
        case Biome.Woodland:
          clr = _woodland;
          break;
        case Biome.Shrubland:
          clr = _shrubLand;
          break;
        case Biome.TemeperateGrassland:
          clr = _temperateGrassland;
          break;
        case Biome.Desert:
          clr = _desert;
          break;
        case Biome.SubtropicalDesert:
          clr = _subtropicalDesert;
          break;
        case Biome.Savana:
          clr = _savana;
          break;
        case Biome.TemperateSeasonalForest:
          clr = _temperateSeasonalForest;
          break;
        case Biome.TemperateRainforest:
          clr = _temperateRainforest;
          break;
        case Biome.TropicalRainforest:
          clr = _tropicalRainforest;
          break;
        case Biome.Barren:
          clr = Color.Gray;
          break;
        case Biome.ShallowOcean:
          clr = _shallows;
          break;
        case Biome.Ocean:
          clr = _ocean;
          break;
        case Biome.Ice:
          clr = Color.White;
          break;
        default:
          break;
      }

      return clr;
    }

    public Tile[,] Tiles
    {
      get { return _tiles; }
    }

    public Point Size
    {
      get { return new Point(_width, _height); }
    }

    public string Name
    {
      get { return _name; }
    }

    public PlanetMetadata Meta
    {
      get { return _meta; }
    }

    public Vector2 Position { get; set; }

    public bool Generated
    {
      get { return _generated; }
    }
  }
}
