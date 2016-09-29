//
// 	SpriteBatchExtensions.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 28/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.Geometry;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine
{
  public static class SpriteBatchExtensions
  {
    public static void DrawGrid(this SpriteBatch spriteBatch, Grid grid, Point position, Color color)
    {
      var rowCount = grid.RowCount;
      var colCount = grid.ColCount;
      for ( int row = 0; row < rowCount; row++ ) {
        for ( int col = 0; col < colCount; col++ ) {
          var rect = new RectangleF(
            position.X + (col * grid.ColSize),
            position.Y + (row * grid.RowSize),
            grid.ColSize,
            grid.RowSize
          );
          spriteBatch.DrawRectangle(rect, color, 1);
        }
      }
    }
  }
}
