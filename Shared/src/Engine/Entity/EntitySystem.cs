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
using System.Diagnostics;

namespace MidnightBlue.Engine.EntityComponent
{
  /// <summary>
  /// Performs logic on an entity.
  /// </summary>
  public abstract class EntitySystem
  {
    /// <summary>
    /// The entities this system should know about
    /// </summary>
    private List<Entity> _entities;
    /// <summary>
    /// The valid components used in this system
    /// </summary>
    private List<Type> _components;

    private List<Entity> _toDestroy;
    /// <summary>
    /// All GUID's of entities this system knows about
    /// </summary>
    protected Dictionary<ulong, Entity> _idEntityMap;

    private Stopwatch _timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.EntityComponent.EntitySystem"/> class.
    /// Checks if the passed in types are valid components and if not, all components are deregistered
    /// leaving a system that knows about nothing and can't operate on any entities.
    /// </summary>
    /// <param name="components">Components this system is interested in</param>
    public EntitySystem(params Type[] components)
    {
      _entities = new List<Entity>();
      _toDestroy = new List<Entity>();
      _components = new List<Type>();
      _idEntityMap = new Dictionary<ulong, Entity>();

      ID = 0;

      foreach ( var c in components ) {
        if ( !(typeof(IComponent).IsAssignableFrom(c)) ) {
          Console.WriteLine("Midnight Blue: '{0}' is not a valid component.", c);
          _components.Clear();
          break;
        } else {
          _components.Add(c);
        }
      }
    }

    /// <summary>
    /// Runs this systems Process() method on all entities
    /// </summary>
    public void Run()
    {
      _timer = Stopwatch.StartNew();

      _toDestroy.Clear();

      PreProcess();
      ProcessingLoop();
      PostProcess();

      _timer.Stop();
#if !TESTING
      if ( MBGame.Console.Vars.ContainsKey("systemRuntime") ) {
        var rtVar = (bool)MBGame.Console.Vars["systemRuntime"];
        if ( rtVar ) {
          MBGame.Console.Write("{0}: {1}", this.GetType().Name, _timer.ElapsedMilliseconds);
        }
      }
#endif
      _timer.Reset();
    }

    public virtual void ProcessingLoop()
    {
      for ( int i = 0; i < _entities.Count; i++ ) {
        Process(_entities[i]);
      }
    }

    public void Destroy(Entity entity)
    {
      _toDestroy.Add(entity);
      _idEntityMap.Remove(entity.ID);
    }

    /// <summary>
    /// Associates a new entity with this system.
    /// </summary>
    /// <param name="entity">Entity to associate</param>
    public void AssociateEntity(Entity entity)
    {
      if ( !_idEntityMap.ContainsKey(entity.ID) ) {
        _entities.Add(entity);
        _idEntityMap.Add(entity.ID, entity);
      }
      PostAssociate(entity);
    }

    protected virtual void PreProcess() { }
    protected virtual void PostProcess() { }

    protected virtual void PostAssociate(Entity entity) { }

    /// <summary>
    /// Executes this systems logic on a single entity
    /// </summary>
    /// <param name="entity">Entity to operate on</param>
    protected abstract void Process(Entity entity);

    /// <summary>
    /// Gets or sets the associated entities list.
    /// </summary>
    /// <value>The associated entities.</value>
    public List<Entity> AssociatedEntities
    {
      get { return _entities; }

      set
      {
        _entities = value;
      }
    }

    /// <summary>
    /// Gets the list of components this system is interested in
    /// </summary>
    /// <value>The valid components.</value>
    public List<Type> ValidComponents
    {
      get { return _components; }
    }

    /// <summary>
    /// Gets or sets this systems ID mask
    /// </summary>
    /// <value>The identifier mask.</value>
    public ulong ID { get; set; }

    public List<Entity> DestroyList
    {
      get { return _toDestroy; }
    }
  }
}

