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
    private LinkedList<Entity> _list;

    public CollisionCell()
    {
      _list = new LinkedList<Entity>();
    }

    public void Add(Entity entity)
    {
      _list.AddFirst(entity);
    }

    public void Remove(Entity entity)
    {
      _list.Remove(entity);
    }

    public bool Contains(Entity entity)
    {
      return _list.Contains(entity);
    }

    public void Clear()
    {
      _list.Clear();
    }

    public LinkedList<Entity> Items
    {
      get { return _list; }
    }
  }
}
