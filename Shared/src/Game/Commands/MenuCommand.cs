//
// 	MenuCommand.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 2/11/2016.
// 	Copyright  All rights reserved
//
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.IO;
using MidnightBlue.Engine.Scenes;

namespace MidnightBlue
{
  public class MenuCommand : Command
  {
    private SceneStack _sceneController;
    private ContentManager _content;

    public MenuCommand(
      Keys key, CommandType commandType, SceneStack sceneController, ContentManager content
    ) : base(key, commandType)
    {
      _sceneController = sceneController;
      _content = content;
    }

    protected override void OnKeyPress(Entity e = null)
    {
      if ( _sceneController.Top.GetType() == typeof(MenuScene) ) {
        _sceneController.Pop();
        return;
      }

      _sceneController.Push(new MenuScene(_content));
    }
  }
}
