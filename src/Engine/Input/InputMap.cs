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

namespace MidnightBlueMono
{
  public class InputMap
  {
    private Dictionary<Keys, Command> _inputMap = new Dictionary<Keys, Command>();

    public InputMap()
    {
      _inputMap = new Dictionary<Keys, Command>();
    }

    public void Assign<T>(Keys key, CommandType type) where T : Command
    {
      if ( _inputMap.ContainsKey(key) ) {
        _inputMap[key] = NewCommand<T>(key, type);
      } else {
        _inputMap.Add(key, NewCommand<T>(key, type));
      }
    }

    private Command NewCommand<T>(Keys key, CommandType type) where T : Command
    {
      return (T)Activator.CreateInstance(typeof(T), key, type);
    }

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

    public Dictionary<Keys, Command> Collection
    {
      get { return _inputMap; }
    }
  }
}

