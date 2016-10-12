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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.IO;

namespace MidnightBlue.Engine.EntityComponent
{
  public class InputSystem : EntitySystem
  {
    public InputSystem() : base(
      typeof(PlayerController), typeof(UtilityController)
    )
    {
      _associationType = EntityAssociation.Vague;
    }

    protected override void Process(Entity entity)
    {
      var keys = Keyboard.GetState().GetPressedKeys();

      var controller = entity.GetComponent<PlayerController>();
      var utilityController = entity.GetComponent<UtilityController>();

      if ( keys.Length > 1 ) {
        Console.WriteLine("combo");
      }
      for ( int k = 0; k < keys.Length; k++ ) {
        if ( controller != null ) {
          HandleInput(
            controller.InputMap[keys[k]], entity
          );
        }
        if ( utilityController != null ) {
          HandleInput(
            utilityController.InputMap[keys[k]], entity
          );
        }
      }
    }


    private void HandleInput(List<Command> commands, Entity entity)
    {
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

