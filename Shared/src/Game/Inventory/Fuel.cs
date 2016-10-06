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
  public class Fuel : Collectable
  {
    public Fuel(int initialCount) : base("Thruster Fuel", "Fuel", initialCount) { }

    public override void Effect(Entity entity)
    {

    }
  }
}
