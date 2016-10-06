﻿//
// 	CollisionRenderSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.Testing
{
  public class CollisionRenderSystem : EntitySystem
  {
    private SpriteBatch _spriteBatch;
    public CollisionRenderSystem(SpriteBatch spriteBatch)
      : base(typeof(CollisionComponent), typeof(StarSystem), typeof(SpriteComponent))
    {
      _spriteBatch = spriteBatch;
    }

    protected override void Process(Entity entity)
    {
      var collision = entity.GetComponent<CollisionComponent>();
      var sprite = entity.GetComponent<SpriteComponent>();

      if ( sprite != null && MBGame.Camera.Contains(sprite.Target.Position) == ContainmentType.Contains ) {
        _spriteBatch.DrawRectangle(sprite.Target.GetBoundingRectangle(), Color.Yellow);
        if ( collision != null ) {
          foreach ( var b in collision.Boxes ) {
            _spriteBatch.DrawRectangle(b, Color.Red);
          }
        }
      }

    }
  }
}