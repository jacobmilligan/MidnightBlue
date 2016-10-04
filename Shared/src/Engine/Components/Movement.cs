//
// 	Movement.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MidnightBlue.Engine.EntityComponent
{
  public enum RotationDirection { None, Right, Left };

  public class Movement : IComponent
  {
    public Movement(float acceleration = 0.0f, float rotationSpeed = 0.0f)
    {
      Velocity = new Vector2(0, 0);
      Acceleration = acceleration;
      RotationSpeed = rotationSpeed;
    }

    public Vector2 Velocity { get; set; }
    public float RotationAcceleration { get; set; }
    public float Acceleration { get; set; }
    public float RotationSpeed { get; set; }
    public RotationDirection RotationDirection { get; set; }
  }
}
