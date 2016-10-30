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
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace MidnightBlue.Engine.Testing
{
  [TestFixture]
  public class TestConsoleParse
  {
    private MBConsoleParser _parser;
    private MBConsole _console;

    private const int _testInt = 9;
    private const double _testDouble = 823.45;
    private const bool _testBool = true;
    private const string _testString = "test string";

    private string _testFuncResult;

    private void TestFunc(params string[] args)
    {
      _testFuncResult = string.Empty;
      foreach ( var a in args ) {
        _testFuncResult += a;
      }
    }

    [SetUp]
    public void SetUp()
    {
      _console = new MBConsole(
        Color.Transparent, Color.Transparent, null
      );
      _parser = new MBConsoleParser();

      _console.AddVar("testNum", _testInt);
      _console.AddVar("testBool", _testBool);
      _console.AddVar("testDouble", _testDouble);
      _console.AddVar("testString", _testString);

      _console.AddFunc("TestFunc", TestFunc);
    }

    [Test]
    public void TestPrintImmediate()
    {
      var root = _parser.Parse("print 9");
      root.Handle(_console);
      Assert.AreEqual("9", _console.LastOutput);
    }

    [Test]
    public void TestPrintIntVar()
    {
      var root = _parser.Parse("print testNum");
      root.Handle(_console);
      Assert.AreEqual(_testInt.ToString(), _console.LastOutput);
    }

    [Test]
    public void TestPrintDoubleVar()
    {
      var root = _parser.Parse("print testDouble");
      root.Handle(_console);
      Assert.AreEqual(_testDouble.ToString(), _console.LastOutput);
    }

    [Test]
    public void TestPrintStringVar()
    {
      var root = _parser.Parse("print testString");
      root.Handle(_console);
      Assert.AreEqual("'" + _testString + "'", _console.LastOutput);
    }

    [Test]
    public void TestPrintBoolVar()
    {
      var root = _parser.Parse("print testBool");
      root.Handle(_console);
      Assert.AreEqual(_testBool.ToString(), _console.LastOutput);
    }

    [Test]
    public void TestFuncNoArgs()
    {
      var root = _parser.Parse("run TestFunc");
      root.Handle(_console);
      Assert.AreEqual(_testFuncResult, string.Empty);
    }

    [Test]
    public void TestFuncOneArg()
    {
      var args = "test";
      var root = _parser.Parse("run TestFunc " + args);
      root.Handle(_console);
      Assert.AreEqual(_testFuncResult, args);
    }

    [Test]
    public void TestFuncTwoArg()
    {
      var args = "test thing";
      var root = _parser.Parse("run TestFunc " + args);
      root.Handle(_console);
      Assert.AreEqual(_testFuncResult, args.Replace(" ", ""));
    }

    [Test]
    public void TestFuncThreeArg()
    {
      var args = "test thing again";
      var root = _parser.Parse("run TestFunc " + args);
      root.Handle(_console);
      Assert.AreEqual(_testFuncResult, args.Replace(" ", ""));
    }

    [Test]
    public void TestSetString()
    {
      var args = "'changed string'";
      var root = _parser.Parse("set testString " + args);
      root.Handle(_console);
      Assert.AreEqual(args.Replace("'", ""), _console.Vars["testString"]);

      _console.Vars["testString"] = _testString;
    }

    [Test]
    public void TestSetInt()
    {
      var args = 0;
      var root = _parser.Parse("set testInt " + args);
      root.Handle(_console);
      Assert.AreEqual(args, _console.Vars["testInt"]);

      _console.Vars["testInt"] = _testInt;
    }

    [Test]
    public void TestSetDouble()
    {
      var args = 100.203;
      var root = _parser.Parse("set testDouble " + args);
      root.Handle(_console);
      Assert.AreEqual(args, _console.Vars["testDouble"]);

      _console.Vars["testDouble"] = _testDouble;
    }

    [Test]
    public void TestSetBool()
    {
      var args = false;
      var root = _parser.Parse("set testBool " + args);
      root.Handle(_console);
      Assert.AreEqual(args, _console.Vars["testBool"]);

      _console.Vars["testBool"] = _testBool;
    }
  }
}
