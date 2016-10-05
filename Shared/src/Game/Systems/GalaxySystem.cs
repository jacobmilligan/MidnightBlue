//
// 	GalaxySystem.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 5/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.EntityComponent;

namespace MidnightBlue.Engine
{
  public class GalaxySystem : EntitySystem
  {
    private List<Entity> _collisionEntities;

    public GalaxySystem()
      : base(
        typeof(StarSystemComponent),
        typeof(Collision)
      )
    {
      _collisionEntities = new List<Entity>();
    }

    protected override void PostAssociate(Entity entity)
    {
      var collision = entity.GetComponent<Collision>();
      var star = entity.GetComponent<StarSystemComponent>();

      if ( collision != null && star == null && !_collisionEntities.Contains(entity) ) {
        _collisionEntities.Add(entity);
      }
    }

    protected override void Process(Entity entity)
    {

      var sys = entity.GetComponent<StarSystemComponent>();
      var sysCollision = entity.GetComponent<Collision>();

      if ( sys != null ) {

        sys.Draw = false;

        if ( sysCollision != null ) {

          foreach ( var c in _collisionEntities ) {
            var collision = c.GetComponent<Collision>();
            if ( collision != null ) {
              sys.Draw = CheckCollision(collision, sysCollision);
            }
          }
        }
      }
    }

    private bool CheckCollision(Collision a, Collision b)
    {
      bool collision = false;
      foreach ( var aBox in a.Boxes ) {
        foreach ( var bBox in b.Boxes ) {
          if ( aBox.Intersects(bBox) ) {
            collision = true;
          }
        }
      }

      return collision;
    }
  }
}
