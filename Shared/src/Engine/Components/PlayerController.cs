//
// PlayerController.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//
using System;

using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.IO;

namespace MidnightBlue.Engine.EntityComponent
{
  /// <summary>
  /// Defines the attached entity as controllable
  /// </summary>
  public class PlayerController : Component
  {
    /// <summary>
    /// The input map for the player controller. 
    /// Allows different input for different entities.
    /// </summary>
    private InputMap _inputMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.PlayerController"/> component
    /// with default input assignment
    /// </summary>
    public PlayerController()
    {
      _inputMap = new InputMap();
      //_inputMap.Assign<MoveUp>(Keys.W, CommandType.Hold);
      _inputMap.Assign<ConsoleCommand>(Keys.OemTilde, CommandType.Trigger);
    }

    /// <summary>
    /// Gets the input map.
    /// </summary>
    /// <value>The input map.</value>
    public InputMap InputMap
    {
      get { return _inputMap; }
    }

  }
}

