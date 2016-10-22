//
// 	ShipCommands.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 7/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.IO;

namespace MidnightBlue
{
  /// <summary>
  /// Performs logic aside from movement required to execute when
  /// moving the ship such as consuming fuel.
  /// </summary>
  public class MoveShip : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.MoveShip"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public MoveShip(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Consumes fuel, stopping the ship if there's none remaining
    /// </summary>
    /// <param name="e">Entity with inventory to operate on.</param>
    protected override void OnKeyPress(Entity e)
    {
      if ( e.HasComponent<Inventory>() ) {

        var inventory = e.GetComponent<Inventory>();

        // Consumes a single unit of fuel
        if ( inventory.Items.ContainsKey(typeof(Fuel)) ) {

          var fuel = inventory.Items[typeof(Fuel)];
          fuel.Consume();

          // Stops the ship if no fuel remaining
          if ( fuel.Count <= 0 && e.HasComponent<PhysicsComponent>() ) {
            e.GetComponent<PhysicsComponent>().Acceleration = new Vector2(0, 0);
          }

        }
      }
    }
  }

  /// <summary>
  /// Lands the ship
  /// </summary>
  public class LandCommand : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.LandCommand"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public LandCommand(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Lands the ship on the key press if terrain is landable
    /// </summary>
    /// <param name="e">Entity with the ship controller to operate on.</param>
    protected override void OnKeyPress(Entity e)
    {
      var shipController = e.GetComponent<ShipController>();

      // Lands if possible
      if ( shipController != null && shipController.IsLandable ) {
        shipController.State = ShipState.Landing;
      }
    }
  }

  /// <summary>
  /// Launches the ship from a landed state
  /// </summary>
  public class LaunchCommand : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.LaunchCommand"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public LaunchCommand(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Launches the ship from landed on key press.
    /// </summary>
    /// <param name="e">Entity with ship controller to operate on.</param>
    protected override void OnKeyPress(Entity e)
    {
      var shipController = e.GetComponent<ShipController>();

      // Launch
      if ( shipController == null ) {
        shipController = e.Attach<ShipController>() as ShipController;
        shipController.State = ShipState.Launching;
      }
    }
  }

  /// <summary>
  /// Enters a star system scene from the galaxy view
  /// </summary>
  public class EnterStarSystem : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.EnterStarSystem"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public EnterStarSystem(Keys key, CommandType type) : base(key, type) { }

    /// <summary>
    /// Enters the collided with star system on keypress
    /// </summary>
    /// <param name="e">E.</param>
    protected override void OnKeyPress(Entity e)
    {
      var collision = e.GetComponent<CollisionComponent>();
      var shipController = e.GetComponent<ShipController>();

      // Check for collision event with star system in galaxy view
      if ( collision != null && collision.Event ) {
        var sys = collision.Collider.GetComponent<StarSystem>();
        var planet = collision.Collider.GetComponent<PlanetComponent>();

        // Enter the system if key was pressed
        if ( sys != null || planet != null ) {
          shipController.State = ShipState.Landing;
        }
      }
    }
  }

  public class LeaveStarSystem : Command
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.LeaveStarSystem"/> class.
    /// </summary>
    /// <param name="key">Key to assign to.</param>
    /// <param name="type">Trigger type.</param>
    public LeaveStarSystem(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var shipController = e.GetComponent<ShipController>();

      // Leave screen
      if ( shipController != null ) {
        shipController = e.Attach<ShipController>() as ShipController;
        shipController.State = ShipState.LeavingScreen;
      }
    }
  }
}
