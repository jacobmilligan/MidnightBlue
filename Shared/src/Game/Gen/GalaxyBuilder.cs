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
using MidnightBlue.Engine;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  public class GalaxyBuilder
  {
    private int _size, _seed;
    private Rectangle _bounds;
    private Texture2D _star;
    private List<StarSystem> _starSystems;
    private List<Color> _availableColors;
    private ContentManager _content;
    private Random _rand;

    public GalaxyBuilder(ContentManager content, int size, int seed = 0)
    {
      _content = content;
      _size = size;
      _seed = seed;
      _star = _content.Load<Texture2D>("Images/starsystem");
      _starSystems = new List<StarSystem>();
      _bounds = new Rectangle();

      _availableColors = new List<Color> {
        Color.White,
        Color.PaleGoldenrod,
        Color.LightYellow,
        Color.Gold
      };
    }

    public List<StarSystem> Generate(int maxDistance)
    {
      _rand = new Random();

      if ( _seed != 0 ) {
        _rand = new Random(_seed);
      }

      var cameraPos = MBGame.Camera.Position.ToPoint();

      for ( int sys = 0; sys < _size; sys++ ) {

        var pos = new Vector2(
          _rand.Next(cameraPos.X - maxDistance, cameraPos.X + maxDistance),
          _rand.Next(cameraPos.Y - maxDistance, cameraPos.Y + maxDistance)
        );

        while ( GetCollision(pos) ) {
          pos = new Vector2(
            _rand.Next(cameraPos.X - maxDistance, cameraPos.X + maxDistance),
            _rand.Next(cameraPos.Y - maxDistance, cameraPos.Y + maxDistance)
          );
        }

        var rect = new Rectangle(pos.ToPoint(), _star.Bounds.Size);
        var nebulaSize = _rand.Next(1000, 10000);
        var radius = _rand.Next(100000 + nebulaSize, 1000000 + nebulaSize);

        var colorIdx = 0;
        if ( radius >= 200000 && radius < 550000 ) {
          colorIdx = 1;
        } else if ( radius >= 550000 && radius < 750000 ) {
          colorIdx = 2;
        } else if ( radius >= 750000 ) {
          colorIdx = 3;
        }
        var color = _availableColors[colorIdx];

        _starSystems.Add(new StarSystem {
          Color = color,
          Bounds = rect,
          Name = GenerateName(),
          Radius = radius,
          Planets = GeneratePlanets(nebulaSize, radius)
        });

        if ( rect.Right > _bounds.Right ) {
          _bounds.Inflate(rect.Right - _bounds.Right, 0);
        }
        if ( rect.Bottom > _bounds.Bottom ) {
          _bounds.Inflate(0, rect.Bottom - _bounds.Bottom);
        }
        if ( rect.Top < _bounds.Top ) {
          _bounds.Inflate(0, _bounds.Top - rect.Top);
        }
        if ( rect.Left < _bounds.Left ) {
          _bounds.Inflate(_bounds.Right - rect.Right, 0);
        }
      }

      return _starSystems;
    }

    private List<Planet> GeneratePlanets(int nebulaSize, int starRadius)
    {
      var planets = new List<Planet>();
      var acceleration = _rand.Next(starRadius / 10000, starRadius / 1000);
      var maxPlanets = _rand.Next(4);
      var growthFactor = (nebulaSize * acceleration) - starRadius;

      if ( growthFactor > 0 && _rand.Next(growthFactor) > starRadius ) {
        maxPlanets += _rand.Next(4);
      }

      for ( int p = 0; p < maxPlanets; p++ ) {
        planets.Add(new Planet {
          Name = GenerateName()
        });
      }

      return planets;
    }

    private string GenerateName()
    {
      string name = string.Empty;
      int max = _rand.Next(2, 10);

      var vowels = new Regex("^[aeiou]{1}");
      var vowelList = new int[] { 97, 101, 105, 111, 117 }; // ASCII vowels

      name += (char)_rand.Next(65, 90); // Capital letters

      var prev = (char)(name[0] + 32); // Get lower case version of first character

      for ( int i = 0; i < max; i++ ) {
        if ( !vowels.Match(prev.ToString()).Success ) {
          prev = (char)vowelList[_rand.Next(0, vowelList.Length)];
        } else {
          prev = (char)_rand.Next(97, 122); // All lower case alpha letters
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

    public Rectangle Bounds
    {
      get { return _bounds; }
    }

    public int Size
    {
      get { return _size; }
    }
  }
}
