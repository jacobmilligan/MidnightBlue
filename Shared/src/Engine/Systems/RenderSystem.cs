//
// 	RenderSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

namespace MidnightBlue.Engine.EntityComponent
{
  public class RenderSystem : EntitySystem
  {
    private SpriteBatch _spriteBatch;

    public RenderSystem(SpriteBatch spriteBatch)
      : base(typeof(SpriteComponent))
    {
      _spriteBatch = spriteBatch;
    }

    protected override void Process(Entity entity)
    {
      var sprite = entity.GetComponent<SpriteComponent>();
      if ( sprite != null ) {
        var spriteRect = (Rectangle)(sprite.Target.GetBoundingRectangle());
        if ( MBGame.Graphics.Viewport.Bounds.Intersects(spriteRect) ) {
          _spriteBatch.Draw(sprite.Target);
        }
      }
    }
  }
}
