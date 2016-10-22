//
// 	EnvironmentTests.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 22/10/2016.
// 	Copyright  All rights reserved
//

using System;
using NUnit.Framework;

namespace MidnightBlue.Testing
{
  [TestFixture]
  public class EnvironmentTests
  {
    [TestCase]
    public void TestBiome()
    {
      foreach ( var m in Enum.GetValues(typeof(MoistureLevel)) ) {
        foreach ( var t in Enum.GetValues(typeof(TemperatureLevel)) ) {
          foreach ( var h in Enum.GetValues(typeof(HeightLevel)) ) {
            var b1 = EcosystemTool.GetBiome((MoistureLevel)m, (TemperatureLevel)t, (HeightLevel)h);
            var b2 = EcosystemTool.GetBiomeFromJSON((MoistureLevel)m, (TemperatureLevel)t, (HeightLevel)h);
            Console.WriteLine("{0}, {1}, {2}", (MoistureLevel)m, (TemperatureLevel)t, (HeightLevel)h);
            Assert.AreEqual(b1, b2);
          }
        }
      }
    }
  }
}
