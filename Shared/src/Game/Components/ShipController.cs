//
// 	ShipController.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.IO;
using MonoGame.Extended.Particles;

namespace MidnightBlue
{
  /// <summary>
  /// Represents the current travelling state of the ship
  /// </summary>
  public enum ShipState
  {
    Normal,
    Landing,
    Launching,
    LeavingScreen,
    Warping
  }

  /// <summary>
  /// Controls a ships movement and actions
  /// </summary>
  public class ShipController : IComponent
  {
    /// <summary>
    /// The input map for the ship controller. 
    /// Allows different input for different entities.
    /// </summary>
    private InputMap _inputMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.ShipController"/> class
    /// and assigns all default input and key mappings.
    /// </summary>
    public ShipController()
    {
      _inputMap = new InputMap();

      State = ShipState.Normal;
      IsLandable = false;

      _inputMap.Assign<MoveForward>(Keys.W, CommandType.Hold);
      _inputMap.Assign<MoveBackward>(Keys.S, CommandType.Hold);

      _inputMap.Assign<RotateRight>(Keys.D, CommandType.Hold);
      _inputMap.Assign<RotateLeft>(Keys.A, CommandType.Hold);

      _inputMap.Assign<MoveShip>(Keys.W, CommandType.Hold);
      _inputMap.Assign<MoveShip>(Keys.S, CommandType.Hold);

      _inputMap.Assign<EnterStarSystem>(Keys.E, CommandType.Trigger);
      _inputMap.Assign<LandCommand>(Keys.Space, CommandType.Trigger);
      _inputMap.Assign<LeaveStarSystem>(Keys.Q, CommandType.Trigger);
    }

    /// <summary>
    /// Gets the input map.
    /// </summary>
    /// <value>The input map.</value>
    public InputMap InputMap
    {
      get { return _inputMap; }
    }

    /// <summary>
    /// Gets or sets the current travel state of the ship.
    /// </summary>
    /// <value>The ships travelling state.</value>
    public ShipState State { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:MidnightBlue.ShipController"/> is able to
    /// be landed when the entity calls their LandCommand.
    /// </summary>
    /// <value><c>true</c> if is landable; otherwise, <c>false</c>.</value>
    public bool IsLandable { get; set; }
  }
}
