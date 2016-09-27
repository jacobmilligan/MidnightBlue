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

namespace MidnightBlue.Engine.EntityComponent
{

  public class EntityMap
  {
    private ulong _lastMask;
    private ulong _nextID;

    private Dictionary<Type, EntitySystem> _systems;
    private Dictionary<Type, ulong> _components;
    private Dictionary<string, int> _tags;
    private List<Entity> _entities;

    public EntityMap()
    {
      _lastMask = _nextID = 0;
      _systems = new Dictionary<Type, EntitySystem>();
      _entities = new List<Entity>();
      _components = new Dictionary<Type, ulong>();
      _tags = new Dictionary<string, int>();
    }

    public EntityMap(EntityMap map)
    {
      _lastMask = map._lastMask;
      _systems = new Dictionary<Type, EntitySystem>(map._systems);
      _entities = new List<Entity>(map._entities);
      _components = new Dictionary<Type, ulong>(map._components);
      _tags = new Dictionary<string, int>(map._tags);
    }

    public void AddComponent<T>() where T : IComponent
    {
      var type = typeof(T);
      if ( !_components.ContainsKey(type) ) {
        // Couldn't get this to fail tests but just in case any bitwise-related bugs
        // I didn't think of crop up, make this increase by a power of 2 instead
        _components.Add(type, ++_lastMask);
      }
    }

    public void AddSystem<T>() where T : EntitySystem, new()
    {
      var sysType = typeof(T);
      if ( !_systems.ContainsKey(sysType) ) {
        var sysT = new T();
        _systems.Add(sysType, sysT);
        var sys = _systems[sysType];
        foreach ( var c in sys.ValidComponents ) {
          sys.ID |= NewOrExistingComponentID(c);
        }
      }
    }

    public void AddEntity(Entity entity)
    {
      UpdateEntityMask(entity);
      if ( !_tags.ContainsKey(entity.Tag) ) {

        if ( entity.Tag.Length > 0 ) {
          _tags.Add(entity.Tag, _entities.Count);
        }

        _entities.Add(entity);
        UpdateSystems(entity);

      } else {
        Debug.WriteLine("Duplicate Tag '" + entity.Tag + "'");
      }
    }

    public void UpdateEntityMask(Entity entity)
    {
      foreach ( var c in entity.ComponentTypeList ) {
        entity.Mask |= NewOrExistingComponentID(c);
      }
    }

    public void UpdateSystems(Entity entity)
    {
      foreach ( var sys in _systems.Values ) {
        if ( (entity.Mask & sys.ID) == sys.ID ) {
          sys.AssociateEntity(entity);
        }
      }
    }

    public Entity CreateEntity(string tag = "")
    {
      var entity = new Entity(this, tag);
      AddEntity(entity);
      return entity;
    }

    public ulong GetComponentID<T>() where T : IComponent
    {
      ulong result = 0;
      var type = typeof(T);
      if ( _components.ContainsKey(type) ) {
        result = _components[type];
      } else {
        Debug.WriteLine("Invalid Component '{0}'", type);
      }
      return result;
    }

    public EntitySystem GetSystem<T>() where T : EntitySystem
    {
      EntitySystem result = null;
      var key = typeof(T);
      if ( key.BaseType == typeof(EntitySystem) ) {
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
      if ( typeof(IComponent).IsAssignableFrom(component) ) {
        if ( _components.ContainsKey(component) ) {
          result = _components[component];
        } else {
          // Couldn't get this to fail tests but just in case any bitwise-related bugs
          // I didn't think of crop up, make this increase by a power of 2 instead
          result = ++_lastMask;
          _components.Add(component, result);
        }
      } else {
        Debug.WriteLine("Invalid Component '{0}'", component);
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

    public int EntityCount
    {
      get { return _entities.Count; }
    }

    public ulong NextID
    {
      get { return ++_nextID; }
    }
  }
}

