//
// 	GalaxyBuilder.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  public class GalaxyBuilder
  {
    private ContentManager _content;
    private int _size, _seed;
    private Texture2D _star;
    private List<StarSystem> _starSystems;

    public GalaxyBuilder(ContentManager content, int size, int seed = 0)
    {
      _content = content;
      _size = size;
      _seed = seed;
      _star = content.Load<Texture2D>("Images/starsystem");
      _starSystems = new List<StarSystem>();
    }

    public List<StarSystem> Generate(int maxDistance)
    {
      var rand = new Random();

      if ( _seed != 0 ) {
        rand = new Random(_seed);
      }

      var boundingCircle = new CircleF { Radius = maxDistance };

      for ( int sys = 0; sys < _size; sys++ ) {
        var color = new Color(
            rand.Next(255),
            rand.Next(255),
            rand.Next(255)
        );
        var pos = new Vector2(rand.Next(maxDistance), rand.Next(maxDistance));
        while ( GetCollision(pos) ) {
          pos = new Vector2(rand.Next(maxDistance), rand.Next(maxDistance));
        }
        var rect = new Rectangle(pos.ToPoint(), _star.Bounds.Size);

        _starSystems.Add(new StarSystem {
          Color = color,
          Bounds = rect
        });

      }

      return _starSystems;
    }

    private bool GetCollision(Vector2 position)
    {
      var collision = false;

      foreach ( var s in _starSystems ) {
        if ( s.Bounds.Contains(position) ) {
          collision = true;
          break;
        }
      }

      return collision;
    }
  }
}
