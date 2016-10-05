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

namespace MidnightBlue.Engine.EntityComponent
{
  public class CollisionComponent : IComponent
  {

    public CollisionComponent(params Rectangle[] boxes)
    {
      if ( boxes.Length > 0 ) {
        Boxes = new List<Rectangle>(boxes);
      } else {
        Boxes = new List<Rectangle>();
      }
      ContainingCells = new List<CollisionCell>();
    }

    public List<Rectangle> Boxes { get; set; }
    public List<CollisionCell> ContainingCells { get; set; }
    public bool Event { get; set; }
  }
}
