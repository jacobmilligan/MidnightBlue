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

      var controller = entity.GetComponent<PlayerController>();

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

