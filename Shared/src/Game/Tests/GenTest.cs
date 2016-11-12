//
// GenTest.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

namespace MidnightBlue.Testing
{
  public class GenTest
  {
    public static void GalaxyGenTest(params string[] args)
    {
      int size, radius, seed = 0;
      int.TryParse(args[0], out size);
      int.TryParse(args[1], out radius);

      if ( args.Length > 2 ) {
        int.TryParse(args[2], out seed);
      }

      var galaxy = new Galaxy(size, radius);
      galaxy.Generate(seed);

      var w = SwinGame.OpenWindow("Galaxy Gen Test", 1600, 900);

      SwinGame.SetCurrentWindow(w);

      while ( !SwinGame.WindowCloseRequested(w) ) {
        SwinGame.ProcessEvents();

        SwinGame.ClearScreen(SwinGame.RGBColor(0, 24, 72));

        galaxy.Draw();

        SwinGame.RefreshScreen(60);
      }

      SwinGame.CloseWindow(w);
      MBGame.ResetWindow();
    }
  }
}

