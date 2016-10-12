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
    /// <summary>
    /// Jupiter radius - unit of measurement for planets.
    /// <a href="https://en.wikipedia.org/wiki/Jupiter_radius">See wikipedia link</a>
    /// </summary>
    private const int _jup = 71492;

    private int _size, _seed;
    private bool _done;
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
      _done = false;

      _availableColors = new List<Color> {
        Color.White,
        Color.PaleGoldenrod,
        Color.LightYellow,
        Color.Gold
      };
    }

    public List<StarSystem> Generate(int maxDistance)
    {
      if ( _seed == 0 ) {
        _seed = new Random().Next(100);
      }
      _rand = new Random(_seed);

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

      _done = true;
      return _starSystems;
    }

    private List<PlanetMetadata> GeneratePlanets(int nebulaSize, int starRadius)
    {
      var planets = new List<PlanetMetadata>();
      var availableMatter = starRadius - nebulaSize;
      var gravity = starRadius - _rand.Next(_jup);
      var acceleration = gravity / 1000;

      var impact = 0;
      var planetSize = 1;
      for ( int nebulaLeft = availableMatter; nebulaLeft > 0; nebulaLeft -= planetSize + (impact * impact) ) {
        impact = _rand.Next(acceleration);
        if ( impact > 100 && impact < 400 ) {

          // Convert distance (d/1000000) for less overhead while calculating
          // the planets parameters
          // Range of valid distances to the sun: mercury - (2 * Pluto)
          var starDistance = _rand.Next(50, 1000000);
          var newPlanet = CreatePlanet(starDistance, impact);
          if ( newPlanet != null ) {
            planetSize = newPlanet.Radius;
            planets.Add(newPlanet);
          }
        }

      }

      return planets;
    }

    private PlanetMetadata CreatePlanet(int starDistance, int impactSpeed)
    {
      var maxDensity = 6;
      var density = maxDensity - (impactSpeed / 100);
      density = _rand.Next(density);
      var radius = density + (_jup * (impactSpeed / 1000));

      var gas = _rand.Next(radius + impactSpeed);
      var water = _rand.Next(starDistance);
      var carbon = _rand.Next(radius + impactSpeed + density);

      var type = PlanetType.Terrestrial;

      if ( gas > water + carbon && _rand.Next(10) > 5 ) {
        type = PlanetType.Gas;
      } else if ( water > gas + carbon && _rand.Next(10) > 5 ) {
        type = PlanetType.Water;
      }

      var surfaceArea = (4 * MathHelper.Pi * (radius * radius));
      var temperature = (surfaceArea * (int)type) - (starDistance / 10000) + _rand.Next(30);

      var life = 30 - Math.Abs(temperature) + _rand.Next(density);
      if ( type == PlanetType.Terrestrial && life > 0 ) {
        life *= life;
      }

      return new PlanetMetadata {
        Name = GenerateName(),
        Density = density,
        Gas = gas,
        Water = water,
        Carbon = carbon,
        Type = type,
        Radius = radius * 100000,
        SurfaceTemperature = temperature,
        Habitable = life,
        // convert distance back to actual kilometers
        StarDistance = new Length((ulong)starDistance * 1000000)
      };
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

    public bool Done
    {
      get { return _done; }
    }

    public List<StarSystem> StarSystems
    {
      get { return _starSystems; }
    }
  }
}
