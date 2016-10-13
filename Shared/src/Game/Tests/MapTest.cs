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
using MidnightBlue.Engine.Tiles;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Testing
{
  public class MapTest : Scene
  {
    private Planet _planet;
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
      GameObjects.GetSystem<ShipInputSystem>().Run();
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      if ( _planet == null ) {
        var seed = 1050;
        var length = new Length((ulong)(Length.AstronomicalUnit * 0.6359717) * 1000);
        _planet = new Planet(
            new PlanetMetadata {
              Radius = 142987,
              SurfaceTemperature = (float)-1.293776E+10,
              Type = PlanetType.Terrestrial,
              StarDistance = new Length(length.Kilometers),
              Water = 77704,
              Carbon = 80432,
              Density = 3
            }, seed);
        _planet.Generate(new Random(seed));
        _planet.CreateMapTexture(Content);
      }

      uiSpriteBatch.Draw(_planet.GetMapLayer("planet map"), new Vector2(0, 0));

      //var tile = _planet.Tiles[pos.X, pos.Y];
      //var strSize = _font.MeasureString(tile.Biome.ToString());

      GameObjects.GetSystem<RenderSystem>().Run();
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
