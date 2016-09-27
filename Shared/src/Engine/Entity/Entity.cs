//
// Entity.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Collections.Generic;

namespace MidnightBlue.Engine.EntityComponent
{
  public class Entity
  {
    public ulong Mask { get; set; }
    public ulong ID { get; set; }

    private string _tag;
    private EntityMap _container;
    private Dictionary<Type, IComponent> _components;

    private Entity(string tag = "")
    {
      _components = new Dictionary<Type, IComponent>();
      _tag = tag;
      Mask = 0;
      Persistant = false;
    }

    public Entity(EntityMap container, string tag = "") : this(tag)
    {
      _container = container;
      ID = container.NextID;
    }

    public void Attach<T>(params object[] args) where T : IComponent
    {
      _components.Add(typeof(T), NewComponent<T>(args));
      _container.UpdateEntityMask(this);
      _container.UpdateSystems(this);
    }

    public T GetComponent<T>() where T : IComponent
    {
      T result = default(T);
      var cType = typeof(T);
      if ( _components.ContainsKey(cType) ) {
        result = (T)_components[cType];
      }
      return result;
    }

    private IComponent NewComponent<T>(params object[] args) where T : IComponent
    {
      return (T)Activator.CreateInstance(typeof(T), args);
    }

    public string Tag
    {
      get { return _tag; }
    }

    public Dictionary<Type, IComponent>.ValueCollection ComponentList
    {
      get { return _components.Values; }
    }

    public Dictionary<Type, IComponent>.KeyCollection ComponentTypeList
    {
      get { return _components.Keys; }
    }

    public bool Persistant { get; set; }
  }

}

