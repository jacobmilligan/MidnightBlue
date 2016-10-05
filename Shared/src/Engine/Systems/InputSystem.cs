//
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
      typeof(PlayerController)
    )
    { }

    protected override void Process(Entity entity)
    {
      var keys = Keyboard.GetState().GetPressedKeys();

      Command cmd = null;
      var controller = entity.GetComponent<PlayerController>();

      if ( controller != null ) {
        foreach ( var k in keys ) {
          cmd = controller.InputMap[k];
          if ( cmd != null ) {
            cmd.Execute(entity);
          }
        }
      }

    }

  }
}

