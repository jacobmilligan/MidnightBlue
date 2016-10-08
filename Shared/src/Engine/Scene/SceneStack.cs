//
// SceneStack.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace MidnightBlue.Engine.Scenes
{
  public class SceneStack
  {
    private List<Scene> _scenes;
    private Scene _nextScene;

    public SceneStack()
    {
      _scenes = new List<Scene>();
    }

    public void Push(Scene scene)
    {
      scene.SceneController = this;

      if ( _scenes.Count > 0 ) {
        Top.TransitionState = TransitionState.Pausing;
      }

      _nextScene = scene;
      if ( _nextScene.TransitionState == TransitionState.Null ) {
        _nextScene.TransitionState = TransitionState.Initializing;
      }
    }

    public void Update()
    {
      if ( _scenes.Count > 0 ) {
        HandleState(Top);
        if ( _nextScene != null ) {
          HandleState(_nextScene);
        }
      } else {
        if ( _nextScene != null ) {
          HandleState(_nextScene);
        } else {
          MBGame.ForceQuit = true;
        }
      }

      if ( Top != null ) {
        Top.UpdateTransition();
        Top.HandleInput();
        Top.Update();
      }
    }

    public void Pop()
    {
      if ( _scenes.Count > 0 ) {
        if ( Top.TransitionState != TransitionState.Null ) {
          Top.TransitionState = TransitionState.Exiting;
        }
        if ( _scenes.Count > 1 ) {
          _scenes[_scenes.Count - 2].TransitionState = TransitionState.Resuming;
        }
      } else {
        MBGame.Console.Debug("Midnight Blue: Cannot pop scene from empty stack");
      }
    }

    public void ResetTo(Scene scene)
    {
      if ( _scenes.Count > 0 ) {
        _nextScene = scene;
        var currentScene = Top;
        while ( _scenes.Count > 0 ) {
          Pop();
          Update();
        }
      }
      Push(scene);
    }

    public Scene SceneAt(int index)
    {
      Scene result = null;
      if ( index > 0 && index < _scenes.Count ) {
        result = _scenes[index];
      }
      return result;
    }

    private void HandleState(Scene scene)
    {
      var last = scene.PreviousTransitionState;
      var current = scene.TransitionState;

      if ( last != current ) {
        if ( last == TransitionState.Null ) {
          switch ( current ) {
            case TransitionState.Initializing:
              scene.Initialize();
              break;
            case TransitionState.None:
              _scenes.Add(_nextScene);
              _nextScene = null;
              break;
            default:
              Console.WriteLine("Transition from '{0}' to '{1}' is invalid", last, current);
              break;
          }
        }
        if ( last == TransitionState.None ) {
          switch ( current ) {
            case TransitionState.Null:
              if ( _scenes.Count > 0 ) {
                _scenes.RemoveAt(_scenes.Count - 1);
              }
              break;
            case TransitionState.Pausing:
              scene.Pause();
              break;
            case TransitionState.Resuming:
              scene.Resume();
              break;
            case TransitionState.Exiting:
              scene.Exit();
              break;
            default:
              Console.WriteLine("Transition from '{0}' to '{1}' is invalid", last, current);
              break;
          }
        }
        if ( last == TransitionState.Pausing ) {
          if ( current == TransitionState.None ) {

          }
        }
        if ( last == TransitionState.Resuming ) {
          if ( current == TransitionState.None ) {
            _scenes.RemoveAt(_scenes.Count - 1);
          }
        }
        if ( last == TransitionState.Initializing ) {
          if ( current == TransitionState.None ) {
            _scenes.Add(_nextScene);
            _nextScene = null;
          }
        }
        if ( last == TransitionState.Exiting ) {
          if ( current == TransitionState.Null ) {
            _scenes.RemoveAt(_scenes.Count - 1);
          }
        }
      } else {
        switch ( current ) {
          case TransitionState.Null:
            if ( _scenes.Count > 0 ) {
              _scenes.RemoveAt(_scenes.Count - 1);
            }
            break;
          case TransitionState.None:
            break;
          case TransitionState.Pausing:
            scene.Pause();
            break;
          case TransitionState.Resuming:
            scene.Resume();
            break;
          case TransitionState.Exiting:
            scene.Exit();
            break;
          case TransitionState.Initializing:
            scene.Initialize();
            break;
          default:
            Console.WriteLine("Transition from '{0}' to '{1}' is invalid", last, current);
            break;
        }
      }
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

