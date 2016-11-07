#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;

using AppKit;
using Foundation;

using MB2D;
#endregion

namespace MidnightBlue.MacOSX
{
  static class MacMain
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main(string[] args)
    {
      NSApplication.Init();

      using ( var game = new MBGame(typeof(InitScene)) ) {
        game.Run();
      }
    }
  }
}
