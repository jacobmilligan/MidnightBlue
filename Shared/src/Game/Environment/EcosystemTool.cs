//
// 	Ecosystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
namespace MidnightBlue
{

  public enum Biome
  {
    Tundra,
    Taiga,
    Woodland,
    Shrubland,
    TemeperateGrassland,
    Desert,
    SubtropicalDesert,
    Savana,
    TropicalSeasonalForest,
    TemperateSeasonalForest,
    TemperateRainforest,
    TropicalRainforest,
    Barren,
    ShallowOcean,
    Ocean,
    Ice
  }

  public enum HeightLevel
  {
    Depths,
    SeaLevel,
    Lowland,
    Mountainous,
    Alpine,
    Snow
  }

  public enum MoistureLevel
  {
    Arid,
    Dry,
    SemiDry,
    SemiMoist,
    Moist,
    Wet
  }

  public enum TemperatureLevel
  {
    Freezing,
    Polar,
    Tundra,
    Taiga,
    Temperate,
    SubTropical,
    Tropical,
    Hot,
    Harsh,
    SuperHot,
    Scorching
  }

  public static class EcosystemTool
  {

    internal static Biome GetBiome(MoistureLevel moisture, TemperatureLevel temperature, HeightLevel height)
    {
      var biome = Biome.Barren;

      if ( temperature == TemperatureLevel.Freezing ) {
        biome = Biome.Ice;
      }
      if ( temperature == TemperatureLevel.Polar || temperature == TemperatureLevel.Tundra ) {
        biome = Biome.Tundra;
      }
      if ( temperature == TemperatureLevel.Taiga ) {
        switch ( moisture ) {
          case MoistureLevel.Arid:
            biome = Biome.TemeperateGrassland;
            break;
          case MoistureLevel.Dry:
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
            biome = Biome.Savana; // TODO: 50/50 chance this or Tropical seasonal forest
            break;
          case MoistureLevel.SemiMoist:
            biome = Biome.Savana; // TODO: 50/50 chance this or Tropical seasonal forest
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
          case MoistureLevel.Moist:
            biome = Biome.Barren;
            break;
          case MoistureLevel.Wet:
            biome = Biome.Desert;
            break;
        }
      }

      if ( temperature == TemperatureLevel.Scorching ) {
        biome = Biome.Barren;
      }

      if ( height == HeightLevel.Depths ) {
        biome = Biome.Ocean;
      }
      if ( height == HeightLevel.SeaLevel ) {
        biome = Biome.ShallowOcean;
      }

      return biome;
    }

    internal static HeightLevel GetHeight(double height)
    {
      var heightLevel = HeightLevel.Depths;

      if ( height >= 0.24 && height < 0.32 ) {
        heightLevel = HeightLevel.SeaLevel;
      } else if ( height >= 0.32 && height < 0.48 ) {
        heightLevel = HeightLevel.Lowland;
      } else if ( height >= 0.48 && height < 0.64 ) {
        heightLevel = HeightLevel.Mountainous;
      } else if ( height >= 0.64 && height < 0.80 ) {
        heightLevel = HeightLevel.Alpine;
      } else if ( height >= 80 ) {
        heightLevel = HeightLevel.Snow;
      }

      return heightLevel;
    }

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

    internal static TemperatureLevel GetTemperature(double temperature)
    {
      var tempLevel = TemperatureLevel.Freezing;

      if ( temperature >= 0.09 && temperature < 0.18 ) {
        tempLevel = TemperatureLevel.Polar;
      } else if ( temperature >= 0.18 && temperature < 0.27 ) {
        tempLevel = TemperatureLevel.Tundra;
      } else if ( temperature >= 0.27 && temperature < 0.36 ) {
        tempLevel = TemperatureLevel.Taiga;
      } else if ( temperature >= 0.36 && temperature < 0.45 ) {
        tempLevel = TemperatureLevel.Temperate;
      } else if ( temperature >= 0.45 && temperature < 0.54 ) {
        tempLevel = TemperatureLevel.SubTropical;
      } else if ( temperature >= 0.54 && temperature < 0.63 ) {
        tempLevel = TemperatureLevel.Tropical;
      } else if ( temperature >= 0.63 && temperature < 0.72 ) {
        tempLevel = TemperatureLevel.Hot;
      } else if ( temperature >= 0.72 && temperature < 0.81 ) {
        tempLevel = TemperatureLevel.Harsh;
      } else if ( temperature >= 0.81 && temperature < 0.90 ) {
        tempLevel = TemperatureLevel.SuperHot;
      } else if ( temperature >= 0.90 ) {
        tempLevel = TemperatureLevel.Scorching;
      }

      return tempLevel;
    }
  }
}
