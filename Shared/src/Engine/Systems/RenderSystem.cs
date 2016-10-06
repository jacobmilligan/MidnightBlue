﻿//
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
    private int _drawn;

    public RenderSystem(SpriteBatch spriteBatch)
      : base(typeof(SpriteComponent))
    {
      _spriteBatch = spriteBatch;
    }

    protected override void PreProcess()
    {
      _drawn = 0;
      AssociatedEntities.Sort((x, y) => {
        var xSprite = x.GetComponent<SpriteComponent>();
        var ySprite = y.GetComponent<SpriteComponent>();

        return xSprite.Z.CompareTo(ySprite.Z);
      });
    }

    protected override void Process(Entity entity)
    {
      var sprite = entity.GetComponent<SpriteComponent>();
      if ( sprite != null && sprite.Target.IsVisible ) {
        //TODO: Implement camera system instead
        if ( MBGame.Camera.GetBoundingRectangle().Intersects((Rectangle)sprite.Bounds) ) {
          _spriteBatch.Draw(sprite.Target);
          _drawn++;
        }
      }
    }
  }
}
