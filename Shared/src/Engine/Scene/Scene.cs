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
  /// <summary>
  /// Holds all logic and data for a single game screen
  /// </summary>
  public abstract class Scene
  {
    /// <summary>
    /// The scenes EntityMap
    /// </summary>
    private EntityMap _gameObjects;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.Scenes.Scene"/> class
    /// </summary>
    private Scene()
    {
      _gameObjects = new EntityMap();
      WindowBackgroundColor = Color.MidnightBlue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.Scenes.Scene"/> class
    /// with a pre-existing EntityMap
    /// </summary>
    /// <param name="gameObjects">EntityMap to assign to the scene.</param>
    public Scene(EntityMap gameObjects)
    {
      _gameObjects = new EntityMap(gameObjects);
      _gameObjects.Clear();
    }

    /// <summary>
    /// Initialize this scene and loads all resources.
    /// </summary>
    public abstract void Initialize();
    public abstract void HandleInput();
    public abstract void Update();
    public abstract bool Pause();
    public abstract bool Resume();
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Exit();

    public void Cleanup()
    {
      Exit();
      _gameObjects.Clear();
    }

    protected EntityMap GameObjects
    {
      get { return _gameObjects; }
    }

    public bool Pausing { get; set; }
    public bool Resuming { get; set; }

    public ContentManager Content { get; set; }
    public Color WindowBackgroundColor { get; set; }
    public SceneStack SceneController { get; set; }

    public float DeltaTime { get; set; }
  }
}

