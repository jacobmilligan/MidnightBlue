//
// 	StarSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{
  public class StarSystem : IComponent
  {
    private StringBuilder _stringBuilder;

    public StarSystem()
    {
      _stringBuilder = new StringBuilder();
      Planets = new List<Planet>();
    }

    public Color Color { get; set; }
    public Rectangle Bounds { get; set; }
    public string Name { get; set; }
    public int Radius { get; set; }
    public bool Draw { get; set; }
    public bool Scanned { get; set; }
    public string PlanetList
    {
      get
      {
        var numPlanets = Planets.Count;
        _stringBuilder.Clear();
        for ( int p = 0; p < numPlanets; p++ ) {
          _stringBuilder.AppendLine("- " + Planets[p].Name);
        }
        return _stringBuilder.ToString();
      }
    }
    public List<Planet> Planets { get; set; }
  }
}
