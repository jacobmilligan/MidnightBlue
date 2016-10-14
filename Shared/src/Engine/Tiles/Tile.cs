//
// 	Tile.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 13/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;

namespace MidnightBlue
{
  public enum TileFlag { Passable, Impassable }

  public class Tile
  {
    public Tile(int textureID, Color color)
    {
      ID = textureID;
      TintColor = color;
      Flag = TileFlag.Passable;
    }

    public Tile() : this(1, Color.Transparent) { }

    public int ID { get; protected set; }

    public Color TintColor { get; protected set; }

    public TileFlag Flag { get; set; }
  }
}
