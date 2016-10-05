﻿//
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
  /// <summary>
  /// Represents a tagged and id'd container for components that can be
  /// operated on by systems.
  /// </summary>
  public class Entity
  {
    /// <summary>
    /// Gets and sets the entities component mask.
    /// </summary>
    /// <value>The component mask.</value>
    public ulong Mask { get; set; }

    /// <summary>
    /// Gets and sets the entities Globally Unique ID in the EntityMap
    /// </summary>
    /// <value>GUID</value>
    public ulong ID { get; set; }

    /// <summary>
    /// The entities tag name
    /// </summary>
    private string _tag;
    /// <summary>
    /// The entities parent container
    /// </summary>
    private EntityMap _container;
    /// <summary>
    /// The Components this entity currently has attached
    /// </summary>
    private Dictionary<Type, IComponent> _components;

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:MidnightBlue.Engine.EntityComponent.Entity"/> class.
    /// </summary>
    /// <param name="tag">Tag name to give entity</param>
    private Entity(string tag = "")
    {
      _components = new Dictionary<Type, IComponent>();
      _tag = tag;
      Mask = 0;
      Persistant = false;
    }

    /// <summary>
    /// Initializes a new instance of the 
    /// <see cref="T:MidnightBlue.Engine.EntityComponent.Entity"/> class.
    /// </summary>
    /// <param name="container">The entities parent EntityMap</param>
    /// <param name="tag">Tagname to give the entity</param>
    public Entity(EntityMap container, string tag = "") : this(tag)
    {
      _container = container;
      ID = container.NextID;
    }

    /// <summary>
    /// Attaches a new component to the entity.
    /// </summary>
    /// <param name="args">The components constructor arguments</param>
    /// <typeparam name="T">Type of component to attach</typeparam>
    public IComponent Attach<T>(params object[] args) where T : IComponent
    {
      var component = NewComponent<T>(args);
      _components.Add(typeof(T), component);
      _container.UpdateEntityMask(this);
      _container.UpdateSystems(this);
      return component;
    }

    /// <summary>
    /// Queries the entity to see if it has a component attached and returns it if it does
    /// </summary>
    /// <returns>The component if the entity has it attached, null otherwise</returns>
    /// <typeparam name="T">Component to query the entity for.</typeparam>
    public T GetComponent<T>() where T : IComponent
    {
      T result = default(T);
      var cType = typeof(T);
      if ( _components.ContainsKey(cType) ) {
        result = (T)_components[cType];
      }
      return result;
    }

    /// <summary>
    /// Creates a new instance of an object derived from IComponent
    /// </summary>
    /// <returns>The new component instance</returns>
    /// <param name="args">Constructor arguments for the component type</param>
    /// <typeparam name="T">Type of component to create</typeparam>
    private IComponent NewComponent<T>(params object[] args) where T : IComponent
    {
      return (T)Activator.CreateInstance(typeof(T), args);
    }

    /// <summary>
    /// Gets this entities tagname
    /// </summary>
    /// <value>The tagname</value>
    public string Tag
    {
      get { return _tag; }
    }

    /// <summary>
    /// Gets the list if components attached to this entity.
    /// </summary>
    /// <value>The component list.</value>
    public Dictionary<Type, IComponent>.ValueCollection ComponentList
    {
      get { return _components.Values; }
    }

    /// <summary>
    /// Gets the types of components this entity has attached
    /// </summary>
    /// <value>The component type list.</value>
    public Dictionary<Type, IComponent>.KeyCollection ComponentTypeList
    {
      get { return _components.Keys; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:MidnightBlue.Engine.EntityComponent.Entity"/> is persistant in its
    /// parent <see cref="T:MidnightBlue.Engine.EntityComponent.EntityMap"/>.
    /// </summary>
    /// <value><c>true</c> if persistant; otherwise, <c>false</c>.</value>
    public bool Persistant { get; set; }
  }

}

