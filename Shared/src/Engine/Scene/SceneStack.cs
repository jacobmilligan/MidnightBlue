//
// SceneStack.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace MidnightBlue.Engine.Scenes
{
  public class SceneStack
  {
    private List<Scene> _scenes;

    public SceneStack()
    {
      _scenes = new List<Scene>();
    }

    public void Push(Scene scene, ContentManager content)
    {
      scene.Content = content;
      scene.Initialize();
      _scenes.Add(scene);
    }

    public void Pop()
    {
      if ( _scenes.Count > 0 ) {
        _scenes.RemoveAt(_scenes.Count - 1);
      } else {
        MBGame.Console.Debug("Midnight Blue: Cannot pop scene from empty stack");
      }
    }

    public void ResetTo(Scene scene, ContentManager content)
    {
      if ( _scenes.Count > 0 ) {
        while ( _scenes.Count > 0 ) {
          Pop();
        }
      }
      Push(scene, content);
    }

    public Scene SceneAt(int index)
    {
      Scene result = null;
      if ( index > 0 && index < _scenes.Count ) {
        result = _scenes[index];
      }
      return result;
    }

    public Scene Top
    {
      get
      {
        Scene result = null;

        if ( _scenes.Count > 0 ) {
          result = _scenes[_scenes.Count - 1];
        }

        return result;
      }
    }

    public Scene Bottom
    {
      get { return _scenes[0]; }
    }

    public int Size
    {
      get { return _scenes.Count; }
    }

    public int LastIndex
    {
      get { return _scenes.Count - 1; }
    }
  }
}

