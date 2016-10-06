//
// 	Collision.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.Collision;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.EntityComponent
{
  public class CollisionComponent : IComponent
  {

    public CollisionComponent(params RectangleF[] boxes)
    {
      if ( boxes.Length > 0 ) {
        Boxes = new List<RectangleF>(boxes);
      } else {
        Boxes = new List<RectangleF>();
      }
    }

    public List<RectangleF> Boxes { get; set; }
    public bool Event { get; set; }
  }
}
