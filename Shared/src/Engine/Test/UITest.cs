//
// 	UITest.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.UI;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MidnightBlue.Engine.Testing
{
  public class UITest : Scene
  {
    private TestUIView _ui;

    public UITest(EntityMap map) : base(map)
    {
    }

    public override void Initialize()
    {
      _ui = new TestUIView(Content, 20, 20);
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
      _ui.Update();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      _ui.Draw(spriteBatch);
    }

    public override void Exit()
    {

    }

    public override bool Pause()
    {
      return false;
    }

    public override bool Resume()
    {
      return false;
    }
  }
}
