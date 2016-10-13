//
// 	TileMap.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 13/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;
using MonoGame.Extended.TextureAtlases;

namespace MidnightBlue.Engine.Tiles
{
  public class TileMap
  {
    private int _width, _height, _cellSize, _offset;
    private Point _realCellSize;
    private Vector2 _scale;
    private TextureAtlas _atlas;
    private Tile[,] _tiles;

    public void Fill(Tile[,] tiles)
    {
      _width = tiles.GetLength(0);
      _height = tiles.GetLength(1);
      _tiles = tiles;
    }

    public TileMap(
      Texture2D texture, int cellSize, int margin = 0, int spacing = 0, int offset = 0, float scale = 1.0f
    )
    {
      _width = texture.Width;
      _height = texture.Height;
      _cellSize = cellSize;
      _offset = offset;
      _scale = new Vector2(scale, scale);
      _realCellSize = (_cellSize * _scale).ToPoint();
      _atlas = TextureAtlas.Create(texture, cellSize, cellSize, margin: margin, spacing: spacing);
    }

    public Rectangle GetTile(int id)
    {
      return _atlas.GetRegion(id).Bounds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      var cameraBounds = MBGame.Camera.GetBoundingRectangle();
      var startX = (int)(cameraBounds.Location.X / TileSize.X);
      var startY = (int)(cameraBounds.Location.Y / TileSize.Y);
      var endX = startX + (int)(cameraBounds.Width / TileSize.X);
      var endY = startY + (int)(cameraBounds.Height / TileSize.Y);

      for ( int x = startX - TileSize.X; x < endX + TileSize.X; x++ ) {
        for ( int y = startY - TileSize.Y; y < endY + TileSize.Y; y++ ) {

          var drawPos = MBMath.WrapGrid(x, y, _width, _height);
          var drawView = new Rectangle(
            x * TileSize.X,
            y * TileSize.Y,
            TileSize.X + _offset,
            TileSize.Y + _offset
          );

          if ( cameraBounds.Intersects(drawView) ) {
            var tile = _tiles[drawPos.X, drawPos.Y];
            var tileSource = GetTile(tile.ID);

            spriteBatch.Draw(
              Texture,
              destinationRectangle: drawView,
              sourceRectangle: tileSource,
              scale: _scale
            );
          }

        }
      }
    }

    public void HandleWrapping(Movement movement)
    {
      if ( movement == null ) {
        return;
      }

      if ( movement.Position.X < 0 ) {
        movement.Position = new Vector2(MapSize.X * TileSize.X, movement.Position.Y);
      }
      if ( movement.Position.X > MapSize.X * TileSize.X ) {
        movement.Position = new Vector2(0, movement.Position.Y);
      }
      if ( movement.Position.Y > MapSize.Y * TileSize.X ) {
        movement.Position = new Vector2(movement.Position.X, 0);
      }
      if ( movement.Position.Y < 0 ) {
        movement.Position = new Vector2(movement.Position.X, MapSize.Y * TileSize.Y);
      }
    }

    public Texture2D Texture
    {
      get { return _atlas.Texture; }
    }

    public Point TileSize
    {
      get { return _realCellSize; }
    }

    public Point MapSize
    {
      get { return new Point(_width, _height); }
    }
  }
}
