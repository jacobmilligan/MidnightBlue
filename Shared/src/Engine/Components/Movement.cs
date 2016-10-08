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
    public Movement(float speed = 0.0f, float rotationSpeed = 0.0f)
    {
      Speed = speed;
      RotationSpeed = rotationSpeed;
    }

    public Vector2 Position { get; set; }
    public float RotationSpeed { get; set; }
    public RotationDirection RotationDirection { get; set; }
    public float Speed { get; set; }
    public Vector2 Heading { get; set; }
    public float Angle { get; set; }
  }
}
