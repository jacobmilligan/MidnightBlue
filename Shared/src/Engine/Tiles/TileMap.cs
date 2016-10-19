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
  /// <summary>
  /// A grid of tiles with collision. Wraps coordinates when they fall out of bounds.
  /// Allows accessing tiles by index.
  /// </summary>
  public class TileMap
  {
    /// <summary>
    /// The width of the tile map
    /// </summary>
    private int _width,
    /// <summary>
    /// The height of the tile map
    /// </summary>
    _height,
    /// <summary>
    /// The width and height of each tile in the grid before applying
    /// a scale vector.
    /// </summary>
    _cellSize,
    /// <summary>
    /// A number to offset the drawing of each tile by
    /// </summary>
    _offset;

    /// <summary>
    /// The size of each cell in the grid after applying the scale vector.
    /// </summary>
    private Point _worldCellSize;

    /// <summary>
    /// The scale vector to apply to each cell.
    /// </summary>
    private Vector2 _scale;

    /// <summary>
    /// The texture atlas to use for applying textures to tiles. Contains all valid regions
    /// in a single texture.
    /// </summary>
    private TextureAtlas _atlas;

    /// <summary>
    /// Stores all the tiles in the map.
    /// </summary>
    private Tile[,] _tiles;

    /// <summary>
    /// Uses a 2D Array of previously defined tile information to fill a tile map with collision
    /// data and other information. Must be called in order for the TileMap to function.
    /// </summary>
    /// <param name="tiles">Tiles.</param>
    public void Fill(Tile[,] tiles)
    {
      _width = tiles.GetLength(0);
      _height = tiles.GetLength(1);
      _tiles = tiles;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.Engine.Tiles.TileMap"/> class.
    /// Seperates the texture into a series of regions.
    /// </summary>
    /// <param name="texture">Texture to use in the texture atlas.</param>
    /// <param name="cellSize">The size of each cell in the texture atlas.</param>
    /// <param name="margin">Margin to apply to each rendered tile.</param>
    /// <param name="spacing">Spacing to apply to each rendered tile.</param>
    /// <param name="offset">Offset to apply to the x and y coordinates of each tile when rendering.</param>
    /// <param name="scale">Scale vector to apply to each cell when rendering.</param>
    public TileMap(
      Texture2D texture, int cellSize, int margin = 0, int spacing = 0, int offset = 0, float scale = 1.0f
    )
    {
      _width = texture.Width;
      _height = texture.Height;
      _cellSize = cellSize;
      _offset = offset;
      _scale = new Vector2(scale, scale);
      _worldCellSize = (_cellSize * _scale).ToPoint();
      _atlas = TextureAtlas.Create(texture, cellSize, cellSize, margin: margin, spacing: spacing);
    }

    /// <summary>
    /// Retrieves a tile
    /// </summary>
    /// <returns>The tile.</returns>
    /// <param name="id">Identifier.</param>
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

    public Tile this[int x, int y]
    {
      get
      {
        var pos = MBMath.WrapGrid(x, y, _width, _height);
        return _tiles[pos.X, pos.Y];
      }
    }

    public Texture2D Texture
    {
      get { return _atlas.Texture; }
    }

    public Point TileSize
    {
      get { return _worldCellSize; }
    }

    public Point MapSize
    {
      get { return new Point(_width, _height); }
    }
  }
}
