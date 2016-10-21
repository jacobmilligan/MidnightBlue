//
// 	Fuel.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{
  /// <summary>
  /// Fuel used in the ships normal thruster drive
  /// </summary>
  public class Fuel : Collectable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Fuel"/> class.
    /// </summary>
    /// <param name="initialCount">Initial amount of fuel.</param>
    public Fuel(int initialCount) : base("Thruster Fuel", "Fuel", initialCount) { }

    /// <summary>
    /// Has no effect on the entity
    /// </summary>
    /// <param name="entity">Entity.</param>
    public override void Effect(Entity entity)
    {

    }
  }
}
