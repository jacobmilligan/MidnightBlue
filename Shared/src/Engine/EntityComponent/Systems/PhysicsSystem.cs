//
// 	PhysicsSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.EntityComponent
{
  public class PhysicsEnvironment
  {
    public float Inertia { get; set; }
    public float RotationInertia { get; set; }
  }

  public class PhysicsSystem : EntitySystem
  {
    public PhysicsSystem()
      : base(typeof(PhysicsComponent), typeof(Movement))
    {
      Environment = new PhysicsEnvironment {
        Inertia = 0.999f,
        RotationInertia = 0.98f
      };
    }

    protected override void Process(Entity entity)
    {
      var physics = entity.GetComponent<PhysicsComponent>();
      var movement = entity.GetComponent<Movement>();

      physics.RotationAcceleration *= Environment.RotationInertia;
      movement.Angle += physics.RotationAcceleration;

      // Newtons law except without mass - just some arbitrary 'power' value
      // applied by adding thrust to the ship
      var force = movement.Heading * physics.Power;
      physics.Acceleration = force;
      physics.Velocity += physics.Acceleration * MBGame.DeltaTime;
      movement.Position += physics.Velocity * MBGame.DeltaTime;

      physics.Velocity *= Environment.Inertia;

      // Reset every frame as power should be only on or off
      // not decaying
      physics.Power = 0;
    }

    public PhysicsEnvironment Environment { get; set; }
  }
}
