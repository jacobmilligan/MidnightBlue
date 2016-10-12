//
// 	DepthSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
namespace MidnightBlue.Engine.EntityComponent
{
  public class DepthSystem : EntitySystem
  {
    public DepthSystem() : base(typeof(Depth), typeof(SpriteTransform)) { }

    protected override void Process(Entity entity)
    {
      var depth = entity.GetComponent<Depth>();
      var sprite = entity.GetComponent<SpriteTransform>();
      if ( depth != null && sprite != null ) {
        sprite.Z = sprite.Bounds.Top;
      }
    }
  }
}
