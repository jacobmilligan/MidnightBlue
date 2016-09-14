//
// MBConsole.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 01/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MidnightBlueMono
{
  public abstract class Node
  {
    protected Point _position;
    protected List<Node> _neighbours;

    public Node(float x, float y)
    {
      _neighbours = new List<Node>();
      _position = new Point();
      _position.X = (int)x;
      _position.Y = (int)y;
    }

    public virtual void AddNeighbour(Node n)
    {
      if ( n.GetType() == this.GetType() ) {
        if ( !_neighbours.Contains(n) ) {
          _neighbours.Add(n);
          n.AddNeighbour(this);
        }
      }
    }

    public Point Position
    {
      get { return _position; }
    }
  }
}

