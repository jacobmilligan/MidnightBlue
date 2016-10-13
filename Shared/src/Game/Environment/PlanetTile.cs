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

namespace MidnightBlue
{
  public class PlanetTile : Tile
  {
    private Color _tundra = new Color(180, 220, 220);
    private Color _boreal = new Color(51, 142, 129);
    private Color _desert = new Color(229, 218, 135);
    private Color _shrubLand = new Color(143, 157, 113);
    private Color _woodland = new Color(5, 102, 33);
    private Color _temperateSeasonalForest = new Color(48, 116, 68);
    private Color _temperateRainforest = new Color(98, 139, 23);
    private Color _subtropicalDesert = new Color(255, 188, 64);
    private Color _savana = new Color(250, 240, 192);
    private Color _tropicalRainforest = new Color(83, 123, 9);
    private Color _temperateGrassland = new Color(141, 179, 96);
    private Color _ocean = new Color(16, 32, 79);
    private Color _shallows = new Color(17, 41, 89);

    private HeightLevel _height;
    private MoistureLevel _moisture;
    private TemperatureLevel _temperature;
    private Biome _biome;

    public PlanetTile(double height, double moisture, double temperature, Random rand)
    {
      _height = EcosystemTool.GetHeight(height);
      _moisture = EcosystemTool.GetMoisture(moisture);
      _temperature = EcosystemTool.GetTemperature(temperature);
      _biome = EcosystemTool.GetBiome(Moisture, Temperature, Height);

      ID = GetTextureID(_biome, rand);
      TintColor = GetColor(_biome);
    }

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
        case Biome.TemeperateGrassland:
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

    public HeightLevel Height
    {
      get { return _height; }
    }

    public MoistureLevel Moisture
    {
      get { return _moisture; }
    }

    public TemperatureLevel Temperature
    {
      get { return _temperature; }
    }

    public Biome Biome
    {
      get { return _biome; }
    }
  }
}
