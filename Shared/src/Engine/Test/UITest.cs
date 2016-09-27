//
// 	UITest.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.UI;

namespace MidnightBlue.Engine.Testing
{
  public class UITest : Scene
  {
    private UIContext _ui;

    public UITest(EntityMap map) : base(map)
    {
      _ui = new UIContext(20, 20);
    }

    public override void Initialize()
    {
      _ui.Add(new Layout(20, 20), 2, 2, 10, 10);
    }

    public override void HandleInput()
    {

    }

    public override void Update()
    {

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      _ui.Draw(spriteBatch);
    }
  }
}
