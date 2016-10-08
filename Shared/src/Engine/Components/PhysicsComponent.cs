//
// 	PhysicsComponent.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 8/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MidnightBlue.Engine.EntityComponent
{
  public class PhysicsComponent : IComponent
  {
    public float RotationAcceleration { get; set; }
    public Vector2 Acceleration { get; set; }
    public Vector2 Velocity { get; set; }
    public float Power { get; set; }
  }
}
