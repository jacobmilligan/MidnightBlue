﻿//
// Galaxy.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 05/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Shapes;

namespace MidnightBlueMono
{
  public class Galaxy
  {
    private Graph<GalaxyNode> _graph;
    private int _nodeRadius, _numSystems;
    private int _seed;

    public const float MouseDistance = 5.0f;

    public Galaxy(int numSystems, int systemRadius)
    {
      _graph = new Graph<GalaxyNode>();
      _nodeRadius = systemRadius;
      _numSystems = numSystems;
    }

    public void Generate(int seed = 0)
    {
      var rand = new Random();
      _seed = seed;

      if ( _seed != 0 ) {
        rand = new Random(_seed);
      }

      var cir = new CircleF();

      var windowSize = new Point {
        X = MBGame.Graphics.Adapter.CurrentDisplayMode.Width,
        Y = MBGame.Graphics.Adapter.CurrentDisplayMode.Height
      };

      for ( int i = 0; i < _numSystems; i++ ) {
        var pt = new Point();
        var rad = new CircleF();

        bool collision = true;
        while ( collision ) {
          collision = false;

          pt.X = rand.Next((int)-windowSize.X, (int)(windowSize.X * 2));
          pt.Y = rand.Next((int)-windowSize.Y, (int)(windowSize.Y * 2));
          rad.Center = pt.ToVector2();
          rad.Radius = _nodeRadius * 5;

          foreach ( var node in _graph.Nodes ) {
            cir.Center = node.Position.ToVector2();
            cir.Radius = _nodeRadius * 5;
            if ( rad.Contains(cir) ) {
              collision = true;
              break;
            }
          }
        }

        var system = new GalaxyNode(pt, _nodeRadius);
        system.GenerateName(rand);
        _graph.Nodes.Add(system);
      }

      cir.Radius = 150;
      foreach ( var node in _graph.Nodes ) {
        cir.Center = node.Position.ToVector2();

        foreach ( var neighbour in _graph.Nodes ) {
          if ( cir.Contains(neighbour.Position) && neighbour != node ) {
            var edge = new Edge<GalaxyNode>(
              neighbour,
              node,
              Vector2.Distance(node.Position.ToVector2(), neighbour.Position.ToVector2())
            );
            node.AddNeighbour(neighbour);
            if ( !_graph.Edges.Contains(edge) ) {
              _graph.Edges.Add(edge);
            }
          }
        }
      }
    }

    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
      foreach ( var node in _graph.Nodes ) {
        if ( MBGame.Graphics.Viewport.Bounds.Contains(node.Position) ) {
          var mouseCircle = new CircleF(node.Position.ToVector2(), node.Radius * 2);
          if ( mouseCircle.Contains(Mouse.GetState().Position) ) {
            foreach ( var edge in _graph.Edges ) {
              if ( edge.A == node || edge.B == node ) {
                edge.Draw(Color.SkyBlue, 5.0f, spriteBatch, font);
              }
            }
            node.Draw(Color.Lime, spriteBatch, font);
          } else {
            node.Draw(Color.LightYellow, spriteBatch, font);
          }
        }
      }
    }

    public int NodeSize
    {
      get { return _nodeRadius; }
    }
  }
}

