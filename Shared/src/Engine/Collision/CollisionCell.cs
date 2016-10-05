//
// 	CollisionCell.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine.Collision
{
  public class CollisionCell
  {
    private List<Rectangle> _rects;

    public CollisionCell()
    {
      _rects = new List<Rectangle>();
    }

    public void Add(Rectangle rect)
    {
      _rects.Add(rect);
    }

    public void Remove(Rectangle rect)
    {
      _rects.Remove(rect);
    }

    public bool Contains(Rectangle rect)
    {
      return _rects.Contains(rect);
    }

    public List<Rectangle> Items
    {
      get { return _rects; }
    }
  }
}
