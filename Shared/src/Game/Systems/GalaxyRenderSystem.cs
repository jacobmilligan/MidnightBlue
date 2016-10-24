//
// 	GalaxyRenderSystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue
{
  /// <summary>
  /// Renders all information into the main HUD's list box on the hovered star system. Also
  /// displays the name of all the star systems planets.
  /// </summary>
  public class GalaxyRenderSystem : EntitySystem
  {
    /// <summary>
    /// The sprite batch to draw to.
    /// </summary>
    private SpriteBatch _spriteBatch;

    /// <summary>
    /// Font to display the information with.
    /// </summary>
    private SpriteFont _font;

    /// <summary>
    /// The list of each planets information
    /// </summary>
    private List<string> _starInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.GalaxyRenderSystem"/> class.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw to.</param>
    /// <param name="content">Content to load fonts from.</param>
    public GalaxyRenderSystem(SpriteBatch spriteBatch, ContentManager content)
      : base(typeof(StarSystem))
    {
      _spriteBatch = spriteBatch;
      _font = content.Load<SpriteFont>("Fonts/Bender");
      _starInfo = new List<string>();
    }

    /// <summary>
    /// Clears the starsystems info list before processing all entities.
    /// </summary>
    protected override void PreProcess()
    {
      _starInfo.Clear();
    }

    /// <summary>
    /// Checks for collisions with a star system in the galaxy view and
    /// renders any information associated with that star re:planets.
    /// </summary>
    /// <param name="entity">Entity to check collisions with.</param>
    protected override void Process(Entity entity)
    {
      var star = entity.GetComponent<StarSystem>();
      var collision = entity.GetComponent<CollisionComponent>();

      if ( star != null && collision != null && collision.Event ) {

        var textPosition = GetCenter(
          MBGame.Camera.GetBoundingRectangle().Center,
          new Vector2(_font.MeasureString(star.Name).X / 2, _font.MeasureString(star.Name).Y / 2)
        );

        var starInfo = string.Format("{0}\nRadius: {1}\nPlanets:\n{2}", star.Name, star.Radius, star.PlanetList);

        _spriteBatch.DrawString(
          _font,
          starInfo,
          textPosition,
          Color.White
        );

        foreach ( var p in star.Planets ) {
          var distance = p.StarDistance.Kilometers + "KM";
          if ( p.StarDistance.AU > 0.1f ) {
            distance = p.StarDistance.AU.ToString("N1") + "AU";
          }
          _starInfo.Add(string.Format(
            "* {0} - Radius: {1}\n  Type: {2}\n   Surface Temperature: {3}\n   Density: {4}\n   Life Rating: {5}\n   Carbon: {6}\n   Water: {7}\n   Gas: {8}\n  Star Distance: {9}\n",
            p.Name, p.Radius, p.Type, p.SurfaceTemperature, p.Density, p.Habitable, p.Carbon, p.Water, p.Gas, distance
          ));
        }
      }
    }

    /// <summary>
    /// Gets the center point to render the stars planet list to.
    /// </summary>
    /// <returns>The center point.</returns>
    /// <param name="parentCenter">Parent bounding box's center.</param>
    /// <param name="childCenter">Child bouding box's center.</param>
    private Vector2 GetCenter(Vector2 parentCenter, Vector2 childCenter)
    {
      return new Vector2(parentCenter.X - childCenter.X, parentCenter.Y - 200);
    }

    /// <summary>
    /// Gets the list of all planets in the star system's information.
    /// </summary>
    /// <value>The info list.</value>
    public List<string> InfoList
    {
      get { return _starInfo; }
    }
  }
}
