﻿//
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
            var movement = e.GetComponent<Movement>();
            if ( movement != null ) {
              movement.Acceleration = 0;
            }
          }
        }
      }
    }
  }
}
