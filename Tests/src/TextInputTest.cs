//
// 	TextInputTest.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 30/10/2016.
// 	Copyright  All rights reserved
//
using System;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.IO;
using NUnit.Framework;

namespace MidnightBlue.Engine.Testing
{
  [TestFixture]
  public class TextInputTest
  {
    private TextInputHandler _handler;

    [SetUp]
    public void SetUp()
    {
      _handler = new TextInputHandler();
    }

    [Test]
    public void TestShiftNumbers()
    {
      var expectedVals = new char[] {
        ')', '!', '@', '#', '$', '%', '^', '&', '*', '('
      };

      // Check all ASCII number values against their shift type
      var i = 0;
      for ( int k = 48; k < 58; k++ ) {
        var result = _handler.Translate(
          new Keys[] { Keys.LeftShift }, (Keys)k
        );
        Assert.AreEqual(expectedVals[i], result);
        i++;
      }
    }

    [Test]
    public void TestShiftSpecial()
    {
      var charTest = new Tuple<Keys, char>[] {
        Tuple.Create(Keys.OemMinus, '_'),
        Tuple.Create(Keys.OemPlus, '+'),
        Tuple.Create(Keys.OemOpenBrackets, '{'),
        Tuple.Create(Keys.OemCloseBrackets, '}'),
        Tuple.Create(Keys.OemBackslash, '|'),
        Tuple.Create(Keys.OemSemicolon, ':'),
        Tuple.Create(Keys.OemQuotes, '\"'),
        Tuple.Create(Keys.OemComma, '<'),
        Tuple.Create(Keys.OemPeriod, '>'),
        Tuple.Create(Keys.OemQuestion, '?'),
        Tuple.Create(Keys.OemTilde, '~')
      };

      foreach ( var charPair in charTest ) {
        var testKey = (Keys)charPair.Item1;
        var result = _handler.Translate(
          new Keys[] { Keys.LeftShift }, testKey
        );
        Assert.AreEqual(
          charPair.Item2.ToString(), result.ToString()
        );
      }
    }

    [Test]
    public void TestNonShiftSpecial()
    {
      var charTest = new Tuple<Keys, char>[] {
        Tuple.Create(Keys.OemMinus, '-'),
        Tuple.Create(Keys.OemPlus, '='),
        Tuple.Create(Keys.OemOpenBrackets, '['),
        Tuple.Create(Keys.OemCloseBrackets, ']'),
        Tuple.Create(Keys.OemBackslash, '\\'),
        Tuple.Create(Keys.OemSemicolon, ';'),
        Tuple.Create(Keys.OemQuotes, '\''),
        Tuple.Create(Keys.OemComma, ','),
        Tuple.Create(Keys.OemPeriod, '.'),
        Tuple.Create(Keys.OemQuestion, '/'),
        Tuple.Create(Keys.OemTilde, '`')
      };

      foreach ( var charPair in charTest ) {
        var testKey = (Keys)charPair.Item1;
        var result = _handler.Translate(
          new Keys[] { }, testKey
        );
        Assert.AreEqual(
          charPair.Item2.ToString(), result.ToString()
        );
      }
    }

    [Test]
    public void TestShiftAlpha()
    {
      // ASCII 66-90 = Capital alpha
      for ( int k = 65; k < 91; k++ ) {
        var result = _handler.Translate(
          new Keys[] { }, (Keys)k
        );
        Assert.AreEqual(
          ((char)(k + 32)).ToString(), result.ToString()
        );
      }
    }

    [Test]
    public void TestBackspace()
    {
      var result = _handler.Translate(
        new Keys[] { }, Keys.Back
      );
      Assert.AreEqual('\b', result);
    }

    [Test]
    public void TestSpace()
    {
      var result = _handler.Translate(
        new Keys[] { }, Keys.Space
      );
      Assert.AreEqual(' ', result);
    }

    [Test]
    public void TestTab()
    {
      var result = _handler.Translate(
        new Keys[] { }, Keys.Tab
      );
      Assert.AreEqual('\t', result);
    }

    [Test]
    public void TestEnter()
    {
      var result = _handler.Translate(
        new Keys[] { }, Keys.Enter
      );
      Assert.AreEqual('\r', result);
    }
  }
}
