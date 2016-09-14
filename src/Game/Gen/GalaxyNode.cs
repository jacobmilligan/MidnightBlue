﻿//
// GalaxyNode.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;

namespace MidnightBlueMono
{
  public class GalaxyNode : Node
  {
    private string _name;
    private int _radius;

    public GalaxyNode(float x, float y, int radius) : base(x, y)
    {
      _radius = radius;
    }

    public GalaxyNode(Point pt, int radius) : this(pt.X, pt.Y, radius) { }

    public void GenerateName(Random rand)
    {
      int max = rand.Next(2, 10);

      var vowels = new Regex("^[aeiou]{1}");
      int[] vowelList = new int[] { 97, 101, 105, 111, 117 }; // ASCII vowels

      _name += (char)rand.Next(65, 90); // Capital letters

      char prev = (char)((int)_name[0] + 32); // Get lower case version of first character

      for ( int i = 0; i < max; i++ ) {
        if ( !vowels.Match(prev.ToString()).Success ) {
          prev = (char)vowelList[rand.Next(0, vowelList.Length)];
        } else {
          prev = (char)rand.Next(97, 122); // All lower case alpha letters
        }
        _name += prev;
      }
    }

    public void Draw(Color color, SpriteBatch spriteBatch, SpriteFont font)
    {
      spriteBatch.DrawCircle(_position.ToVector2(), _radius, 360, color);
      var mouseCircle = new CircleF(_position.ToVector2(), _radius * 2);
      if ( mouseCircle.Contains(Mouse.GetState().Position) ) {
        float civ = (float)_neighbours.Count / 10;
        var txt = _name + "\nChance of civilization: " + (civ * 100) + "%";

        var rect = new Rectangle(
          MBGame.Graphics.Adapter.CurrentDisplayMode.Width / 2,
          10,
          (int)font.MeasureString(txt).X,
          (int)font.MeasureString(txt).Y
        );
        spriteBatch.FillRectangle(rect, Color.White);
        spriteBatch.DrawString(font, txt, rect.Location.ToVector2(), Color.Black);
      }
    }

    public String Name
    {
      get { return _name; }
    }

    public float Radius
    {
      get { return _radius; }
    }
  }
}

