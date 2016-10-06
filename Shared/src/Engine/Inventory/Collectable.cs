//
// 	ICollectable.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine
{
  public abstract class Collectable
  {
    private string _name, _tag;
    private int _count;

    public Collectable(string name, string tag, int initialCount)
    {
      _name = name;
      _tag = tag;
      _count = initialCount;
    }

    public void Consume(int amount = 1)
    {
      if ( amount > 0 ) {
        _count -= amount;
      }
      if ( _count < 0 ) {
        _count = 0;
      }
    }

    public void Add(int amount = 1)
    {
      if ( amount > 0 ) {
        _count += amount;
      }
    }

    public abstract void Effect(Entity entity);

    public string Name
    {
      get { return _name; }
    }

    public string Tag
    {
      get { return _tag; }
    }

    public int Count
    {
      get { return _count; }
    }
  }
}
