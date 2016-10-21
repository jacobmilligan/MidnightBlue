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
  /// <summary>
  /// Planet type used for information.
  /// </summary>
  public enum PlanetType { Water, Terrestrial, Gas }

  /// <summary>
  /// Planet metadata used as information and arguments for generating
  /// the actual biome map of a planet. Required for an entity to be
  /// treated as a planet.
  /// </summary>
  public class PlanetMetadata : IComponent
  {
    /// <summary>
    /// Gets or sets the name of the planet.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the radius of the planet.
    /// </summary>
    /// <value>The radius.</value>
    public int Radius { get; internal set; }

    /// <summary>
    /// Gets or sets the type of the planet.
    /// </summary>
    /// <value>The type.</value>
    public PlanetType Type { get; set; }

    /// <summary>
    /// Gets or sets the surface temperature.
    /// </summary>
    /// <value>The surface temperature.</value>
    public float SurfaceTemperature { get; internal set; }

    /// <summary>
    /// Gets or sets the density.
    /// </summary>
    /// <value>The density.</value>
    public int Density { get; internal set; }

    /// <summary>
    /// Gets or sets the score indicating the planets ability to support life.
    /// </summary>
    /// <value>The life score.</value>
    public float Habitable { get; internal set; }

    /// <summary>
    /// Gets or sets the amount of carbon on the planet.
    /// </summary>
    /// <value>The carbon amount.</value>
    public int Carbon { get; internal set; }

    /// <summary>
    /// Gets or sets the amount of water on the planet.
    /// </summary>
    /// <value>The water amount.</value>
    public int Water { get; internal set; }

    /// <summary>
    /// Gets or sets the amount of gas on the planet.
    /// </summary>
    /// <value>The gas amount.</value>
    public int Gas { get; internal set; }

    /// <summary>
    /// Gets or sets the distance of this planet to its star
    /// </summary>
    /// <value>The star distance.</value>
    public Length StarDistance { get; internal set; }

    /// <summary>
    /// Gets the surface area of the planet. Used mostly for information displays - not
    /// very useful for anything else.
    /// </summary>
    /// <value>The surface area.</value>
    public float SurfaceArea
    {
      get { return 4 * MathHelper.Pi * (Radius * Radius); }
    }
  }
}
