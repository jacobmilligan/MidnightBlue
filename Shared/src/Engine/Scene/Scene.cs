//
// Scene.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine.Scenes
{
  public enum TransitionState
  {
    Null,
    None,
    Pausing,
    Resuming,
    Exiting,
    Initializing
  }

  /// <summary>
  /// Holds all logic and data for a single game screen
  /// </summary>
  public abstract class Scene
  {
    private TransitionState _lastState;
    /// <summary>
    /// The scenes EntityMap
    /// </summary>
    private EntityMap _gameObjects;

    private ContentManager _content;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.Scenes.Scene"/> class
    /// </summary>
    private Scene(ContentManager content)
    {
      _gameObjects = new EntityMap();
      WindowBackgroundColor = Color.MidnightBlue;
      _lastState = TransitionState.Null;
      TransitionState = TransitionState.Initializing;
      _content = content;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.Scenes.Scene"/> class
    /// with a pre-existing EntityMap
    /// </summary>
    /// <param name="gameObjects">EntityMap to assign to the scene.</param>
    public Scene(EntityMap gameObjects, ContentManager content)
    {
      _gameObjects = new EntityMap(gameObjects);
      _gameObjects.Clear();
      _content = content;
    }

    public void UpdateTransition()
    {
      _lastState = TransitionState;
    }

    /// <summary>
    /// Initialize this scene and loads all resources.
    /// </summary>
    public abstract void Initialize();
    public abstract void HandleInput();
    public abstract void Update();
    public abstract void Pause();
    public abstract void Resume();
    public abstract void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch);
    public abstract void Exit();

    public void Cleanup()
    {
      _gameObjects.Clear();
    }

    protected EntityMap GameObjects
    {
      get { return _gameObjects; }
    }

    public TransitionState TransitionState { get; set; }
    public TransitionState PreviousTransitionState
    {
      get { return _lastState; }
    }

    public Color WindowBackgroundColor { get; set; }
    public SceneStack SceneController { get; set; }
    protected ContentManager Content
    {
      get { return _content; }
    }

    public float DeltaTime { get; set; }
  }
}

