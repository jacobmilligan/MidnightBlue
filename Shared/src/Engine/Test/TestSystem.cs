//
// TestSystem.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//


using System;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine.Testing
{
  public class Position : Component
  {
    public Position(int x = 0, int y = 0)
    {
      X = x;
      Y = y;
    }
    public int X { get; set; }
    public int Y { get; set; }
  }

  public class Velocity : Component
  {
    public int X { get; set; }
    public int Y { get; set; }
  }

  public class Test : Component
  {
    public int X { get; set; }
    public int Y { get; set; }
  }

  public class Unregistered : Component
  {
    public int X { get; set; }
    public int Y { get; set; }
  }

  public class TestSystem : ECSystem
  {
    public TestSystem() : base(typeof(Position)) { }

    protected override void Process(Entity entity)
    {
      entity.GetComponent<Position>().X += 1;
      entity.GetComponent<Position>().Y += 1;
    }
  }

  public class TestSystem2 : ECSystem
  {
    public TestSystem2() : base(typeof(Test)) { }

    protected override void Process(Entity entity)
    {
      entity.GetComponent<Test>().X += 1;
      entity.GetComponent<Test>().Y += 1;
    }
  }
}

