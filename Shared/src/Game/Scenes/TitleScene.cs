//
// TitleScene.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scene;

namespace MidnightBlue
{
  public class TitleScene : Scene
  {

    public TitleScene(ECSMap map) : base(map) { }

    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Initialize()
    {

    }

    public override void Update()
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
    }
  }
}

