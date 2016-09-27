//
// ECSTest.cs
// Midnight Blue
//
// ---------------------------------------------------
//
// Create by Jacob Milligan on 12/09/2016.
// Copyright (c) Jacob Milligan 2016. All rights reserved.
//

using MidnightBlue.Engine.EntityComponent;
using NUnit.Framework;

namespace MidnightBlue.Engine.Testing
{
  [TestFixture]
  public class ECSTest
  {
    EntityMap _map;

    [SetUp]
    public void SetUp()
    {
      _map = new EntityMap();
      _map.AddSystem<TestSystem>();
      _map.AddSystem<TestSystem2>();
      var e = new Entity(_map, "test entity");
      e.Attach<Position>(0, 0);
      e.Attach<Velocity>();
      e.Attach<Test>();
      _map.AddEntity(e);
      var e2 = _map.CreateEntity("persistant entity");
      e2.Attach<Position>(0, 0);
      e2.Persistant = true;
      _map.AddEntity(e2);
    }

    [Test]
    public void TestEntitiesArentDuplicated()
    {
      Assert.AreEqual(2, _map.GetSystem<TestSystem>().AssociatedEntities.Count);
      Assert.AreEqual(1, _map.GetSystem<TestSystem2>().AssociatedEntities.Count);
    }

    [Test]
    public void TestGetComponentID()
    {
      Assert.Greater(_map.GetComponentID<Position>(), 0);
    }

    [Test]
    public void TestComponentHasNoID()
    {
      Assert.AreEqual(0, _map.GetComponentID<Unregistered>());
    }

    [Test]
    public void TestEntityAndSystemIDAreSame()
    {
      Assert.AreEqual(_map["persistant entity"].Mask, _map.GetSystem<TestSystem>().ID);
    }

    [Test]
    public void TestSystemHasEntityReferences()
    {
      Assert.AreEqual(_map.GetSystem<TestSystem>().AssociatedEntities[0], _map["test entity"]);
    }

    [Test]
    public void TestSystemHasCorrectID()
    {
      Assert.AreEqual(_map.GetSystem<TestSystem>().ValidComponents.Count, 1);
    }

    [Test]
    public void TestEntityHasCorrectID()
    {
      Assert.AreEqual(_map.GetSystem<TestSystem>().ID, _map.GetSystem<TestSystem>().ID);
    }

    [Test]
    public void TestSystemRuns()
    {
      _map.GetSystem<TestSystem>().Run();
      _map.GetSystem<TestSystem>().Run();
      Assert.Greater(_map["test entity"].GetComponent<Position>().X, 1);
    }

    [Test]
    public void TestEntityNotFoundByTag()
    {
      _map.CreateEntity().Attach<Position>(0, 0);
      Assert.IsNull(_map[""]);
    }

    [Test]
    public void TestPersistantEntity()
    {
      var newMap = new EntityMap(_map);
      newMap.Clear();
      Assert.NotNull(newMap["persistant entity"]);
      Assert.IsNull(newMap["test entity"]);
    }

    [Test]
    public void TestSystemOnPersistantEntityAfterClear()
    {
      var newMap = new EntityMap(_map);
      newMap.Clear();
      _map.GetSystem<TestSystem>().Run();
      _map.GetSystem<TestSystem>().Run();
      Assert.Greater(_map["persistant entity"].GetComponent<Position>().X, 1);
    }

    [Test]
    public void TestCopyOfECSMap()
    {
      var newMap = new EntityMap(_map);
      Assert.AreSame(_map["test entity"], newMap["test entity"]);
      Assert.AreEqual(_map["test entity"].Tag, newMap["test entity"].Tag);
      Assert.NotNull(newMap.GetSystem<TestSystem>());
    }

  }
}

