//
// MoveCommands.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 13/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using MidnightBlue.Engine.Testing;
using MidnightBlue.Engine.EntityComponent;
using Microsoft.Xna.Framework.Input;

namespace MidnightBlue.Engine.IO
{
  public class MoveUp : Command
  {
    public MoveUp(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      e.GetComponent<Position>().Y -= 1;
    }
  }
}

