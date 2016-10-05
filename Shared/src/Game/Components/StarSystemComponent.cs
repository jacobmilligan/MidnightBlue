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
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{
  public class StarSystemComponent : IComponent
  {
    public string Name { get; set; }
    public Color Color { get; set; }
    public bool Draw { get; set; }
  }
}
