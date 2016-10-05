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

namespace MidnightBlue.Engine.EntityComponent
{
  public class Collision : IComponent
  {

    public Collision(params Rectangle[] boxes)
    {
      if ( boxes.Length > 0 ) {
        Boxes = new List<Rectangle>(boxes);
      } else {
        Boxes = new List<Rectangle>();
      }
    }

    public List<Rectangle> Boxes { get; set; }
  }
}
