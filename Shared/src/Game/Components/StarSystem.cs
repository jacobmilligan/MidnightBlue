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
using MB2D.EntityComponent;

namespace MidnightBlue
{
  /// <summary>
  /// Represents an star system entity to be used in the galaxy view
  /// </summary>
  public class StarSystem : IComponent
  {
    /// <summary>
    /// Used to generate the list of planets and information inside the star system.
    /// </summary>
    private StringBuilder _stringBuilder;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.StarSystem"/> class.
    /// </summary>
    public StarSystem()
    {
      _stringBuilder = new StringBuilder();
      Planets = new List<PlanetMetadata>();
    }

    /// <summary>
    /// Gets or sets the color of the star rendered in the galaxy view.
    /// </summary>
    /// <value>The color.</value>
    public Color Color { get; set; }

    /// <summary>
    /// Gets or sets the bounding rectangle of the star system in the galaxy view.
    /// </summary>
    /// <value>The bounds.</value>
    public Rectangle Bounds { get; set; }

    /// <summary>
    /// Gets or sets the name of the star system.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the radius of the star at the center of the system.
    /// </summary>
    /// <value>The radius.</value>
    public int Radius { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:MidnightBlue.StarSystem"/> is drawn or not.
    /// </summary>
    /// <value><c>true</c> if should be drawn; otherwise, <c>false</c>.</value>
    public bool Draw { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:MidnightBlue.StarSystem"/> has been scanned
    /// by the player.
    /// </summary>
    /// <value><c>true</c> if scanned; otherwise, <c>false</c>.</value>
    public bool Scanned { get; set; }

    /// <summary>
    /// Gets a string representation of the list of planets in the star system and all their information.
    /// </summary>
    /// <value>The planet list.</value>
    public string PlanetList
    {
      get
      {
        var numPlanets = Planets.Count;
        _stringBuilder.Clear();

        // Build the string list and append information
        for ( int p = 0; p < numPlanets; p++ ) {
          _stringBuilder.AppendLine("- " + Planets[p].Name);
        }
        return _stringBuilder.ToString();
      }
    }

    /// <summary>
    /// Gets or sets the list of all planets.
    /// </summary>
    /// <value>The planets.</value>
    public List<PlanetMetadata> Planets { get; set; }
  }
}
