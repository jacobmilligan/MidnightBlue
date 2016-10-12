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
  public class PlanetComponent : IComponent
  {
    public Planet Data { get; set; }
  }
}
