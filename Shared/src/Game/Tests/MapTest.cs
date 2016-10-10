//
// 	MapTest.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 9/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Testing
{
  public struct MapCell
  {
    public Rectangle Rect { get; set; }
    public Color Color { get; set; }
  }

  public class MapTest : Scene
  {
    OpenSimplexNoise _noise;
    MapCell[,] _cells;
    const int _cellSize = 8;
    const int _mapSize = 64;

    public MapTest(EntityMap map, ContentManager content) : base(map, content)
    {
      _noise = new OpenSimplexNoise();
      _cells = new MapCell[_mapSize, _mapSize];
    }

    /// <summary>
    /// Gets a noise value. Adapted from Christian Maher. Source: https://cmaher.github.io/posts/working-with-simplex-noise/
    /// </summary>
    /// <returns>The noise value.</returns>
    /// <param name="iterations">Number of iterations to calculate.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="persistance">Persistance of scale in each iteration.</param>
    /// <param name="scale">Scale for x and y.</param>
    /// <param name="low">Lower bound.</param>
    /// <param name="high">Upper bound.</param>
    private float GetNoise(int iterations, float x, float y, float persistance, float scale, int low, int high)
    {
      var maxAmp = 0.0f;
      var amp = 1.0f;
      var freq = scale;
      var noise = 0.0f;

      // add successively smaller, higher-frequency terms
      for ( var i = 0; i < iterations; i++ ) {
        noise += (float)_noise.Evaluate(x * freq, y * freq) * amp;
        maxAmp += amp;
        amp *= persistance;
        freq *= 2;
      }

      //take the average value of the iterations
      noise /= maxAmp;

      //normalize the result
      noise = noise * (high - low) / 2 + (high + low) / 2;

      return noise;
    }

    public override void Initialize()
    {
      var width = _cells.GetLength(1);
      var height = _cells.GetLength(0);
      var camPos = MBGame.Camera.Position;

      var scale = 0.007f;
      for ( int y = 0; y < height; y++ ) {
        for ( int x = 0; x < width; x++ ) {
          var noise = GetNoise(5, x, y, 0.5f, scale, 0, 255);

          _cells[y, x] = new MapCell();
          _cells[y, x].Rect = new Rectangle(
            (int)(camPos.X + _cellSize * x),
            (int)(camPos.Y + _cellSize * y),
            _cellSize,
            _cellSize
          );

          var clr = Color.SeaGreen;
          clr.R *= (byte)noise;
          _cells[y, x].Color = clr;
        }
      }

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
      foreach ( var cell in _cells ) {
        spriteBatch.FillRectangle(cell.Rect, cell.Color);
      }
      foreach ( var cell in _cells ) {
        spriteBatch.FillRectangle(
          cell.Rect.X + _mapSize * _cellSize,
          cell.Rect.Y,
          cell.Rect.Width,
          cell.Rect.Height,
          cell.Color
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
