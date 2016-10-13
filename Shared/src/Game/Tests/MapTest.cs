//
// 	SimplexTest.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 11/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine;
using MidnightBlue.Engine.EntityComponent;
using MidnightBlue.Engine.Scenes;
using MidnightBlue.Engine.Tiles;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Testing
{
  public class MapTest : Scene
  {
    private Planet _planet;
    private TileMap _tiles;
    private SpriteFont _font;

    public MapTest(EntityMap map, ContentManager content) : base(map, content)
    {
      _font = content.Load<SpriteFont>("Fonts/Bender Large");
      _tiles = new TileMap(content.Load<Texture2D>("Images/terrain"), 32, offset: 1);
    }

    public override void Initialize()
    {
      BecomeShip(GameObjects["player"]);
      var physicsSystem = GameObjects.GetSystem<PhysicsSystem>() as PhysicsSystem;
      physicsSystem.Environment = new PhysicsEnvironment {
        Inertia = 0.6f,
        RotationInertia = 0.85f
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
      HandleWrapping();

      GameObjects.GetSystem<CollisionSystem>().Run();
      GameObjects.GetSystem<PhysicsSystem>().Run();
      GameObjects.GetSystem<MovementSystem>().Run();
      GameObjects.GetSystem<DepthSystem>().Run();

    }

    private void BecomeShip(Entity entity)
    {
      entity.DetachAll();

      var sprite = entity.Attach<SpriteTransform>(
        Content.Load<Texture2D>("Images/playership_blue"),
        new Vector2(MBGame.Camera.Position.X, MBGame.Camera.Position.Y),
        new Vector2(0.6f, 0.6f)
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

      entity.Attach<PlayerController>();
      entity.Attach<UtilityController>();
    }

    private void HandleWrapping()
    {
      var movement = GameObjects["player"].GetComponent<Movement>();
      if ( movement.Position.X < 0 ) {
        movement.Position = new Vector2(_tiles.MapSize.X * _tiles.TileSize.X, movement.Position.Y);
      }
      if ( movement.Position.X > _tiles.MapSize.X * 32 ) {
        movement.Position = new Vector2(0, movement.Position.Y);
      }
      if ( movement.Position.Y > _tiles.MapSize.Y * 32 ) {
        movement.Position = new Vector2(movement.Position.X, 0);
      }
      if ( movement.Position.Y < 0 ) {
        movement.Position = new Vector2(movement.Position.X, _tiles.MapSize.Y * _tiles.TileSize.Y);
      }
    }

    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      var seed = 10490;
      if ( _planet == null ) {
        var length = new Length((ulong)(Length.AstronomicalUnit * 5.9) * 1000);
        _planet = new Planet(
          new PlanetMetadata {
            Radius = 400000,
            SurfaceTemperature = 20,
            Type = PlanetType.Terrestrial,
            StarDistance = new Length(length.Kilometers)
          }, seed
        );
        _planet.Generate(new Random(seed));
        _planet.CreateMapTexture(Content);
        _tiles.Fill(_planet.Tiles);
      }

      _tiles.Draw(spriteBatch);
      uiSpriteBatch.Draw(_planet.GetMapLayer("map"), new Vector2(0, 0));

      //FIXME: Remove method after debugging
      var movement = GameObjects["player"].GetComponent<Movement>();
      var pos = MBMath.WrapGrid((int)movement.Position.X / 32, (int)movement.Position.Y / 32, _tiles.MapSize.X, _tiles.MapSize.Y);
      var tile = _planet.Tiles[pos.X, pos.Y];
      var strSize = _font.MeasureString(tile.Biome.ToString());

      GameObjects.GetSystem<RenderSystem>().Run();

      uiSpriteBatch.DrawString(
        _font,
        tile.Biome.ToString(),
        new Vector2(MBGame.Graphics.Viewport.Bounds.Right - strSize.X, 0),
        Color.White
      );
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

  }
}
