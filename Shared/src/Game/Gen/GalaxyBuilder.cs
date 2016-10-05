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
using System.Text.RegularExpressions;
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
    private List<Color> _availableColors;

    public GalaxyBuilder(ContentManager content, int size, int seed = 0)
    {
      _content = content;
      _size = size;
      _seed = seed;
      _star = _content.Load<Texture2D>("Images/starsystem");
      _starSystems = new List<StarSystem>();

      _availableColors = new List<Color> {
        Color.White,
        Color.AntiqueWhite,
        Color.LightYellow,
        Color.LavenderBlush
      };
    }

    public List<StarSystem> Generate(int maxDistance)
    {
      var rand = new Random();

      if ( _seed != 0 ) {
        rand = new Random(_seed);
      }

      for ( int sys = 0; sys < _size; sys++ ) {
        var color = _availableColors[
          rand.Next(_availableColors.Count - 1)
        ];
        var pos = new Vector2(rand.Next(-maxDistance, maxDistance), rand.Next(-maxDistance, maxDistance));
        while ( GetCollision(pos) ) {
          pos = new Vector2(rand.Next(-maxDistance, maxDistance), rand.Next(-maxDistance, maxDistance));
        }
        var rect = new Rectangle(pos.ToPoint(), _star.Bounds.Size);

        _starSystems.Add(new StarSystem {
          Color = color,
          Bounds = rect,
          Name = GenerateSystemName(rand)
        });
      }

      return _starSystems;
    }

    private string GenerateSystemName(Random rand)
    {
      string name = string.Empty;
      int max = rand.Next(2, 10);

      var vowels = new Regex("^[aeiou]{1}");
      var vowelList = new int[] { 97, 101, 105, 111, 117 }; // ASCII vowels

      name += (char)rand.Next(65, 90); // Capital letters

      var prev = (char)(name[0] + 32); // Get lower case version of first character

      for ( int i = 0; i < max; i++ ) {
        if ( !vowels.Match(prev.ToString()).Success ) {
          prev = (char)vowelList[rand.Next(0, vowelList.Length)];
        } else {
          prev = (char)rand.Next(97, 122); // All lower case alpha letters
        }
        name += prev;
      }

      return name;
    }

    private bool GetCollision(Vector2 position)
    {
      var collision = false;
      var boundingCircle = new CircleF(position, 100.0f);
      foreach ( var s in _starSystems ) {
        if ( boundingCircle.Contains(s.Bounds.Center) ) {
          collision = true;
          break;
        }
      }

      return collision;
    }
  }
}
