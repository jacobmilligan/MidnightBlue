//
// ECSystem.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Collections.Generic;

namespace MidnightBlueMono
{
  public abstract class ECSystem
  {
    private List<Entity> _entities;
    private List<Type> _components;
    private Dictionary<ulong, Entity> _idEntityMap;

    public ECSystem(params Type[] components)
    {
      _entities = new List<Entity>();
      _components = new List<Type>();
      _idEntityMap = new Dictionary<ulong, Entity>();

      ID = 0;

      foreach ( var c in components ) {
        if ( c.BaseType != typeof(Component) ) {
          MBGame.Console.Write("Midnight Blue: '{0}' is not a valid component.", c);
          _components.Clear();
          break;
        } else {
          _components.Add(c);
        }
      }
    }

    public void Run()
    {
      for ( int i = 0; i < _entities.Count; i++ ) {
        Process(_entities[i]);
      }
    }

    public void AssociateEntity(Entity entity)
    {
      if ( !_idEntityMap.ContainsKey(entity.ID) ) {
        _entities.Add(entity);
        _idEntityMap.Add(entity.ID, entity);
      }
    }

    protected abstract void Process(Entity entity);

    public List<Entity> AssociatedEntities
    {
      get { return _entities; }

      set
      {
        _entities = value;
      }
    }

    public List<Type> ValidComponents
    {
      get { return _components; }
    }

    public ulong ID { get; set; }
  }
}

