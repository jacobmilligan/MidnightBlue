//
// 	Planet.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{

  public enum PlanetType { Water, Terrestrial, Gas }

  public class PlanetMetadata : IComponent
  {
    public string Name { get; set; }
    public int Radius { get; internal set; }
    public PlanetType Type { get; set; }
    public float SurfaceTemperature { get; internal set; }
    public int Density { get; internal set; }
    public float Habitable { get; internal set; }
    public int Carbon { get; internal set; }
    public int Water { get; internal set; }
    public int Gas { get; internal set; }
    public Length StarDistance { get; internal set; }

    public float SurfaceArea
    {
      get { return 4 * MathHelper.Pi * (Radius * Radius); }
    }
  }
}
