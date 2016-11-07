//
// 	PlanetScene.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 12/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MB2D;
using MB2D.EntityComponent;
using MB2D.Scenes;
using MB2D.Tiles;

namespace MidnightBlue
{
  /// <summary>
  /// Scene active when the player is exploring a given planet.
  /// </summary>
  public class PlanetScene : Scene
  {
    /// <summary>
    /// The scale of the players sprite when out of their ship
    /// </summary>
    private const float _playerScale = 0.4f;

    /// <summary>
    /// The scale of the players sprite when in their ship
    /// </summary>
    private const float _shipScale = 0.8f;

    /// <summary>
    /// The planet object to use for this scene
    /// </summary>
    private Planet _planet;

    /// <summary>
    /// The planets tile map
    /// </summary>
    private TileMap _tiles;

    /// <summary>
    /// The sound of the ships thrusters
    /// </summary>
    private SoundTrigger _thrusterSound;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.PlanetScene"/> class.
    /// </summary>
    /// <param name="map">Game object map.</param>
    /// <param name="content">Content manager for loading resources.</param>
    /// <param name="planet">Planet to use in this scene.</param>
    public PlanetScene(EntityMap map, ContentManager content, Planet planet) : base(map, content)
    {
      _planet = planet;

      // Setup the tilemap
      _tiles = new TileMap(content.Load<Texture2D>("Images/terrain"), 32, scale: 1.5f);
      _tiles.Fill(planet.Tiles);
      (GameObjects.GetSystem<CollisionSystem>() as CollisionSystem).SetTileMap(_tiles);

      // Setup sounds
      _thrusterSound = new SoundTrigger(content.Load<SoundEffect>("Audio/engine"));
      _thrusterSound.IsLooped = true;

      // Setup collisions
      map.Clear();

      var collision = map.GetSystem<CollisionSystem>() as CollisionSystem;
      if ( collision != null ) {
        var width = _planet.Tiles.GetLength(0);
        var height = _planet.Tiles.GetLength(1);
        collision.ResetGrid(0, width, 0, height, 108);
      }
    }

    /// <summary>
    /// Sets up the player and physics environment for this planet
    /// </summary>
    public override void Initialize()
    {
      // Setup player
      var player = GameObjects["player"];
      GameObjects.UseBlueprint("planet ship", player);
      player.GetComponent<SpriteTransform>().Target.Scale = new Vector2(_shipScale, _shipScale);
      player.GetComponent<Movement>().Position = new Vector2(_planet.Size.X / 2, _planet.Size.Y / 2);

      // Setup physics environment
      var physicsSystem = GameObjects.GetSystem<PhysicsSystem>() as PhysicsSystem;
      physicsSystem.Environment = new PhysicsEnvironment {
        Inertia = 0.92f,
        RotationInertia = 0.95f
      };

      // End transition instantly
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Handles the input for the scene.
    /// </summary>
    public override void HandleInput()
    {
      var shipInput = GameObjects.GetSystem<ShipInputSystem>() as ShipInputSystem;
      shipInput.Run();

      GameObjects.GetSystem<InputSystem>().Run();
    }

    /// <summary>
    /// Updates the players position and state alongside the current biome the player is
    /// located at.
    /// </summary>
    public override void Update()
    {
      if ( TransitionState == TransitionState.None ) {
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
    }


    /// <summary>
    /// Updates the scale of the ship according to the curent landing/launching state.
    /// Handles turning the player into the player/ship.
    /// </summary>
    /// <param name="shipController">Ship controller of the player.</param>
    private void UpdateShip(ShipController shipController)
    {
      var player = GameObjects["player"];

      // Animate landing scale transition - move closer to ground
      if ( shipController.State == ShipState.Landing ) {

        var sprite = player.GetComponent<SpriteTransform>();
        sprite.Target.Scale = new Vector2(
          sprite.Target.Scale.X - 0.01f, sprite.Target.Scale.Y - 0.01f
        );

        // Check to see if reached min scale
        if ( sprite.Target.Scale.X < _playerScale ) {
          GameObjects.UseBlueprint("player", player);
        }

      } else if ( shipController.State == ShipState.Launching ) {
        // Animate launching transition - move further from ground
        if ( player.HasComponent<PlayerController>() ) {
          GameObjects.UseBlueprint("planet ship", player);

          var newController = player.GetComponent<ShipController>();

          newController.State = ShipState.Launching;
          player.GetComponent<SpriteTransform>().Target.Scale = new Vector2(_playerScale, _playerScale);
        }

        var sprite = player.GetComponent<SpriteTransform>();

        // Only change scale if not reached max scale yet
        if ( sprite.Target.Scale.X < _shipScale ) {
          sprite.Target.Scale = new Vector2(sprite.Target.Scale.X + 0.01f, sprite.Target.Scale.Y + 0.01f);
        }
      } else if ( shipController.State == ShipState.LeavingScreen ) {
        SceneController.Pop();
      }
    }

    /// <summary>
    /// Updates the biome information for the player according to the tile they're currently
    /// on.
    /// </summary>
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

      // Set landable state for ship
      if ( shipController != null ) {
        // Stop landing over ocean
        if ( biome == Biome.Ocean || biome == Biome.ShallowOcean ) {
          shipController.IsLandable = false;
        } else {
          shipController.IsLandable = true;
        }
      }
    }

    /// <summary>
    /// Updates the ship sounds
    /// </summary>
    /// <param name="player">Player entity.</param>
    private void UpdateSounds(Entity player)
    {
      // Only activate thruster if the player is currently in their ship
      if ( player.HasComponent<ShipController>() && player.HasComponent<PhysicsComponent>() ) {
        var physics = player.GetComponent<PhysicsComponent>();

        if ( physics.Power > 0 || physics.Power < 0 ) {
          _thrusterSound.FadeUp();
        } else {
          _thrusterSound.FadeDown();
        }

      } else {
        _thrusterSound.FadeDown();
      }
    }

    /// <summary>
    /// Draw the tilemap to the specified spriteBatch and uiSpriteBatch.
    /// </summary>
    /// <param name="spriteBatch">Sprite batch to draw world-based entities to.</param>
    /// <param name="uiSpriteBatch">User interface sprite batch.</param>
    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
      _tiles.Draw(spriteBatch);
      GameObjects.GetSystem<RenderSystem>().Run();
    }

    /// <summary>
    /// Exit this scene.
    /// </summary>
    public override void Exit()
    {
      var collision = GameObjects.GetSystem<CollisionSystem>() as CollisionSystem;
      collision.SetTileMap(null);
      // End transition instantly
      TransitionState = TransitionState.Null;
    }

    /// <summary>
    /// Instantly pause the scene
    /// </summary>
    public override void Pause()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Instantly resume the scene
    /// </summary>
    public override void Resume()
    {
      // End transition instantly
      TransitionState = TransitionState.None;
    }
  }
}
