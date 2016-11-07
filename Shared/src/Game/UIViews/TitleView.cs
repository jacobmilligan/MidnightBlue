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
using MB2D;
using MB2D.EntityComponent;
using MB2D.Scenes;
using MB2D.UI;

namespace MidnightBlue
{
  /// <summary>
  /// The title screens UI view
  /// </summary>
  public class TitleView : UIView
  {
    /// <summary>
    /// The main UI font for the title screen.
    /// </summary>
    private SpriteFont _benderLarge,
    /// <summary>
    /// Font to use with headers.
    /// </summary>
    _serifGothic;

    /// <summary>
    /// Sound effect to play when hovering over an item.
    /// </summary>
    private SoundEffect _select,
    /// <summary>
    /// Sound effect to play when clicking on an item.
    /// </summary>
    _confirm;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.TitleView"/> class.
    /// </summary>
    /// <param name="content">Content to load sounds, fonts, and textures from.</param>
    /// <param name="gameObjects">Game objects to track.</param>
    /// <param name="scenes">Scenes to use in UI interactions.</param>
    public TitleView(ContentManager content, EntityMap gameObjects, SceneStack scenes) : base(25, 25)
    {
      _benderLarge = content.Load<SpriteFont>("Fonts/Bender Large");
      _serifGothic = content.Load<SpriteFont>("Fonts/SerifGothicBlack");
      _select = content.Load<SoundEffect>("Audio/select");
      _confirm = content.Load<SoundEffect>("Audio/confirm");

      this.BackgroundTexture = content.Load<Texture2D>("Images/TitleBackground");

      var opts = new Layout(this, 20, 20);

      var titleLayout = new Layout(this, 5, 6) {
        BorderDisplayed = true,
        BorderColor = UIColors.NormalMenuText,
        BorderWidth = 2,
        BorderRightColor = Color.Transparent,
        BorderLeftColor = Color.Transparent
      };

      this.Add(opts, 9, 9, 8, 6);
      this.Add(titleLayout, 2, 9, (int)titleLayout.Size.Y, (int)titleLayout.Size.X);

      var titleA = new Label {
        TextContent = "Midnight",
        Font = _serifGothic,
        TextColor = UIColors.NormalMenuText,
      };
      var titleB = new Label {
        TextContent = "Blue",
        Font = _serifGothic,
        TextColor = UIColors.NormalMenuText
      };

      titleLayout.Add(titleA, 2, 2, 10, 10);
      titleLayout.Add(titleB, 4, 3, 10, 10);

      var newGameBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = UIColors.HighlightedMenuText,
        TextContent = "New Game",
        Font = _benderLarge,
        Fill = true,
        HighlightedSound = _select,
        PressedSound = _confirm.CreateInstance()
      };

      var continueBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = UIColors.HighlightedMenuText,
        TextContent = "Continue",
        Font = _benderLarge,
        Fill = true,
        HighlightedSound = _select,
        PressedSound = _confirm.CreateInstance()
      };

      var quitBtn = new Button {
        NormalTextColor = Color.White,
        HighlightedTextColor = UIColors.HighlightedMenuText,
        TextContent = "Quit",
        Font = _benderLarge,
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

      newGameBtn.OnPress += (sender, e) => scenes.ResetTo(new GalaxyScene(gameObjects, content));

      opts.Add(newGameBtn, 1, 2, 7, 20);
      opts.Add(continueBtn, 8, 4, 7, 15);
      opts.Add(quitBtn, 15, 8, 7, 7);
    }
  }
}
