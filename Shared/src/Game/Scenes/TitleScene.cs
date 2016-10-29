//
// TitleScene.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  /// <summary>
  /// The scene shown at the title screen.
  /// </summary>
  public class TitleScene : Scene
  {
    /// <summary>
    /// The titles user interface
    /// </summary>
    private TitleView _ui;

    /// <summary>
    /// Song to play at the title screen
    /// </summary>
    private Song _bgSong;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.TitleScene"/> class.
    /// </summary>
    /// <param name="map">Game objects.</param>
    /// <param name="content">Content manager for loading textures and sounds.</param>
    public TitleScene(EntityMap map, ContentManager content) : base(map, content)
    {
      WindowBackgroundColor = Color.Black;

      _bgSong = content.Load<Song>("Audio/Title");
    }

    /// <summary>
    /// Creates the UIView and starts the background music.
    /// </summary>
    public override void Initialize()
    {
      _ui = new TitleView(Content, GameObjects, SceneController);
      if ( MediaPlayer.GameHasControl ) {
        MediaPlayer.Play(_bgSong);
        MediaPlayer.IsRepeating = true;
      }

      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Handles the input for the menu.
    /// </summary>
    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    /// <summary>
    /// Updates the UI
    /// </summary>
    public override void Update()
    {
      _ui.Update();
    }

    /// <summary>
    /// Draws the UI to the uiSpriteBatch
    /// </summary>
    /// <param name="spriteBatch">Sprite batch for world-based entities.</param>
    /// <param name="uiSpriteBatch">User interface sprite batch.</param>
    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      _ui.Draw(spriteBatch, uiSpriteBatch);
    }

    /// <summary>
    /// Stops the music and quits instantly
    /// </summary>
    public override void Exit()
    {
      MediaPlayer.Stop();
      TransitionState = TransitionState.Null;
    }

    /// <summary>
    /// Pauses the title screen, fading music while it does so.
    /// </summary>
    public override void Pause()
    {
      var fadeSpeed = 0.8f;
      if ( MediaPlayer.Volume > 0 ) {
        MediaPlayer.Volume -= fadeSpeed;
      } else {
        MediaPlayer.Pause();
        TransitionState = TransitionState.None;
      }
    }

    /// <summary>
    /// Resumes the title screen, fading music in while it does so.
    /// </summary>
    public override void Resume()
    {
      var fadeSpeed = 2f;

      MediaPlayer.Resume();

      if ( MediaPlayer.Volume < 100 ) {
        MediaPlayer.Volume += DeltaTime * fadeSpeed;
      } else {
        TransitionState = TransitionState.None;
      }

    }

  }
}

