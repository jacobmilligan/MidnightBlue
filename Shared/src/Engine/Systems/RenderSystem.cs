//
// 	RenderSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 4/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

namespace MidnightBlue.Engine.EntityComponent
{
  public class RenderSystem : EntitySystem
  {
    private SpriteBatch _spriteBatch;
    private int _drawn;
    private RectangleF _cameraRect;

    public RenderSystem(SpriteBatch spriteBatch)
      : base(typeof(SpriteComponent))
    {
      _spriteBatch = spriteBatch;
    }

    protected override void PreProcess()
    {
      _drawn = 0;
      AssociatedEntities = AssociatedEntities.OrderBy(
        entity => entity.GetComponent<SpriteComponent>().Z
      ).ToList();
      _cameraRect = MBGame.Camera.GetBoundingRectangle();
    }

    protected override void Process(Entity entity)
    {
      var sprite = entity.GetComponent<SpriteComponent>();

      if ( sprite != null && _cameraRect.Intersects((Rectangle)sprite.Bounds) ) {
        _spriteBatch.Draw(sprite.Target);
        _drawn++;
      }
    }
  }
}
