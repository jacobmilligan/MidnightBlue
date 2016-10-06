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
namespace MidnightBlue.Engine.EntityComponent
{
  public enum PhysicsEnvironement { Galaxy, System, Planet }

  public class PhysicsSystem : EntitySystem
  {
    public PhysicsSystem() : base(typeof(Movement))
    {
      Environment = PhysicsEnvironement.Galaxy;
    }

    protected override void Process(Entity entity)
    {
      var movement = entity.GetComponent<Movement>();
      var decay = 0.0f;

      switch ( Environment ) {
        case PhysicsEnvironement.Galaxy:
          decay = 0.998f;
          break;
        case PhysicsEnvironement.System:
          decay = 0.98f;
          break;
        case PhysicsEnvironement.Planet:
          decay = 0.50f;
          break;
      }

      movement.Velocity *= decay;
      movement.RotationAcceleration *= decay * 0.96f;
    }

    public PhysicsEnvironement Environment { get; set; }
  }
}
