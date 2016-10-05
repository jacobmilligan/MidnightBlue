//
// 	GalaxyRenderSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{
  public class GalaxyRenderSystem : EntitySystem
  {
    private SpriteBatch _spriteBatch;
    private ContentManager _content;
    private SpriteFont _font;

    public GalaxyRenderSystem(SpriteBatch spriteBatch, ContentManager content)
      : base(typeof(StarSystemComponent))
    {
      _content = content;
      _spriteBatch = spriteBatch;
      _font = _content.Load<SpriteFont>("Bender");
    }

    protected override void Process(Entity entity)
    {
      var star = entity.GetComponent<StarSystemComponent>();

      if ( star != null && star.Draw ) {
        _spriteBatch.DrawString(
          _font,
          star.Name,
          MBGame.Camera.Position,
          Color.White
        );
      }
    }
  }
}
