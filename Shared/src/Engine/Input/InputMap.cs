//
// InputMap.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace MidnightBlue.Engine.IO
{
  /// <summary>
  /// Maps commands to keys and trigger types
  /// </summary>
  public class InputMap
  {
    /// <summary>
    /// All commands mapped to specific keys
    /// </summary>
    private Dictionary<Keys, Command> _inputMap = new Dictionary<Keys, Command>();

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.IO.InputMap"/> class.
    /// </summary>
    public InputMap()
    {
      _inputMap = new Dictionary<Keys, Command>();
    }

    /// <summary>
    /// Assign a Command to the specified key and CommandType .
    /// </summary>
    /// <param name="key">Key to assign the command to</param>
    /// <param name="type">Type of trigger</param>
    /// <typeparam name="T">The command to assign</typeparam>
    public void Assign<T>(Keys key, CommandType type) where T : Command
    {
      if ( _inputMap.ContainsKey(key) ) {
        _inputMap[key] = NewCommand<T>(key, type);
      } else {
        _inputMap.Add(key, NewCommand<T>(key, type));
      }
    }

    /// <summary>
    /// Creates a new instance of a Command
    /// </summary>
    /// <returns>The command.</returns>
    /// <param name="key">Key to assign the command to</param>
    /// <param name="type">Type of trigger</param>
    /// <typeparam name="T">Type of Command to new</typeparam>
    private Command NewCommand<T>(Keys key, CommandType type) where T : Command
    {
      return (T)Activator.CreateInstance(typeof(T), key, type);
    }

    /// <summary>
    /// Gets the <see cref="T:MidnightBlue.Engine.IO.Commnad"/> mapped to the specified key.
    /// </summary>
    /// <param name="key">Key to query</param>
    public Command this[Keys key]
    {
      get
      {
        Command result = null;
        if ( _inputMap.ContainsKey(key) ) {
          result = _inputMap[key];
        }
        return result;
      }
    }

    /// <summary>
    /// Gets a key->Command dictionary
    /// </summary>
    /// <value>The collection of commands.</value>
    public Dictionary<Keys, Command> Collection
    {
      get { return _inputMap; }
    }
  }
}

