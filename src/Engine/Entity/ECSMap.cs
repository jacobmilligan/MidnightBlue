//
// ECSMap.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace MidnightBlueMono
{

  public class ECSMap
  {
    private ulong _lastID;

    private Dictionary<Type, ECSystem> _systems;
    private Dictionary<Type, ulong> _components;
    private Dictionary<string, int> _tags;
    private List<Entity> _entities;

    public ECSMap()
    {
      _lastID = 0;
      _systems = new Dictionary<Type, ECSystem>();
      _entities = new List<Entity>();
      _components = new Dictionary<Type, ulong>();
      _tags = new Dictionary<string, int>();
    }

    public ECSMap(ECSMap map)
    {
      _lastID = map._lastID;
      _systems = new Dictionary<Type, ECSystem>(map._systems);
      _entities = new List<Entity>(map._entities);
      _components = new Dictionary<Type, ulong>(map._components);
      _tags = new Dictionary<string, int>(map._tags);
    }

    public void AddComponent<T>() where T : Component
    {
      var type = typeof(T);
      if ( !_components.ContainsKey(type) ) {
        _components.Add(type, _lastID++);
      }
    }

    public void AddSystem<T>() where T : ECSystem, new()
    {
      var sysType = typeof(T);
      if ( !_systems.ContainsKey(sysType) ) {
        _systems.Add(sysType, new T());
        var sys = _systems[sysType];
        foreach ( var c in sys.ValidComponents ) {
          sys.ID |= NewOrExistingComponentID(c);
        }
      }
    }

    public void AddEntity(Entity entity)
    {
      foreach ( var c in entity.ComponentTypeList ) {
        entity.ID |= NewOrExistingComponentID(c);
      }
      if ( !_tags.ContainsKey(entity.Tag) && entity.Tag != string.Empty ) {
        _tags.Add(entity.Tag, _entities.Count);
      } else {
        Debug.WriteLine("Midnight Blue: entity with duplicate tag '{0}' exists.", entity.Tag);
      }
      _entities.Add(entity);
      foreach ( var sys in _systems.Values ) {
        if ( (entity.ID & sys.ID) == sys.ID ) {
          sys.AssociatedEntities.Add(entity);
        }
      }
    }

    public Entity CreateEntity(string tag = "")
    {
      var entity = new Entity(tag);
      AddEntity(entity);
      return entity;
    }

    public ulong GetComponentID(Type component)
    {
      ulong result = 0;
      if ( component.BaseType == typeof(Component) && _components.ContainsKey(component) ) {
        result = _components[component];
      } else {
        Debug.WriteLine("Midnight Blue: Not a valid component '{0}'", component);
      }
      return result;
    }

    public ECSystem GetSystem<T>() where T : ECSystem
    {
      ECSystem result = null;
      var key = typeof(T);
      if ( key.BaseType == typeof(ECSystem) ) {
        result = _systems[key];
      }
      return result;
    }

    public void Clear()
    {
      foreach ( var system in _systems.Values ) {
        system.AssociatedEntities.RemoveAll(entity => !entity.Persistant);
      }
      _tags.Clear();
      _entities.RemoveAll(entity => !entity.Persistant);
      for ( int i = 0; i < _entities.Count; i++ ) {
        if ( _entities[i].Tag != string.Empty ) {
          _tags.Add(_entities[i].Tag, i);
        }
      }
    }

    private ulong NewOrExistingComponentID(Type component)
    {
      ulong result = 0;
      if ( component.BaseType == typeof(Component) ) {
        if ( _components.ContainsKey(component) ) {
          result = _components[component];
        } else {
          result = ++_lastID;
          _components.Add(component, result);
        }
      } else {
        Debug.WriteLine("Midnight Blue: Not a valid component '{0}'", component);
      }
      return result;
    }

    public Entity this[string key]
    {
      get
      {
        Entity result = null;
        if ( _tags.ContainsKey(key) ) {
          var index = _tags[key];
          if ( index < _entities.Count ) {
            result = _entities[index];
          }
        }
        return result;
      }
    }
  }
}

