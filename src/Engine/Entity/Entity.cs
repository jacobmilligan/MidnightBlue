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

namespace MidnightBlueMono
{
  public class Entity
  {
    public ulong ID { get; set; }

    private string _tag;
    private Dictionary<Type, Component> _components;

    public Entity(string tag = "")
    {
      _components = new Dictionary<Type, Component>();
      _tag = tag;
      ID = 0;
      Persistant = false;
    }

    public void Attach<T>(params object[] args) where T : Component
    {
      _components.Add(typeof(T), NewComponent<T>(args));
    }

    private Component NewComponent<T>(params object[] args) where T : Component
    {
      return (T)Activator.CreateInstance(typeof(T), args);
    }

    public string Tag
    {
      get { return _tag; }
    }

    public T GetComponent<T>() where T : Component
    {
      T result = null;
      var cType = typeof(T);
      if ( _components.ContainsKey(cType) ) {
        result = (T)_components[cType];
      }
      return result;
    }

    public Dictionary<Type, Component>.ValueCollection ComponentList
    {
      get { return _components.Values; }
    }

    public Dictionary<Type, Component>.KeyCollection ComponentTypeList
    {
      get { return _components.Keys; }
    }

    public bool Persistant { get; set; }
  }

}

