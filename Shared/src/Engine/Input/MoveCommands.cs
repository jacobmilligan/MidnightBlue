//
// MoveCommands.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 13/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using MidnightBlue.Engine.Testing;
using MidnightBlue.Engine.EntityComponent;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace MidnightBlue.Engine.IO
{
  /// <summary>
  /// Moves a player controller up 
  /// </summary>
  public class MoveUp : Command
  {
    public MoveUp(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(270); // radians
      physics.Velocity += new Vector2(0, -1 * movement.Speed) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  public class MoveRight : Command
  {
    public MoveRight(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(0); ; // radians
      physics.Velocity += new Vector2(1 * movement.Speed, 0) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  public class MoveDown : Command
  {
    public MoveDown(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(90); ; // radians
      physics.Velocity += new Vector2(0, 1 * movement.Speed) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  public class MoveLeft : Command
  {
    public MoveLeft(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      movement.Angle = MathHelper.ToRadians(180);
      physics.Velocity += new Vector2(-1 * movement.Speed, 0) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  public class MoveForward : Command
  {
    public MoveForward(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var physics = e.GetComponent<PhysicsComponent>();
      var movement = e.GetComponent<Movement>();
      physics.Velocity += (movement.Heading * movement.Speed) * MBGame.DeltaTime;
      physics.Power = movement.Speed;
    }
  }

  public class MoveBackward : Command
  {
    public MoveBackward(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var movement = e.GetComponent<Movement>();
      var physics = e.GetComponent<PhysicsComponent>();
      physics.Velocity -= (movement.Heading * movement.Speed) * MBGame.DeltaTime;
      physics.Power = -movement.Speed;
    }
  }

  public class RotateRight : Command
  {
    public RotateRight(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var movement = e.GetComponent<Movement>();
      var physics = e.GetComponent<PhysicsComponent>();
      if ( movement != null ) {
        if ( physics != null ) {
          physics.RotationAcceleration += movement.RotationSpeed * MBGame.DeltaTime;
        } else {
          movement.Angle += movement.RotationSpeed * MBGame.DeltaTime;
        }
      }
    }
  }

  public class RotateLeft : Command
  {
    public RotateLeft(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var movement = e.GetComponent<Movement>();
      var physics = e.GetComponent<PhysicsComponent>();
      if ( movement != null ) {
        if ( physics != null ) {
          physics.RotationAcceleration -= movement.RotationSpeed * MBGame.DeltaTime;
        } else {
          movement.Angle -= movement.RotationSpeed * MBGame.DeltaTime;
        }
      }
    }
  }
}

