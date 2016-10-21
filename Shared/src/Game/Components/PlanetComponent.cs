//
// 	PlanetComponent.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 12/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{
  /// <summary>
  /// Represents a planet entity with pre-generated metadata
  /// </summary>
  public class PlanetComponent : IComponent
  {
    /// <summary>
    /// All pre-generated arguments used when generating a planets map
    /// </summary>
    /// <value>The data.</value>
    public Planet Data { get; set; }
  }
}
