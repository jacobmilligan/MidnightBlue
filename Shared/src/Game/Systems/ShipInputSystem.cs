//
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

      Command cmd = null;
      var controller = entity.GetComponent<ShipController>();

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
