//
// 	MenuView.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 2/11/2016.
// 	Copyright  All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.UI;

namespace MidnightBlue
{
  public class MenuView : UIView
  {
    private bool _shouldExit;

    public MenuView(ContentManager content)
      : base(30, 30)
    {
      var font = content.Load<SpriteFont>("Fonts/Bender Large");
      var select = content.Load<SoundEffect>("Audio/select");
      var confirm = content.Load<SoundEffect>("Audio/confirm");
      _shouldExit = false;

      var layout = new Layout(this, 6, 30) {
        BackgroundColor = UIColors.Background,
        BorderTopColor = UIColors.Border,
        BorderBottomColor = UIColors.Border,
        BorderWidth = 3,
        BorderDisplayed = true,
        Tag = "Map"
      };
      this.Add(layout, 10, 10, 7, 10);

      var continueBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = UIColors.HighlightedMenuText,
        TextContent = "Continue",
        Font = font,
        HighlightedSound = select,
        PressedSound = confirm.CreateInstance(),
      };

      var quitBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = UIColors.HighlightedMenuText,
        TextContent = "Back to Menu",
        Font = font,
        HighlightedSound = select,
        PressedSound = confirm.CreateInstance(),
        Tag = "Quit Button",
      };

      continueBtn.OnPress += (sender, e) =>
        _shouldExit = true;

      layout.Add(continueBtn, 1, 11, 2, 13);
      layout.Add(quitBtn, 3, 8, 2, 18);
    }

    public bool ShouldExit
    {
      get { return _shouldExit; }
    }
  }
}
