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
  public class MoveShip : Command
  {
    public MoveShip(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      if ( e != null ) {
        var inventory = e.GetComponent<Inventory>();
        if ( inventory != null && inventory.Items.ContainsKey(typeof(Fuel)) ) {
          var fuel = inventory.Items[typeof(Fuel)];
          fuel.Consume();
          if ( fuel.Count <= 0 ) {
            var physics = e.GetComponent<PhysicsComponent>();
            if ( physics != null ) {
              physics.Acceleration = new Vector2(0, 0);
            }
          }
        }
      }
    }
  }

  public class EnterStarSystem : Command
  {
    public EnterStarSystem(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      var collision = e.GetComponent<CollisionComponent>();
      var shipController = e.GetComponent<ShipController>();

      if ( collision != null && collision.Event ) {
        var sys = collision.Collider.GetComponent<StarSystem>();
        var planet = collision.Collider.GetComponent<PlanetComponent>();
        if ( sys != null || planet != null ) {
          shipController.WillEnter = true;
        }
      }
    }
  }
}
