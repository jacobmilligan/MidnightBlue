//
// ConsoleCommand.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine.IO
{
  /// <summary>
  /// Shows or hides the debug console
  /// </summary>
  public class ConsoleCommand : Command
  {

    public ConsoleCommand(Keys key, CommandType type) : base(key, type) { }

    protected override void OnKeyPress(Entity e = null)
    {
      MBGame.Console.Toggle();
    }
  }
}

