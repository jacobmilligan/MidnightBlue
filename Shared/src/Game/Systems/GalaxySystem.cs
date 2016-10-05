//
// 	GalaxySystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine
{
  public class GalaxySystem : EntitySystem
  {
    public GalaxySystem()
      : base(
        typeof(StarSystemComponent),
        typeof(CollisionComponent)
      )
    {
    }

    protected override void Process(Entity entity)
    {

      var sys = entity.GetComponent<StarSystemComponent>();
      var sysCollision = entity.GetComponent<CollisionComponent>();

      if ( sys != null && sysCollision != null ) {
        sys.Draw = sysCollision.Event;
      }
    }
  }
}
