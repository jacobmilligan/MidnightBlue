//
// 	Inventory.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 6/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;

namespace MidnightBlue.Engine.EntityComponent
{
  public class Inventory : IComponent
  {
    public Inventory()
    {
      Items = new Dictionary<Type, Collectable>();
    }

    public Dictionary<Type, Collectable> Items { get; set; }
  }
}
