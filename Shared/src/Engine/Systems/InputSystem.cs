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
      var cmd = entity.GetComponent<PlayerController>().InputMap[IOUtil.LastKeyDown];
      if ( cmd != null ) {
        cmd.Execute(entity);
      }
    }
  }
}

