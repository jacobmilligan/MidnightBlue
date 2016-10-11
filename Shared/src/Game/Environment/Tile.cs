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
  public class Tile
  {
    private HeightLevel _height;
    private MoistureLevel _moisture;
    private TemperatureLevel _temperature;
    private Biome _biome;

    public Tile(double height, double moisture, double temperature)
    {
      _height = EcosystemTool.GetHeight(height);
      _moisture = EcosystemTool.GetMoisture(moisture);
      _temperature = EcosystemTool.GetTemperature(temperature);
      _biome = EcosystemTool.GetBiome(Moisture, Temperature, Height);
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
