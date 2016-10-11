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
    private Color _woodland = new Color(194, 111, 80);
    private Color _temperateForest = new Color(145, 172, 121);
    private Color _temperateRainforest = new Color(126, 162, 119);
    private Color _subtropicalDesert = new Color(222, 188, 114);
    private Color _savana = new Color(157, 156, 53);
    private Color _tropicalRainforest = new Color(55, 113, 71);
    private Color _ocean = new Color(17, 46, 76);
    private Color _shallows = new Color(0, 150, 231);

    private int _width, _height;
    private string _name;
    private NoiseMap _elevation, _temperature, _moisture;
    private PlanetMetadata _meta;
    private Dictionary<string, Texture2D> _layers;

    private Tile[,] _tiles;

    public Planet(PlanetMetadata meta, int seed)
    {
      _meta = meta;
      _name = _meta.Name;

      var scaledRadius = (MathHelper.Pi * _meta.Radius * 2) / 1000;
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
      temperatureMap.Frequency = 4;
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
      moistureMap.Octaves = 4;
      moistureMap.Frequency = 3.0;
      moistureMap.Seed = seed;
      _moisture = new NoiseMap(moistureMap, _width, _height);

      _layers = new Dictionary<string, Texture2D>();
    }

    public void Generate()
    {
      var elevationThread = new Thread(new ThreadStart(_elevation.Generate));
      //_elevation.Generate();
      //CreateNoiseMapTexture("elevation", _elevation);

      var moistureThread = new Thread(new ThreadStart(_moisture.Generate));
      //_moisture.Generate();
      //CreateNoiseMapTexture("moisture", _temperature);

      var temperatureThread = new Thread(new ThreadStart(_temperature.Generate));
      //_temperature.Generate();
      //CreateNoiseMapTexture("temperature", _temperature);

      elevationThread.Start();
      moistureThread.Start();
      temperatureThread.Start();

      elevationThread.Join();
      moistureThread.Join();
      temperatureThread.Join();

      for ( int x = 0; x < _width; x++ ) {
        for ( int y = 0; y < _height; y++ ) {
          var temperature = _temperature.GetValue(x, y);
          var moisture = _moisture.GetValue(x, y);
          var elevation = _elevation.GetValue(x, y);

          // Adjust moisture based off planet type
          switch ( _meta.Type ) {
            case PlanetType.Water:
              moisture += 0.5f * moisture;
              elevation -= 0.3f * moisture;
              break;
            case PlanetType.Terrestrial:
              moisture += 0.1f * moisture;
              break;
            case PlanetType.Gas:
              moisture = 0;
              break;
          }

          // Adjust values based of generated average surface temperature
          // from galaxy view.
          if ( _meta.SurfaceTemperature < -100 ) {
            temperature -= 0.9f * temperature;
          } else if ( _meta.SurfaceTemperature < -80 ) {
            temperature -= 0.8f * temperature;
          } else if ( _meta.SurfaceTemperature < -60 ) {
            temperature -= 0.7f * temperature;
          } else if ( _meta.SurfaceTemperature < -40 ) {
            temperature -= 0.6f * temperature;
          } else if ( _meta.SurfaceTemperature < -20 ) {
            temperature -= 0.5f * temperature;
          } else if ( _meta.SurfaceTemperature < 0 ) {
            temperature -= 0.4f * temperature;
          } else if ( _meta.SurfaceTemperature < 5 ) {
            temperature -= 0.3f * temperature;
          } else if ( _meta.SurfaceTemperature < 10 ) {
            temperature -= 0.2f * temperature;
          } else if ( _meta.SurfaceTemperature < 40 ) {
            temperature -= 0.1f * temperature;
          } else if ( _meta.SurfaceTemperature < 60 ) {
            temperature += 0.1f * temperature;
          } else if ( _meta.SurfaceTemperature < 100 ) {
            temperature += 0.2f * temperature;
          } else if ( _meta.SurfaceTemperature < 120 ) {
            temperature += 0.3f * temperature;
          } else if ( _meta.SurfaceTemperature < 150 ) {
            temperature += 0.4f * temperature;
          } else if ( _meta.SurfaceTemperature < 200 ) {
            temperature += 0.5f * temperature;
          }

          _tiles[x, y] = new Tile(elevation, moisture, temperature);
        }
      }
    }

    public void CreateMapTexture(SpriteBatch spriteBatch)
    {
      if ( _layers.ContainsKey("map") ) {
        return;
      }
      spriteBatch.End();
      spriteBatch.Begin();

      // Generates a texture for the map
      var target = new RenderTarget2D(
        MBGame.Graphics,
        _width * _cellSize,
        _height * _cellSize
      );

      MBGame.Graphics.SetRenderTarget(target);
      MBGame.Graphics.Clear(Color.Black);

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
      MBGame.Graphics.SetRenderTarget(null);

      _layers.Add("map", target);
      //_map = target;
      spriteBatch.Begin();
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

    private Color GetColor(Biome biome)
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
          clr = _savana;
          break;
        case Biome.TemeperateGrassland:
          clr = _desert;
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
        case Biome.TropicalSeasonalForest:
          clr = _savana;
          clr.G += 10;
          break;
        case Biome.TemperateSeasonalForest:
          clr = _temperateForest;
          clr = _woodland;
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

    public Point Size
    {
      get { return new Point(_width, _height); }
    }

    public string Name
    {
      get { return _name; }
    }
  }
}
