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
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

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
      var normal = Content.Load<Texture2D>("Images/blue_button00");
      var down = Content.Load<Texture2D>("Images/blue_button01");
      var font = Content.Load<SpriteFont>("Bender");
      var btn = new UIControlElement(2, 2, normal, down, down) {
        Font = font,
        TextColor = Color.White,
        TextContent = "Test Button"
      };
      var layout1 = new Layout(20, 20) {
        BorderColor = Color.Black,
        BorderWidth = 2,
      };
      _ui.Add(layout1, 2, 2, 10, 10);
      layout1.Add(btn, 1, 1, 4, 4);
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
  }
}
