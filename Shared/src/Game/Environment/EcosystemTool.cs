//
// 	Ecosystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved

//
//  Admittedly this would have been better implemented using JSON but I
//  didn't have time and this is super fast compared to having to read from
//  a data file
//

namespace MidnightBlue
{
  /// <summary>
  /// Represents a biome type in a planets tile map.
  /// </summary>
  public enum Biome
  {
    /// <summary>
    /// Super cold environment.
    /// </summary>
    Tundra,
    /// <summary>
    /// Most common cold environment.
    /// </summary>
    Taiga,
    /// <summary>
    /// Cold forest biome.
    /// </summary>
    Woodland,
    /// <summary>
    /// Shrubland - can be cold or hot.
    /// </summary>
    Shrubland,
    /// <summary>
    /// Most common temperate biome.
    /// </summary>
    TemperateGrassland,
    /// <summary>
    /// Desert biome.
    /// </summary>
    Desert,
    /// <summary>
    /// Desert biome akin to southern californian deserts.
    /// </summary>
    SubtropicalDesert,
    /// <summary>
    /// Savana biome - flatlands with grass.
    /// </summary>
    Savana,
    /// <summary>
    /// Biome similar to Australian outback forests.
    /// </summary>
    TropicalSeasonalForest,
    /// <summary>
    /// Biome similar to canadian redwood forests.
    /// </summary>
    TemperateSeasonalForest,
    /// <summary>
    /// Biome like southern Australian rainforests - colder environment rainforest.
    /// </summary>
    TemperateRainforest,
    /// <summary>
    /// Classic rainforest biome.
    /// </summary>
    TropicalRainforest,
    /// <summary>
    /// Completely cracked, barren environment most present in inhospitable location.
    /// </summary>
    Barren,
    /// <summary>
    /// Shallow water biome - lighter in color than deep water.
    /// </summary>
    ShallowOcean,
    /// <summary>
    /// Deep ocean biome.
    /// </summary>
    Ocean,
    /// <summary>
    /// Ice biome - mostly present in inhospitable cold planets.
    /// </summary>
    Ice
  }

  /// <summary>
  /// Represents height categories for biomes.
  /// </summary>
  public enum HeightLevel
  {
    /// <summary>
    /// Will always be ocean biome unless super cold where it will be ice.
    /// </summary>
    Depths = 0,
    /// <summary>
    /// Will always be shallow ocean biome unless super cold where it will be ice.
    /// </summary>
    SeaLevel,
    /// <summary>
    /// Will contain all forest and grassland biomes.
    /// </summary>
    Lowland,
    /// <summary>
    /// Will contain all mountaineous biomes - will be colder than other biomes, too.
    /// </summary>
    Mountainous,
    /// <summary>
    /// Will contain all mountaineous biomes - will be much colder than other biomes, too.
    /// </summary>
    Alpine,
    /// <summary>
    /// Heighest level of elevation, super cold and only mountaineous biomes.
    /// </summary>
    Snow
  }

  /// <summary>
  /// Represents moisture categories for biomes used in generation.
  /// </summary>
  public enum MoistureLevel
  {
    Arid = 0,
    Dry,
    SemiDry,
    SemiMoist,
    Moist,
    Wet
  }

  /// <summary>
  /// Represents categories of temperature used in generating biomes and player interactions.
  /// </summary>
  public enum TemperatureLevel
  {
    /// <summary>
    /// Super cold - only on uninhabitable planets
    /// </summary>
    Freezing = 0,
    /// <summary>
    /// Coldest temperature found on inhabitable planets.
    /// </summary>
    Polar,
    /// <summary>
    /// Coldest temperature to produce varied biomes.
    /// </summary>
    Tundra,
    /// <summary>
    /// Most common cold temperature across inhabitable planets.
    /// </summary>
    Taiga,
    /// <summary>
    /// Most inhabitable temperature that produces the most varied biomes.
    /// </summary>
    Temperate,
    /// <summary>
    /// Temperature most commonly found for 'hot' biomes.
    /// </summary>
    SubTropical,
    /// <summary>
    /// Produces simultaneously very moist, green biomes, and very dry, arid biomes.
    /// </summary>
    Tropical,
    /// <summary>
    /// Hottest inhabitable temperature.
    /// </summary>
    Hot,
    /// <summary>
    /// Hottest temperature to produce varied biomes.
    /// </summary>
    Harsh,
    /// <summary>
    /// Will only produce barren or desert biomes - no water
    /// </summary>
    SuperHot,
    /// <summary>
    /// Will only produce barren biomes - no water. This is like 200+ degrees
    /// </summary>
    Scorching
  }

  /// <summary>
  /// Takes numeric inputs and produces temperature, height, and moisture categories. Also produces biomes based on
  /// the three outputs.
  /// </summary>
  public static class EcosystemTool
  {
    /// <summary>
    /// Produces a biome based on temperature, moisture, and height categories produced from other generation
    /// algorithms.
    /// </summary>
    /// <returns>The biome.</returns>
    /// <param name="moisture">Pre-generated moisture category.</param>
    /// <param name="temperature">Pre-generated temperature category.</param>
    /// <param name="height">Pre-generated height category.</param>
    internal static Biome GetBiome(MoistureLevel moisture, TemperatureLevel temperature, HeightLevel height)
    {
      var biome = Biome.TemperateGrassland;

      if ( temperature == TemperatureLevel.Freezing ) {
        biome = Biome.Ice;
      }
      if ( temperature == TemperatureLevel.Polar || temperature == TemperatureLevel.Tundra ) {
        biome = Biome.Tundra;
      }
      if ( temperature == TemperatureLevel.Taiga ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
            biome = Biome.Tundra;
            break;
          case MoistureLevel.Wet:
            biome = Biome.Woodland;
            break;
          default:
            biome = Biome.Taiga;
            break;
        }
      }
      if ( temperature == TemperatureLevel.Temperate ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
            biome = Biome.Desert;
            break;
          case MoistureLevel.Dry:
          case MoistureLevel.SemiDry:
            biome = Biome.TemperateGrassland;
            break;
          case MoistureLevel.SemiMoist:
            biome = Biome.Woodland;
            break;
          case MoistureLevel.Moist:
            biome = Biome.TemperateSeasonalForest;
            break;
          default:
            biome = Biome.TemperateRainforest;
            break;
        }
      }
      if ( temperature == TemperatureLevel.SubTropical ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
            biome = Biome.SubtropicalDesert;
            break;
          case MoistureLevel.Dry:
            biome = Biome.Shrubland;
            break;
          case MoistureLevel.SemiDry:
            biome = Biome.Woodland;
            break;
          case MoistureLevel.SemiMoist:
          case MoistureLevel.Moist:
            biome = Biome.TemperateSeasonalForest;
            break;
          default:
            biome = Biome.TemperateRainforest;
            break;
        }
      }
      if ( temperature == TemperatureLevel.Tropical ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
          case MoistureLevel.Dry:
            biome = Biome.SubtropicalDesert;
            break;
          case MoistureLevel.SemiDry:
            biome = Biome.Savana;
            break;
          case MoistureLevel.SemiMoist:
            biome = Biome.Savana;
            break;
          default:
            biome = Biome.TropicalRainforest;
            break;
        }
      }
      if ( temperature == TemperatureLevel.Hot ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
          case MoistureLevel.Dry:
          case MoistureLevel.SemiDry:
            biome = Biome.Desert;
            break;
          case MoistureLevel.SemiMoist:
          case MoistureLevel.Moist:
            biome = Biome.SubtropicalDesert;
            break;
          default:
            biome = Biome.TropicalSeasonalForest;
            break;
        }
      }
      if ( temperature == TemperatureLevel.Harsh ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
            biome = Biome.Barren;
            break;
          case MoistureLevel.Dry:
          case MoistureLevel.SemiDry:
          case MoistureLevel.SemiMoist:
          case MoistureLevel.Moist:
            biome = Biome.Desert;
            break;
          default:
            biome = Biome.SubtropicalDesert;
            break;
        }
      }
      if ( temperature == TemperatureLevel.SuperHot ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
          case MoistureLevel.Dry:
          case MoistureLevel.SemiDry:
          case MoistureLevel.SemiMoist:
            biome = Biome.Barren;
            break;
          case MoistureLevel.Moist:
          case MoistureLevel.Wet:
            biome = Biome.Desert;
            break;
        }
      }

      if ( temperature == TemperatureLevel.Scorching ) {
        if ( moisture == MoistureLevel.Wet || moisture == MoistureLevel.Moist ) {
          biome = Biome.Desert;
        } else {
          biome = Biome.Barren;
        }
      }

      if ( height == HeightLevel.Depths && moisture > MoistureLevel.Arid ) {
        biome = Biome.Ocean;
      }
      if ( height == HeightLevel.SeaLevel && moisture > MoistureLevel.Arid ) {
        biome = Biome.ShallowOcean;
      }

      return biome;
    }

    /// <summary>
    /// Gets a height category for a given numeric height value generated from
    /// other algorithms.
    /// </summary>
    /// <returns>The height category.</returns>
    /// <param name="height">Pre-generated numeric height value.</param>
    internal static HeightLevel GetHeight(double height)
    {
      var heightLevel = HeightLevel.Depths;

      if ( height >= 0.30 && height < 0.35 ) {
        heightLevel = HeightLevel.SeaLevel;
      } else if ( height >= 0.35 && height < 0.6 ) {
        heightLevel = HeightLevel.Lowland;
      } else if ( height >= 0.6 && height < 0.7 ) {
        heightLevel = HeightLevel.Mountainous;
      } else if ( height >= 0.70 && height < 0.80 ) {
        heightLevel = HeightLevel.Alpine;
      } else if ( height >= 80 ) {
        heightLevel = HeightLevel.Snow;
      }

      return heightLevel;
    }

    /// <summary>
    /// Gets a height category for a given numeric moisture value generated from
    /// other algorithms.
    /// </summary>
    /// <returns>The moisture category.</returns>
    /// <param name="moisture">Pre-generated numeric moisture value.</param>
    internal static MoistureLevel GetMoisture(double moisture)
    {
      var moistureLevel = MoistureLevel.Arid;

      if ( moisture >= 0.16 && moisture < 0.32 ) {
        moistureLevel = MoistureLevel.Dry;
      } else if ( moisture >= 0.32 && moisture < 0.48 ) {
        moistureLevel = MoistureLevel.SemiDry;
      } else if ( moisture >= 0.48 && moisture < 0.64 ) {
        moistureLevel = MoistureLevel.SemiMoist;
      } else if ( moisture >= 0.64 && moisture < 0.80 ) {
        moistureLevel = MoistureLevel.Moist;
      } else if ( moisture >= 0.80 ) {
        moistureLevel = MoistureLevel.Wet;
      }

      return moistureLevel;
    }

    /// <summary>
    /// Gets a temperature category for a given numeric height value generated from
    /// other algorithms.
    /// </summary>
    /// <returns>The temperature category.</returns>
    /// <param name="temperature">Pre-generated numeric temperature value.</param>
    internal static TemperatureLevel GetTemperature(double temperature)
    {
      var tempLevel = TemperatureLevel.Freezing;

      if ( temperature >= 0.05 && temperature < 0.10 ) {
        tempLevel = TemperatureLevel.Polar;
      } else if ( temperature >= 0.10 && temperature < 0.15 ) {
        tempLevel = TemperatureLevel.Tundra;
      } else if ( temperature >= 0.15 && temperature < 0.20 ) {
        tempLevel = TemperatureLevel.Taiga;
      } else if ( temperature >= 0.30 && temperature < 0.50 ) {
        tempLevel = TemperatureLevel.Temperate;
      } else if ( temperature >= 0.50 && temperature < 0.60 ) {
        tempLevel = TemperatureLevel.SubTropical;
      } else if ( temperature >= 0.60 && temperature < 0.70 ) {
        tempLevel = TemperatureLevel.Tropical;
      } else if ( temperature >= 0.70 && temperature < 0.90 ) {
        tempLevel = TemperatureLevel.Hot;
      } else if ( temperature >= 0.90 && temperature < 1.1 ) {
        tempLevel = TemperatureLevel.Harsh;
      } else if ( temperature >= 1.1 && temperature < 1.5 ) {
        tempLevel = TemperatureLevel.SuperHot;
      } else if ( temperature >= 1.5 ) {
        tempLevel = TemperatureLevel.Scorching;
      }

      return tempLevel;
    }
  }
}
