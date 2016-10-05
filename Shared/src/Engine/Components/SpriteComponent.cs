//
// 	Sprite.cs
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
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MidnightBlue.Engine.EntityComponent
{
  public class SpriteComponent : IComponent
  {
    private void SetDefaults(Vector2 position, Vector2 scale)
    {
      Target.Position = position;
      Target.Scale = scale;
      Rotation = 0.0f;
      Z = 0;
    }

    public SpriteComponent(Texture2D texture, Vector2 position, Vector2 scale)
    {
      Target = new Sprite(texture);
      SetDefaults(position, scale);
    }

    public SpriteComponent(TextureRegion2D texture, Vector2 position, Vector2 scale)
    {
      Target = new Sprite(texture);
      SetDefaults(position, scale);
    }

    public Sprite Target { get; set; }

    // Direction and Rotation implementation derived from Monogame.Extended SpaceGame Demo source code:
    // https://github.com/craftworkgames/MonoGame.Extended/blob/develop/Source/Demos/Demo.SpaceGame/Entities/Spaceship.cs

    public Vector2 Direction
    {
      get
      {
        return Vector2.UnitX.Rotate(Rotation);
      }
    }

    public float Rotation
    {
      get { return Target.Rotation - MathHelper.ToRadians(90); }
      set { Target.Rotation = value + MathHelper.ToRadians(90); }
    }

    public int Z { get; set; }
  }
}
