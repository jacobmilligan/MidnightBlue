//
// 	TestConsoleParse.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 30/10/2016.
// 	Copyright  All rights reserved
//
using System;
using NUnit.Framework;

namespace MidnightBlue.Engine.Testing
{
  [TestFixture]
  public class TestConsoleParse
  {
    private MBConsoleParser _parser;
    private MBGame _game;

    [SetUp]
    public void SetUp()
    {
      _game = new MBGame();
      _game.Run();
      _parser = new MBConsoleParser();
    }

    [Test]
    public void TestQuit()
    {

    }

    [TearDown]
    public void TearDown()
    {
      _game.Dispose();
    }
  }
}
