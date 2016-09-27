//
// Scene.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine.Scenes
{
  public abstract class Scene
  {
    private EntityMap _gameObjects;

    private Scene()
    {
      _gameObjects = new EntityMap();
    }

    public Scene(EntityMap gameObjects)
    {
      _gameObjects = new EntityMap(gameObjects);
      _gameObjects.Clear();
    }

    public abstract void Initialize();
    public abstract void HandleInput();
    public abstract void Update();
    public abstract void Draw(SpriteBatch spriteBatch);

    public void Cleanup()
    {
      _gameObjects.Clear();
    }

    protected EntityMap GameObjects
    {
      get { return _gameObjects; }
    }

    public ContentManager Content { get; set; }
  }
}

