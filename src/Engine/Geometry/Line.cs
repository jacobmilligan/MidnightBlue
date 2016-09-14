using System;
using Microsoft.Xna.Framework;

namespace MidnightBlueMono
{
  public class Line
  {
    public Line(Vector2 start, Vector2 end)
    {
      Start = start;
      End = end;
    }

    public Line(Point start, Point end) : this(start.ToVector2(), end.ToVector2()) { }

    public float PointDistance(Vector2 point)
    {
      float result = 0;
      Vector2 d = End - Start;
      float length = d.Length();
      d.Normalize();
      result = Vector2.Dot((point - End), d);
      if ( result < 0 ) {
        result = (point - Start).Length();
      } else if ( result > length ) {
        result = (point - End).Length();
      } else {
        result = (point - (Start + d * result)).Length();
      }
      return result;
    }

    public float PointDistance(Point point)
    {
      return PointDistance(new Vector2(point.X, point.Y));
    }

    public Vector2 Start { get; set; }
    public Vector2 End { get; set; }
  }
}

