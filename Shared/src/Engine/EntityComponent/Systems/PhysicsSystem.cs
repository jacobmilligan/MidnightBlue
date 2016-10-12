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
  public enum PhysicsEnvironement { Galaxy, System, Planet }

  public class PhysicsSystem : EntitySystem
  {
    public PhysicsSystem()
      : base(typeof(PhysicsComponent), typeof(Movement))
    {
      Environment = PhysicsEnvironement.Galaxy;
    }

    protected override void Process(Entity entity)
    {
      var physics = entity.GetComponent<PhysicsComponent>();
      var movement = entity.GetComponent<Movement>();

      var decay = 0.0f;

      switch ( Environment ) {
        case PhysicsEnvironement.Galaxy:
          decay = 0.999f;
          break;
        case PhysicsEnvironement.System:
          decay = 0.98f;
          break;
        case PhysicsEnvironement.Planet:
          decay = 0.80f;
          break;
      }

      physics.RotationAcceleration *= 0.98f;
      movement.Angle += physics.RotationAcceleration;

      // Newtons law except without mass - just some arbitrary 'power' value
      // applied by adding thrust to the ship
      var force = movement.Heading * physics.Power;
      physics.Acceleration = force;
      physics.Velocity += physics.Acceleration * MBGame.DeltaTime;
      movement.Position += physics.Velocity * MBGame.DeltaTime;

      physics.Velocity *= decay;

      // Reset every frame as power should be only on or off
      // not decaying
      physics.Power = 0;
    }

    public PhysicsEnvironement Environment { get; set; }
  }
}
