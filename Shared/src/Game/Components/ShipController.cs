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
  public enum ShipState { Normal, Landing, Launching, Warping }

  public class ShipController : IComponent
  {
    /// <summary>
    /// The input map for the ship controller. 
    /// Allows different input for different entities.
    /// </summary>
    private InputMap _inputMap;

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
    }

    /// <summary>
    /// Gets the input map.
    /// </summary>
    /// <value>The input map.</value>
    public InputMap InputMap
    {
      get { return _inputMap; }
    }

    public ShipState State { get; set; }
    public bool IsLandable { get; set; }
  }
}
