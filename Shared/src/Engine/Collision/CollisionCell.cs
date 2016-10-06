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
    private LinkedList<RectangleF> _list;

    public CollisionCell()
    {
      _list = new LinkedList<RectangleF>();
    }

    public void Add(RectangleF rect)
    {
      _list.AddFirst(rect);
    }

    public void Remove(RectangleF rect)
    {
      _list.Remove(rect);
    }

    public bool Contains(RectangleF rect)
    {
      return _list.Contains(rect);
    }

    public void Clear()
    {
      _list.Clear();
    }

    public LinkedList<RectangleF> Items
    {
      get { return _list; }
    }
  }
}
