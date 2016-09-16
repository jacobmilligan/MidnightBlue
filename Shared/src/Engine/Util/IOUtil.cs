//
// IOUtil.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using Microsoft.Xna.Framework.Input;

namespace MidnightBlue
{
  public class IOUtil
  {
    private static KeyboardState _previousState;

    public static bool KeyTyped(Keys key)
    {
      var result = false;
      var nextState = Keyboard.GetState();
      if ( nextState.IsKeyDown(key) && _previousState.IsKeyUp(key) ) {
        result = true;
      }
      return result;
    }

    public static void UpdateKeyState()
    {
      _previousState = Keyboard.GetState();
    }

    public static char LastChar()
    {
      return 'c';
    }

    public static string LineToCSV(params string[] values)
    {
      var result = string.Empty;
      for ( int i = 0; i < values.Length; i++ ) {
        if ( values[i].Contains(",") ) {
          Console.WriteLine("CSV parse error: Value '{0}' contains illegal character ','", values[i]);
          result = null;
          break;
        } else {
          result += values[i];
          if ( i < values.Length - 1 ) {
            result += ",";
          }
        }
      }
      return result;
    }

    public static Keys LastKeyTyped
    {
      get
      {
        var result = Keys.None;
        var keyStates = Keyboard.GetState();
        foreach ( var k in keyStates.GetPressedKeys() ) {
          if ( KeyTyped(k) ) {
            result = k;
            break;
          }
        }
        return result;
      }
    }

    public static Keys LastKeyDown
    {
      get
      {
        var result = Keys.None;
        var keyStates = Keyboard.GetState();
        foreach ( var k in keyStates.GetPressedKeys() ) {
          if ( keyStates.IsKeyDown(k) ) {
            result = k;
            break;
          }
        }
        return result;
      }
    }
  }
}
