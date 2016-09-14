//
// ECSTest.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using System;
using NUnit.Framework;

namespace MidnightBlueMono
{
  [TestFixture()]
  public class ECSTest
  {
    ECSMap _map;

    [SetUp()]
    public void SetUp()
    {
      _map = new ECSMap();
      _map.AddSystem<TestSystem>();
      var e = new Entity("test entity");
      e.Attach<Position>(0, 0);
      _map.AddEntity(e);
      var e2 = new Entity("persistant entity");
      e2.Attach<Position>(0, 0);
      e2.Persistant = true;
      _map.AddEntity(e2);
    }

    [Test()]
    public void TestGetComponentID()
    {
      Assert.Greater(_map.GetComponentID(typeof(Position)), 0);
    }

    [Test()]
    public void TestComponentHasNoID()
    {
      Assert.AreEqual(0, _map.GetComponentID(typeof(Velocity)));
    }

    [Test()]
    public void TestEntityAndSystemIDAreSame()
    {
      Assert.AreEqual(_map["test entity"].ID, _map.GetSystem<TestSystem>().ID);
    }

    [Test()]
    public void TestSystemHasEntityReferences()
    {
      Assert.AreEqual(_map.GetSystem<TestSystem>().AssociatedEntities[0], _map["test entity"]);
    }

    [Test()]
    public void TestSystemRuns()
    {
      _map.GetSystem<TestSystem>().Run();
      _map.GetSystem<TestSystem>().Run();
      Assert.Greater(_map["test entity"].GetComponent<Position>().X, 1);
    }

    [Test()]
    public void TestEntityNotFoundByTag()
    {
      _map.CreateEntity().Attach<Position>(0, 0);
      Assert.IsNull(_map[""]);
    }

    [Test()]
    public void TestPersistantEntity()
    {
      var newMap = new ECSMap(_map);
      newMap.Clear();
      Assert.NotNull(newMap["persistant entity"]);
      Assert.IsNull(newMap["test entity"]);
    }

    [Test()]
    public void TestSystemOnPersistantEntityAfterClear()
    {
      var newMap = new ECSMap(_map);
      newMap.Clear();
      _map.GetSystem<TestSystem>().Run();
      _map.GetSystem<TestSystem>().Run();
      Assert.Greater(_map["persistant entity"].GetComponent<Position>().X, 1);
    }

    [Test()]
    public void TestCopyOfECSMap()
    {
      var newMap = new ECSMap(_map);
      Assert.AreSame(_map["test entity"], newMap["test entity"]);
      Assert.AreEqual(_map["test entity"].Tag, newMap["test entity"].Tag);
      Assert.NotNull(newMap.GetSystem<TestSystem>());
    }

  }
}

