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
      : base(typeof(StarSystem))
    {
      _content = content;
      _spriteBatch = spriteBatch;
      _font = _content.Load<SpriteFont>("Bender");
    }

    protected override void Process(Entity entity)
    {
      var star = entity.GetComponent<StarSystem>();

      if ( star != null && star.Draw ) {

        var textPosition = GetCenter(
          MBGame.Camera.GetBoundingRectangle().Center,
          new Vector2(_font.MeasureString(star.Name).X / 2, _font.MeasureString(star.Name).Y / 2)
        );

        var starInfo = string.Format("{0}\nRadius: {1}\nPlanets:\n{2}", star.Name, star.Radius, star.PlanetList);

        _spriteBatch.DrawString(
          _font,
          starInfo,
          textPosition,
          Color.White
        );
      }
    }

    private Vector2 GetCenter(Vector2 parentCenter, Vector2 childCenter)
    {
      return new Vector2(parentCenter.X - childCenter.X, parentCenter.Y - 200);
    }
  }
}
