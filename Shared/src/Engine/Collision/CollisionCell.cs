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
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.Collision
{
  public class CollisionCell
  {
    private List<RectangleF> _rects;
    private HashSet<RectangleF> _lookup;

    public CollisionCell()
    {
      _rects = new List<RectangleF>();
      _lookup = new HashSet<RectangleF>();
    }

    public void Add(RectangleF rect)
    {
      _rects.Add(rect);
      _lookup.Add(rect);
    }

    public void Remove(RectangleF rect)
    {
      _rects.Remove(rect);
      _lookup.Remove(rect);
    }

    public bool Contains(RectangleF rect)
    {
      return _lookup.Contains(rect);
    }

    public void Clear()
    {
      _rects.Clear();
      _lookup.Clear();
    }

    public List<RectangleF> Items
    {
      get { return _rects; }
    }
  }
}
