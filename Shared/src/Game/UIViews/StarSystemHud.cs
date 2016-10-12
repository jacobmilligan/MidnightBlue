//
// 	StarSystemHud.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 12/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework.Content;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.UI;

namespace MidnightBlue
{
  public class StarSystemHud : UIView
  {
    public StarSystemHud(ContentManager content, EntityMap gameObjects, SceneStack scenes) : base(25, 25)
    {
      var map = new Layout(this, 10, 7) {
        BackgroundColor = UIColors.Background,
        BorderTopColor = UIColors.Border,
        BorderBottomColor = UIColors.Border,
        BorderWidth = 3,
        BorderDisplayed = true,
        Tag = "Map"
      };

      this.Add(map, 15, 1, 5, 4);
    }
  }
}
