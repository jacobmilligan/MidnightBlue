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
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;

namespace MidnightBlue.Testing
{
  public class MapTest : Scene
  {
    private Planet _planet;

    public MapTest(EntityMap map, ContentManager content) : base(map, content)
    {
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
        _planet = new Planet(
          new PlanetMetadata {
            Radius = 300000,
            SurfaceTemperature = -80,
            Type = PlanetType.Water
          }, 100
        );
        _planet.Generate();
      }

      //uiSpriteBatch.Draw(_planet.GetMapLayer("elevation"), new Vector2(0, 0));
      uiSpriteBatch.Draw(_planet.GetMapLayer("map"), new Vector2(0, 0));
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
