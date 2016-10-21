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

    /// <summary>
    /// The overall number of star systems to generate
    /// </summary>
    private int _size,
    /// <summary>
    /// The seed to use for all generation in the galaxy and star systems.
    /// </summary>
    _seed;

    /// <summary>
    /// Indicates whether the generation of the galaxy has finished or not - used for threading and loading
    /// </summary>
    private bool _done;

    /// <summary>
    /// The bounding rectangle encompassing the entire galaxy
    /// </summary>
    private Rectangle _bounds;

    /// <summary>
    /// The texture to use when rendering each star
    /// </summary>
    private Texture2D _star;

    /// <summary>
    /// All star systems in the galaxy
    /// </summary>
    private List<StarSystem> _starSystems;

    /// <summary>
    /// The colors available to be used when generating a new star texture
    /// </summary>
    private List<Color> _availableColors;

    /// <summary>
    /// Content manager used to load resources
    /// </summary>
    private ContentManager _content;

    /// <summary>
    /// The random number generator to use for all procedural generation methods.
    /// </summary>
    private Random _rand;


    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.GalaxyBuilder"/> class. Does
    /// not actually generate the galaxy - that's done via Generate()
    /// </summary>
    /// <param name="content">Content manager to use for loading resources.</param>
    /// <param name="size">Number of star systems to generate.</param>
    /// <param name="seed">Seed to use for generation.</param>
    public GalaxyBuilder(ContentManager content, int size, int seed = 0)
    {
      _content = content;
      _size = size;
      _seed = seed;
      _star = _content.Load<Texture2D>("Images/starsystem");
      _starSystems = new List<StarSystem>();
      _bounds = new Rectangle();
      _done = false;

      // List used for random selection of numbers
      _availableColors = new List<Color> {
        Color.White,
        Color.PaleGoldenrod,
        Color.LightYellow,
        Color.Gold
      };
    }

    /// <summary>
    /// Generates the galaxy with a specified max distance between stars. Takes a while so
    /// should be called only once per gameplay session.
    /// </summary>
    /// <param name="maxDistance">Max distance between generated star systems.</param>
    public List<StarSystem> Generate(int maxDistance)
    {
      // Get random seed if none was specified
      if ( _seed == 0 ) {
        _seed = new Random().Next(100);
      }
      _rand = new Random(_seed);

      var cameraPos = MBGame.Camera.Position.ToPoint();

      // Generate the star systems
      for ( int sys = 0; sys < _size; sys++ ) {

        // Random pos up to max distance from the camera
        var pos = new Vector2(
          _rand.Next(cameraPos.X - maxDistance, cameraPos.X + maxDistance),
          _rand.Next(cameraPos.Y - maxDistance, cameraPos.Y + maxDistance)
        );

        // Check for collisions around the generated position
        while ( GetCollision(pos) ) {
          pos = new Vector2(
            _rand.Next(cameraPos.X - maxDistance, cameraPos.X + maxDistance),
            _rand.Next(cameraPos.Y - maxDistance, cameraPos.Y + maxDistance)
          );
        }

        var rect = new Rectangle(pos.ToPoint(), _star.Bounds.Size);
        var nebulaSize = _rand.Next(1000, 10000); // used for planet generation
        // determines the size of the star at the center of the system
        var radius = _rand.Next(100000 + nebulaSize, 1000000 + nebulaSize);

        // Setup color of the star
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
          Planets = GeneratePlanets(nebulaSize, radius) // Actually generate the systems planets
        });

        // Expand the size of the galaxy's bounding box with each generated planet
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

      _done = true; // finished - let main thread know
      return _starSystems;
    }

    /// <summary>
    /// Generates all of the planets in a single star system by simulating nebula matter
    /// orbiting, smashing and merging around the generated star. Finishes when there's no
    /// more loose matter left.
    /// </summary>
    /// <returns>The planets.</returns>
    /// <param name="nebulaSize">Nebula size.</param>
    /// <param name="starRadius">Star radius.</param>
    private List<PlanetMetadata> GeneratePlanets(int nebulaSize, int starRadius)
    {
      var planets = new List<PlanetMetadata>();
      // remove the space the star takes up in the nebula from the equation
      var availableMatter = starRadius - nebulaSize;
      // Needs to be improved - not a good indicator of gravitys effects on the nebula and matter
      var gravity = starRadius - _rand.Next(_jup);
      // Acceleration of matter around the star - once again needs to be modelled more accurately
      var acceleration = gravity / 1000;

      var impact = 0;
      var planetSize = 1;

      // Simulate orbits, collisions and the merging of matter to form planets
      // decrements the amount of available nebula matter by the size of the planet and the
      // size of the impact squared
      for ( int nebulaLeft = availableMatter; nebulaLeft > 0; nebulaLeft -= planetSize + (impact * impact) ) {
        impact = _rand.Next(acceleration);

        // Per 100,000 KM/H
        if ( impact > 100 && impact < 400 ) {

          // Convert distance (d/1000000) for less overhead while calculating
          // the planets parameters
          // Range of valid distances to the sun: mercury - (2 * Pluto)
          var starDistance = _rand.Next(500, 1000000);

          var newPlanet = CreatePlanet(starDistance, impact);

          // Add the new planet if it exists
          if ( newPlanet != null ) {
            planetSize = newPlanet.Radius;
            planets.Add(newPlanet);
          }
        }

      }

      return planets;
    }

    /// <summary>
    /// Generates a planets metadata to use as arguments in generating its map
    /// for exploration.
    /// </summary>
    /// <returns>The planet metadata.</returns>
    /// <param name="starDistance">Distance from the planet to its star.</param>
    /// <param name="impactSpeed">The speed per 100,000 KM/H of its impact with other matter.</param>
    private PlanetMetadata CreatePlanet(int starDistance, int impactSpeed)
    {
      var maxDensity = 6; // just denser than earth

      // make the density relative to the speed of impact.
      // Means that super-fast impact or super low impact speeds
      // will generate less dense planets. Needs to be just right for earth-like
      // density
      var density = maxDensity - (impactSpeed / 100);
      density = _rand.Next(density);

      // Get radius of the generated planet relative to its density
      // and the stability of its matter after impact. Multiply by jupiter
      // radians to give it a galactic size.
      var radius = density + (_jup * (impactSpeed / 100));

      // Amount of gas should be relative to its stability and size
      // after impact
      var gas = _rand.Next(radius + impactSpeed);
      // Amount of water relative to distance from the sun - too close and
      // no water will be had
      var water = _rand.Next(starDistance);
      // Amount of carbon relative to the planets ability to be stable
      var carbon = _rand.Next(radius + impactSpeed + density);

      var type = PlanetType.Terrestrial;
      // Get planet type relative to the amount of each generated element
      if ( gas > water + carbon && _rand.Next(10) > 5 ) {
        type = PlanetType.Gas;
      } else if ( water > gas + carbon && _rand.Next(10) > 5 ) {
        type = PlanetType.Water;
      }

      // Normalize the radius to calculate size of the planet for its tile map
      var nr = radius / 1000;
      var surfaceArea = (4 * MathHelper.Pi * (nr * nr));

      // Normalize distance to star to allow accurate calculations
      var normalizedStarDistance = starDistance / 10000;
      var temperatureOffset = normalizedStarDistance;
      if ( normalizedStarDistance < 0 ) {
        // Too close - make it hot!
        temperatureOffset = -temperatureOffset;
      } else if ( normalizedStarDistance > 0 && normalizedStarDistance < 100 ) {
        // Earth-like distance - no offset to temperature
        temperatureOffset = 0;
      }
      // Get temperature relative to desireable temperature, a random amount, and
      // its offset relative to the planets distance to its star
      var temperature = (temperatureOffset + _rand.Next(30));

      // Life score should be determined by the temperature
      // and the density of the planet
      var life = 30 - Math.Abs(temperature) + _rand.Next(density);
      if ( type == PlanetType.Terrestrial && life > 0 ) {
        // Terrestrial planets should be able to support life better
        life *= life;
      }

      return new PlanetMetadata {
        Name = GenerateName(),
        Density = density,
        Gas = gas,
        Water = water,
        Carbon = carbon,
        Type = type,
        Radius = radius,
        SurfaceTemperature = temperature,
        Habitable = life,
        // convert distance back to actual kilometers
        StarDistance = new Length((ulong)starDistance * 1000000)
      };
    }

    /// <summary>
    /// Generates the planets name
    /// </summary>
    /// <returns>The planets name.</returns>
    private string GenerateName()
    {
      string name = string.Empty;
      int maxNameLen = _rand.Next(2, 10);

      var vowels = new Regex("^[aeiou]{1}");
      var vowelList = new int[] { 97, 101, 105, 111, 117 }; // ASCII vowels

      name += (char)_rand.Next(65, 90); // Capital letters

      var prev = (char)(name[0] + 32); // Get lower case version of first character

      // Generate the name
      for ( int i = 0; i < maxNameLen; i++ ) {
        // Make sure there aren't constanants next to one another in weird ways
        if ( !vowels.Match(prev.ToString()).Success ) {
          prev = (char)vowelList[_rand.Next(0, vowelList.Length)];
        } else {
          prev = (char)_rand.Next(97, 122); // All lower case alpha letters - vowels possible too
        }
        name += prev;
      }

      return name;
    }

    /// <summary>
    /// Checks whether a star systems generated position collides with another
    /// </summary>
    /// <returns><c>true</c>, if collision, <c>false</c> otherwise.</returns>
    /// <param name="position">Position of the generated planet.</param>
    private bool GetCollision(Vector2 position)
    {
      var collision = false;
      var boundingCircle = new CircleF(position, 100.0f);
      // Brute force the collision check. Is okay as this is only done once per game
      foreach ( var s in _starSystems ) {
        if ( boundingCircle.Contains(s.Bounds.Center) ) {
          collision = true;
          break;
        }
      }

      return collision;
    }

    /// <summary>
    /// Gets the bounding rectangle of the galaxy.
    /// </summary>
    /// <value>The bounds.</value>
    public Rectangle Bounds
    {
      get { return _bounds; }
    }

    /// <summary>
    /// Gets the number of star systems the galaxy has.
    /// </summary>
    /// <value>The number of star systems.</value>
    public int Size
    {
      get { return _size; }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:MidnightBlue.GalaxyBuilder"/> is done generating.
    /// </summary>
    /// <value><c>true</c> if done; otherwise, <c>false</c>.</value>
    public bool Done
    {
      get { return _done; }
    }

    /// <summary>
    /// Gets the star system list.
    /// </summary>
    /// <value>The star systems.</value>
    public List<StarSystem> StarSystems
    {
      get { return _starSystems; }
    }
  }
}
