//
// 	GridTests.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 10/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using System;
using Microsoft.Xna.Framework;
using NUnit.Framework;
namespace MidnightBlue.Engine.Testing
{
  [TestFixture]
  public class GridTests
  {
    const int size = 10;

    [Test]
    public void TestXWrapping()
    {
      var x = 14;
      var y = 9;
      var expected = new Point(4, 9);

      Assert.AreEqual(expected, MBMath.WrapGrid(x, y, size, size));
    }

    [Test]
    public void TestYWrapping()
    {
      var x = 9;
      var y = 14;
      var expected = new Point(9, 4);

      Assert.AreEqual(expected, MBMath.WrapGrid(x, y, size, size));
    }

    [Test]
    public void TestLargeWraps()
    {
      var x = 56;
      var y = 9;
      var expected = new Point(6, 9);

      Assert.AreEqual(expected, MBMath.WrapGrid(x, y, size, size));
    }

    [Test]
    public void TestBothXYWrapping()
    {
      var x = 14;
      var y = 14;
      var expected = new Point(4, 4);

      Assert.AreEqual(expected, MBMath.WrapGrid(x, y, size, size));
    }
  }
}
