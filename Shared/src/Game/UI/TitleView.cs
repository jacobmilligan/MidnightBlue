//
// 	TitleView.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 3/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.UI;

namespace MidnightBlue
{
  public class TitleView : UIView
  {
    private Color _superNovaYellow = new Color(251, 186, 48);
    private Color _grayBlue = new Color(142, 189, 245);
    private SpriteFont _horatio, _serifGothic;
    private SoundEffect _select, _confirm;

    public TitleView(ContentManager content, EntityMap gameObjects, SceneStack scenes) : base(25, 25)
    {
      _horatio = content.Load<SpriteFont>("Fonts/HoratioLarge");
      _serifGothic = content.Load<SpriteFont>("Fonts/SerifGothicBlack");
      _select = content.Load<SoundEffect>("Audio/select");
      _confirm = content.Load<SoundEffect>("Audio/confirm");

      this.BackgroundTexture = content.Load<Texture2D>("Images/TitleBackground");

      var opts = new Layout(20, 20) {
        BaseTexture = content.Load<Texture2D>("Images/uiback")
      };

      var titleLayout = new Layout(5, 6) {
        BorderDisplayed = true,
        BorderColor = _superNovaYellow,
        BorderWidth = 2,
        BorderRightColor = Color.Transparent,
        BorderLeftColor = Color.Transparent
      };

      this.Add(opts, 9, 9, 8, 6);
      this.Add(titleLayout, 2, 9, (int)titleLayout.Size.Y, (int)titleLayout.Size.X);

      var titleA = new Label {
        TextContent = "Midnight",
        Font = _serifGothic,
        TextColor = _superNovaYellow,
      };
      var titleB = new Label {
        TextContent = "Blue",
        Font = _serifGothic,
        TextColor = _superNovaYellow
      };

      titleLayout.Add(titleA, 1, 1, 10, 10);
      titleLayout.Add(titleB, 3, 2, 10, 10);

      var newGameBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = _grayBlue,
        TextContent = "New Game",
        Font = _horatio,
        Fill = true,
        HighlightedSound = _select,
        PressedSound = _confirm.CreateInstance()
      };

      var continueBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = _grayBlue,
        TextContent = "Continue",
        Font = _horatio,
        Fill = true,
        HighlightedSound = _select,
        PressedSound = _confirm.CreateInstance()
      };

      var quitBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = _grayBlue,
        TextContent = "Quit",
        Font = _horatio,
        Fill = true,
        HighlightedSound = _select,
        PressedSound = _confirm.CreateInstance()
      };

      quitBtn.OnPress += (sender, e) => {
        while ( quitBtn.PressedSound.State != SoundState.Stopped ) {
          MediaPlayer.Volume -= 0.0001f * scenes.Top.DeltaTime;
        }
        MBGame.ForceQuit = true;
      };

      newGameBtn.OnPress += (sender, e) => scenes.ResetTo(
              new GalaxyScene(gameObjects),
              content
            );

      opts.Add(newGameBtn, 1, 2, 7, 20);
      opts.Add(continueBtn, 8, 4, 7, 15);
      opts.Add(quitBtn, 15, 8, 7, 7);
    }
  }
}
