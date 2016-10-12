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
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MidnightBlue.Engine.EntityComponent
{
  public class SpriteTransform : IComponent
  {
    private void SetDefaults(Vector2 position, Vector2 scale)
    {
      DeltaPosition = new Vector2(0, 0);
      DeltaSize = new Vector2(0, 0);

      Target.Position = position;
      Target.Scale = scale;
      Rotation = 0.0f;
      Z = 0;
      Bounds = Target.GetBoundingRectangle();
    }

    public SpriteTransform(Texture2D texture, Vector2 position, Vector2 scale)
    {
      Target = new Sprite(texture);
      SetDefaults(position, scale);
    }

    public SpriteTransform(TextureRegion2D texture, Vector2 position, Vector2 scale)
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

    public Vector2 DeltaPosition { get; set; }
    public Vector2 DeltaSize { get; set; }

    public RectangleF Bounds { get; set; }

    public float Z { get; set; }
  }
}
