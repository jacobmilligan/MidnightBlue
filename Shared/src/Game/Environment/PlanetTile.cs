//
// 	Tile.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;

namespace MidnightBlue
{
  /// <summary>
  /// A tile type used in planet tilemaps
  /// </summary>
  public class PlanetTile : Tile
  {
    /// <summary>
    /// Tundra minimap color
    /// </summary>
    private Color _tundra = new Color(180, 220, 220);
    /// <summary>
    /// Taiga minimap color
    /// </summary>
    private Color _boreal = new Color(51, 142, 129);
    /// <summary>
    /// Desert minimap color
    /// </summary>
    private Color _desert = new Color(229, 218, 135);
    /// <summary>
    /// Shrubland minimap color
    /// </summary>
    private Color _shrubLand = new Color(143, 157, 113);
    /// <summary>
    /// Woodland minimap color
    /// </summary>
    private Color _woodland = new Color(5, 102, 33);
    /// <summary>
    /// Temperate Seasonal Forest minimap color
    /// </summary>
    private Color _temperateSeasonalForest = new Color(48, 116, 68);
    /// <summary>
    /// Temperate Rainforest minimap color
    /// </summary>
    private Color _temperateRainforest = new Color(98, 139, 23);
    /// <summary>
    /// Sub-tropical desert minimap color
    /// </summary>
    private Color _subtropicalDesert = new Color(255, 188, 64);
    /// <summary>
    /// Savana minimap color
    /// </summary>
    private Color _savana = new Color(250, 240, 192);
    /// <summary>
    /// Tropical rainforest minimap color
    /// </summary>
    private Color _tropicalRainforest = new Color(83, 123, 9);
    /// <summary>
    /// Temperate Grassland minimap color
    /// </summary>
    private Color _temperateGrassland = new Color(141, 179, 96);
    /// <summary>
    /// Ocean minimap color
    /// </summary>
    private Color _ocean = new Color(16, 32, 79);
    /// <summary>
    /// Shallow ocean minimap color
    /// </summary>
    private Color _shallows = new Color(17, 41, 89);

    /// <summary>
    /// The tiles height category
    /// </summary>
    private HeightLevel _height;
    /// <summary>
    /// The tiles moisture category
    /// </summary>
    private MoistureLevel _moisture;
    /// <summary>
    /// The tiles temperature category
    /// </summary>
    private TemperatureLevel _temperature;
    /// <summary>
    /// The tiles associated biome type
    /// </summary>
    private Biome _biome;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.PlanetTile"/> class,
    /// generates a biome and specifies collision data based on its biome type.
    /// </summary>
    /// <param name="height">Height value to use in generation.</param>
    /// <param name="moisture">Moisture value to use in generation.</param>
    /// <param name="temperature">Temperature value to use in generation.</param>
    /// <param name="rand">Random number generator used in biome generation.</param>
    public PlanetTile(double height, double moisture, double temperature, Random rand)
    {
      _height = EcosystemTool.GetHeight(height);
      _moisture = EcosystemTool.GetMoisture(moisture);
      _temperature = EcosystemTool.GetTemperature(temperature);
      _biome = EcosystemTool.GetBiome(Moisture, Temperature, Height);

      if ( _biome == Biome.Ocean || _biome == Biome.ShallowOcean ) {
        Flag = TileFlag.Impassable;
      }

      ID = GetTextureID(_biome, rand);
      TintColor = GetColor(_biome);
    }

    /// <summary>
    /// Gets the ID of a biomes texture region for use in a tilemap
    /// </summary>
    /// <returns>The texture region identifier.</returns>
    /// <param name="biome">Biome to get.</param>
    /// <param name="rand">Random number generator to use for varied region id's.</param>
    public int GetTextureID(Biome biome, Random rand)
    {
      var id = 3;
      switch ( biome ) {
        case Biome.Tundra:
        case Biome.Taiga:
          id = 499;
          break;
        case Biome.Woodland:
          id = rand.Next(358, 360);
          break;
        case Biome.Shrubland:
          id = 298;
          break;
        case Biome.TemperateGrassland:
          id = 301;
          break;
        case Biome.Desert:
          id = rand.Next(370, 372);
          break;
        case Biome.SubtropicalDesert:
          id = rand.Next(160, 162);
          break;
        case Biome.Savana:
          id = 304;
          break;
        case Biome.TemperateSeasonalForest:
          id = rand.Next(352, 354);
          break;
        case Biome.TemperateRainforest:
          id = rand.Next(364, 366);
          break;
        case Biome.TropicalRainforest:
          id = rand.Next(364, 366);
          break;
        case Biome.Barren:
          id = rand.Next(172, 174);
          break;
        case Biome.ShallowOcean:
          var possibleIds = new int[] { 124, 124, 124, 187, 188, 189 };
          var randId = rand.Next(0, rand.Next(possibleIds.Length - 1));
          id = possibleIds[randId];
          break;
        case Biome.Ocean:
          possibleIds = new int[] { 869, 869, 869, 932, 933, 934 };
          randId = rand.Next(0, rand.Next(possibleIds.Length - 1));
          id = possibleIds[randId];
          break;
        case Biome.Ice:
          id = 499;
          break;
        default:
          break;
      }

      return id;
    }

    /// <summary>
    /// Gets the color of a tile in the minimap based on its biome.
    /// </summary>
    /// <returns>The color.</returns>
    /// <param name="biome">Biome to get.</param>
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
        case Biome.TemperateGrassland:
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

    /// <summary>
    /// Gets the height category of the tile.
    /// </summary>
    /// <value>The height category.</value>
    public HeightLevel Height
    {
      get { return _height; }
    }

    /// <summary>
    /// Gets the moisture category of the tile.
    /// </summary>
    /// <value>The moisture category.</value>
    public MoistureLevel Moisture
    {
      get { return _moisture; }
    }

    /// <summary>
    /// Gets the temperature category of the tile.
    /// </summary>
    /// <value>The temperature category.</value>
    public TemperatureLevel Temperature
    {
      get { return _temperature; }
    }

    /// <summary>
    /// Gets the biome category of the tile.
    /// </summary>
    /// <value>The biome category.</value>
    public Biome Biome
    {
      get { return _biome; }
    }
  }
}
