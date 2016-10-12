//
// 	SimplexTest.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;

namespace MidnightBlue.Testing
{
  public class MapTest : Scene
  {
    private Planet _planet, _planet2;
    private SpriteFont _font;

    public MapTest(EntityMap map, ContentManager content) : base(map, content)
    {
      _font = content.Load<SpriteFont>("Fonts/Bender Large");
    }

    public override void Initialize()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      GameObjects.GetSystem<RenderSystem>().Run();

      if ( _planet == null ) {
        var length = new Length((ulong)(Length.AstronomicalUnit * 5.9) * 1000);
        _planet = new Planet(
          new PlanetMetadata {
            Radius = 400000,
            SurfaceTemperature = 20,
            Type = PlanetType.Terrestrial,
            StarDistance = new Length(length.Kilometers)
          }, 10490
        );
        _planet.Generate();
        _planet.CreateMapTexture(Content);
      }

      if ( _planet2 == null ) {
        var length = new Length((ulong)(Length.AstronomicalUnit * 5.9) * 1000);
        _planet2 = new Planet(
          new PlanetMetadata {
            Radius = 400000,
            SurfaceTemperature = 20,
            Type = PlanetType.Terrestrial,
            StarDistance = new Length(length.Kilometers)
          }, 10490
        );
        _planet2.Generate();
        _planet2.CreateMapTexture(Content);
      }

      //uiSpriteBatch.Draw(_planet.GetMapLayer("elevation"), new Vector2(0, 0));
      uiSpriteBatch.Draw(_planet.GetMapLayer("map"), new Vector2(0, 0));
      uiSpriteBatch.Draw(_planet.GetMapLayer("planet map"), new Vector2(_planet.Size.X, 0));

      uiSpriteBatch.Draw(_planet2.GetMapLayer("map"), new Vector2(0, _planet2.Size.Y));
      uiSpriteBatch.Draw(_planet2.GetMapLayer("planet map"), new Vector2(_planet2.Size.X, _planet2.Size.Y));

      //FIXME: Remove method after debugging
      var mouse = Mouse.GetState().Position;
      if ( new Rectangle(0, 0, _planet.Size.X, _planet.Size.Y).Contains(mouse) ) {
        var tile = _planet.Tiles[mouse.X, mouse.Y];
        var strSize = _font.MeasureString(tile.Biome.ToString());

        uiSpriteBatch.DrawString(
          _font,
          tile.Biome.ToString(),
          new Vector2(MBGame.Graphics.Viewport.Bounds.Right - strSize.X, 0),
          Color.White
        );
      }
    }

    public override void Exit()
    {
      // End transition instantly
      TransitionState = TransitionState.Null;
    }

    public override void Pause()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    public override void Resume()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

  }
}
