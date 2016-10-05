﻿//
// InputSystem.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//
using System;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.IO;

namespace MidnightBlue.Engine.EntityComponent
{
  public class InputSystem : EntitySystem
  {
    public InputSystem() : base(
      typeof(PlayerController), typeof(SpriteComponent)
    )
    { }

    protected override void Process(Entity entity)
    {
      var keys = Keyboard.GetState().GetPressedKeys();

      Command cmd = null;
      IComponent controller = entity.GetComponent<PlayerController>();

      if ( controller == null ) {
        controller = entity.GetComponent<ShipController>();
      }

      if ( controller != null ) {
        foreach ( var k in keys ) {
          if ( controller is PlayerController ) {
            cmd = ((PlayerController)controller).InputMap[k];
          }
          if ( controller is ShipController ) {
            cmd = ((ShipController)controller).InputMap[k];
          }
          if ( cmd != null ) {
            cmd.Execute(entity);
          }
        }
      }

    }

  }
}
