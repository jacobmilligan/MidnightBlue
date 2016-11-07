//
// 	MenuScene.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 2/11/2016.
// 	Copyright  All rights reserved
//
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MB2D.EntityComponent;
using MB2D.Scenes;
using MB2D.UI;

namespace MidnightBlue
{
  public class MenuScene : Scene
  {
    private MenuView _ui;

    public MenuScene(ContentManager content)
      : base(null, content)
    {
      _ui = new MenuView(Content);
      var quit = (Button)_ui["Quit Button"];
      quit.OnPress += (sender, e) => {
        SceneController.ResetTo(new InitScene(SceneController.Bottom.GameObjects, Content));
      };
    }

    /// <summary>
    /// Creates the UIView and starts the background music.
    /// </summary>
    public override void Initialize()
    {
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Handles the input for the menu.
    /// </summary>
    public override void HandleInput()
    {
      var mainGameObjects = SceneController.Bottom.GameObjects;
      mainGameObjects.GetSystem<InputSystem>().Run();
    }

    /// <summary>
    /// Updates the UI
    /// </summary>
    public override void Update()
    {
      _ui.Update();
      if ( _ui.ShouldExit )
        SceneController.Pop();
    }

    /// <summary>
    /// Draws the UI to the uiSpriteBatch
    /// </summary>
    /// <param name="spriteBatch">Sprite batch for world-based entities.</param>
    /// <param name="uiSpriteBatch">User interface sprite batch.</param>
    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      var gameScene = SceneController.SceneAt(SceneController.LastIndex - 1);
      gameScene.Draw(spriteBatch, uiSpriteBatch);

      _ui.Draw(spriteBatch, uiSpriteBatch);
    }

    /// <summary>
    /// Exits the menu
    /// </summary>
    public override void Exit()
    {
      TransitionState = TransitionState.Null;
    }

    /// <summary>
    /// Pauses the scene
    /// </summary>
    public override void Pause()
    {
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Resumes the scene
    /// </summary>
    public override void Resume()
    {
      TransitionState = TransitionState.None;
    }
  }
}
