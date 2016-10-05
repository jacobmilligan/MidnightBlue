//
// Edge.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 01/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

//TODO: Maybe delete this class

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.Geometry;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.Containers
{
  public class Edge<T> where T : Node
  {
    private T _a;
    private T _b;
    private float _cost;
    private Line _line;
    private Vector2 _lineCenter;

    public Edge(T a, T b, float cost)
    {
      _a = a;
      _b = b;
      _cost = cost;
      _line = new Line(_a.Position, _b.Position);
      _lineCenter = new Vector2(
        (_line.End.X + _line.Start.X) / 2,
        (_line.End.Y + _line.Start.Y) / 2
      );
    }

    public void Draw(Color color, float mouseDistance, SpriteBatch spriteBatch, SpriteFont font)
    {
      spriteBatch.DrawLine(_line.Start, _line.End, color, 2);
      var mouseState = Mouse.GetState();
      if ( _line.PointDistance(mouseState.Position) < mouseDistance ) {
        spriteBatch.DrawString(font, _cost.ToString("0.0"), _lineCenter, color);
      }
    }

    public T A
    {
      get { return _a; }
    }

    public T B
    {
      get { return _b; }
    }

    public float Cost
    {
      get { return _cost; }
    }

    public Line Path
    {
      get { return _line; }
    }
  }
}

