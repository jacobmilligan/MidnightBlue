﻿//
// 	ShipInputSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.IO;

namespace MidnightBlue
{
  public class ShipInputSystem : EntitySystem
  {
    public ShipInputSystem() : base(
      typeof(ShipController)
    )
    { }

    protected override void Process(Entity entity)
    {
      var keys = Keyboard.GetState().GetPressedKeys();

      var controller = entity.GetComponent<ShipController>();

      if ( controller != null ) {
        for ( int k = 0; k < keys.Length; k++ ) {
          var commands = controller.InputMap[keys[k]];
          if ( commands != null ) {
            for ( int c = 0; c < commands.Count; c++ ) {
              var cmd = commands[c];
              if ( cmd != null ) {
                cmd.Execute(entity);
              }
            }
          }
        }
      }
    }

  }

}