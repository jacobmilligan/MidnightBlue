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
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.IO;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.Tiles;
using MonoGame.Extended.Shapes;
using MonoGame.Extended.Sprites;

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
      (GameObjects.GetSystem<CollisionSystem>() as CollisionSystem).SetTileMap(_tiles);

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
      player.GetComponent<SpriteTransform>().Target.Scale = new Vector2(0.8f, 0.8f);
      player.GetComponent<Movement>().Position = new Vector2(_planet.Size.X / 2, _planet.Size.Y / 2);

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
      var shipInput = GameObjects.GetSystem<ShipInputSystem>() as ShipInputSystem;
      shipInput.Run();

      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {
      var player = GameObjects["player"];
      UpdateSounds(player);

      var shipController = player.GetComponent<ShipController>();

      if ( shipController != null ) {
        UpdateShip(shipController);
      }
      UpdateBiome();

      GameObjects.GetSystem<CollisionSystem>().Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();

      _tiles.HandleWrapping(player.GetComponent<Movement>());

      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();
    }


    private void UpdateShip(ShipController shipController)
    {
      var player = GameObjects["player"];
      var movement = player.GetComponent<Movement>();

      if ( shipController.State == ShipState.Landing ) {
        var sprite = player.GetComponent<SpriteTransform>();
        sprite.Target.Scale = new Vector2(sprite.Target.Scale.X - 0.01f, sprite.Target.Scale.Y - 0.01f);
        if ( sprite.Target.Scale.X < 0.5f ) {
          BecomePlayer(player);
        }
      } else if ( shipController.State == ShipState.Launching ) {

        if ( player.HasComponent<PlayerController>() ) {
          BecomeShip(player);
          var newController = player.GetComponent<ShipController>();
          newController.State = ShipState.Launching;
        }

        var sprite = player.GetComponent<SpriteTransform>();

        if ( sprite.Target.Scale.X < 0.8f ) {
          sprite.Target.Scale = new Vector2(sprite.Target.Scale.X + 0.01f, sprite.Target.Scale.Y + 0.01f);
        }
      }
    }

    private void UpdateBiome()
    {
      var shipController = GameObjects["player"].GetComponent<ShipController>();
      var movement = GameObjects["player"].GetComponent<Movement>();

      var tilePos = MBMath.WrapGrid(
          (int)(movement.Position.X / _tiles.TileSize.X),
          (int)(movement.Position.Y / _tiles.TileSize.Y),
          _tiles.MapSize.X,
          _tiles.MapSize.Y
        );

      var biome = _planet.Tiles[tilePos.X, tilePos.Y].Biome;

      if ( shipController != null ) {
        if ( biome == Biome.Ocean || biome == Biome.ShallowOcean ) {
          shipController.IsLandable = false;
        } else {
          shipController.IsLandable = true;
        }
      }
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
      var lastPos = MBGame.Camera.GetBoundingRectangle().Center;
      var lastAngle = 0.0f;

      if ( entity.HasComponent<Movement>() ) {
        lastPos = entity.GetComponent<Movement>().Position;
        lastAngle = entity.GetComponent<Movement>().Angle;
      }

      entity.DetachAll();

      var sprite = entity.Attach<SpriteTransform>(
        Content.Load<Texture2D>("Images/playership_blue"),
        new Vector2(MBGame.Camera.Position.X, MBGame.Camera.Position.Y),
        new Vector2(0.5f, 0.5f)
      ) as SpriteTransform;
      sprite.Z = 1;
      sprite.Rotation = lastAngle;

      entity.Attach<PhysicsComponent>();
      var movement = entity.Attach<Movement>(1000.0f, 0.1f) as Movement;
      movement.Position = lastPos;

      entity.Attach<ShipController>();
      entity.Attach<UtilityController>();

      GameObjects.UpdateSystems(entity);
    }

    private void BecomePlayer(Entity entity)
    {
      entity.Detach<SpriteTransform>();

      var movement = entity.GetComponent<Movement>();
      var physics = entity.GetComponent<PhysicsComponent>();
      var sprite = entity.Attach<SpriteTransform>(
        Content.Load<Texture2D>("Images/bkspr01"),
        new Vector2(MBGame.Camera.Position.X, MBGame.Camera.Position.Y),
        new Vector2(0.5f, 0.5f)
      ) as SpriteTransform;
      entity.Attach<CollisionComponent>(new RectangleF[] { sprite.Target.GetBoundingRectangle() });
      movement.Speed = 350;
      physics.Velocity = new Vector2(0, 0);

      entity.Detach<ShipController>();
      var playerController = entity.Attach<PlayerController>() as PlayerController;
      playerController.InputMap.Assign<LaunchCommand>(Keys.Space, CommandType.Trigger);
      entity.Attach<UtilityController>();
    }

  }
}
