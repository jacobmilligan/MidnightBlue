//
// 	MapTest2.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 10/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine
{
  public class MapTest2 : Scene
  {
    private const int _cellSize = 1;
    private DiamondSquare _generator;
    private Texture2D _map;

    public MapTest2(EntityMap map, ContentManager content) : base(map, content)
    {
      _generator = new DiamondSquare(128, true);
      _generator.Generate(32);
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
      if ( _map == null ) {

        uiSpriteBatch.End();
        uiSpriteBatch.Begin();
        var target = new RenderTarget2D(MBGame.Graphics, _generator.Size, _generator.Size);

        MBGame.Graphics.SetRenderTarget(target);
        MBGame.Graphics.Clear(Color.Black);

        for ( int x = 0; x < _generator.Size; x++ ) {
          for ( int y = 0; y < _generator.Size; y++ ) {
            var clr = Color.Lerp(Color.Black, Color.White, (float)_generator.GetValue(x, y));
            uiSpriteBatch.FillRectangle(
              x * _cellSize,
              y * _cellSize,
              _cellSize,
              _cellSize,
              clr
            );
          }
        }

        uiSpriteBatch.End();
        MBGame.Graphics.SetRenderTarget(null);

        _map = target;

        uiSpriteBatch.Begin();
      }

      var scaleVector = new Vector2(4, 4);
      uiSpriteBatch.Draw(_map, new Vector2(0, 0), scale: scaleVector);
      uiSpriteBatch.Draw(_map, new Vector2(_map.Width * scaleVector.X, 0), scale: scaleVector);

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
