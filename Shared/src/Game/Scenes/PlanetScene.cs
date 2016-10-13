//
// 	PlanetScene.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 12/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.Tiles;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  public class PlanetScene : Scene
  {
    private Planet _planet;
    private TileMap _tiles;
    private SoundTrigger _thrusterSound;

    public PlanetScene(EntityMap map, ContentManager content, Planet planet) : base(map, content)
    {
      _planet = planet;

      _tiles = new TileMap(content.Load<Texture2D>("Images/terrain"), 32, scale: 2);
      _tiles.Fill(planet.Tiles);

      _thrusterSound = new SoundTrigger(content.Load<SoundEffect>("Audio/engine"));
      _thrusterSound.IsLooped = true;

      map.Clear();

      var collision = map.GetSystem<CollisionSystem>() as CollisionSystem;
      if ( collision != null ) {
        var width = _planet.Tiles.GetLength(0);
        var height = _planet.Tiles.GetLength(1);
        collision.ResetGrid(0, width, 0, height, 108);
      }
    }

    public override void Initialize()
    {
      var player = GameObjects["player"];
      BecomeShip(player);

      var physicsSystem = GameObjects.GetSystem<PhysicsSystem>() as PhysicsSystem;
      physicsSystem.Environment = new PhysicsEnvironment {
        Inertia = 0.92f,
        RotationInertia = 0.95f
      };

      // End transition instantly
      TransitionState = TransitionState.None;
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<ShipInputSystem>().Run();
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
      var player = GameObjects["player"];
      UpdateSounds(player);

      //FIXME: Weird flash of sprite when wrapping
      _tiles.HandleWrapping(player.GetComponent<Movement>());

      GameObjects.GetSystem<CollisionSystem>().Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();
      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();
    }

    private void UpdateSounds(Entity player)
    {
      var physics = player.GetComponent<PhysicsComponent>();
      var ship = player.GetComponent<ShipController>();

      if ( ship != null && physics != null ) {
        if ( physics.Power > 0 || physics.Power < 0 ) {
          _thrusterSound.FadeUp();
        } else {
          _thrusterSound.FadeDown();
        }
      } else {
        _thrusterSound.FadeDown();
      }
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      _tiles.Draw(spriteBatch);
      GameObjects.GetSystem<RenderSystem>().Run();
    }

    public override void Exit()
    {
      // End transition instantly
      TransitionState = TransitionState.Null;
    }

    public override void Pause()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    public override void Resume()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    private void BecomeShip(Entity entity)
    {
      entity.DetachAll();

      var sprite = entity.Attach<SpriteTransform>(
        Content.Load<Texture2D>("Images/playership_blue"),
        new Vector2(MBGame.Camera.Position.X, MBGame.Camera.Position.Y),
        new Vector2(0.8f, 0.8f)
      ) as SpriteTransform;
      sprite.Z = 1;
      entity.Attach<CollisionComponent>(new RectangleF[] { sprite.Target.GetBoundingRectangle() });
      entity.Attach<PhysicsComponent>();
      var inventory = entity.Attach<Inventory>() as Inventory;
      inventory.Items.Add(typeof(Fuel), new Fuel(10000));
      entity.Attach<Movement>(1000.0f, 0.1f);

      entity.Attach<ShipController>();
      entity.Attach<UtilityController>();

      GameObjects.UpdateSystems(entity);
    }

    private void BecomePlayer(Entity entity)
    {
      entity.DetachAll();

      var movement = entity.GetComponent<Movement>();
      var physics = entity.GetComponent<PhysicsComponent>();
      movement.Position = new Vector2(0, 0);
      movement.Speed = 350;
      physics.Velocity = new Vector2(0, 0);

      entity.Detach<ShipController>();
      entity.Attach<PlayerController>();
      entity.Attach<UtilityController>();
    }

  }
}
