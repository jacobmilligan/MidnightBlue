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

namespace MidnightBlue
{
  public class TitleScene : Scene
  {
    private TitleView _ui;
    private Song _bgSong;

    public TitleScene(EntityMap map, ContentManager content) : base(map, content)
    {
      WindowBackgroundColor = Color.Black;

      _bgSong = content.Load<Song>("Audio/Title");
    }

    public override void Initialize()
    {
      _ui = new TitleView(Content, GameObjects, SceneController);
      if ( MediaPlayer.GameHasControl ) {
        MediaPlayer.Play(_bgSong);
        MediaPlayer.IsRepeating = true;
      }
      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
      _ui.Update();
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      _ui.Draw(uiSpriteBatch);
    }

    public override void Exit()
    {
      MediaPlayer.Stop();
      TransitionState = TransitionState.Null;
    }

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

