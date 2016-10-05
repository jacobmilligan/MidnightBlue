﻿//
// 	StarSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{
  public class StarSystemComponent : IComponent
  {
    public StarSystemComponent()
    {
    }

    public string Name { get; set; }
  }
}