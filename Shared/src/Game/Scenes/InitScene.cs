//
// 	InitScene.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by jacobmilligan on 25/10/2016.
// 	Copyright  All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MB2D;
using MB2D.EntityComponent;
using MB2D.IO;
using MB2D.Scenes;
using MonoGame.Extended.Shapes;

namespace MidnightBlue
{
  /// <summary>
  /// The scene shown at the title screen.
  /// </summary>
  public class InitScene : Scene
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MidnightBlue.InitScene"/> class.
    /// Loads all blueprints and setup data.
    /// </summary>
    /// <param name="map">Game objects.</param>
    /// <param name="content">Content manager for loading textures and sounds.</param>
    public InitScene(EntityMap map, ContentManager content) : base(map, content) { }

    /// <summary>
    /// Registers all blueprints to the EntityMap
    /// </summary>
    public override void Initialize()
    {
      GameObjects.Reset();

      GameObjects.AddSystem<ShipInputSystem>();

      GameObjects.MakeBlueprint("galaxy playership", MakeGalaxyPlayership);
      GameObjects.MakeBlueprint("starsystem playership", MakeStarSystemPlayerShip);
      GameObjects.MakeBlueprint("player", MakePlayer);
      GameObjects.MakeBlueprint("planet ship", MakeShip);

      // Setup player
      Entity player = GameObjects.CreateEntity("player");
      player.Attach<UtilityController>();
      player.Persistent = true;

      var utility = GameObjects["player"].GetComponent<UtilityController>() as UtilityController;
      AssignMenu(utility);

      SceneController.ResetTo(new TitleScene(GameObjects, Content));

      if ( !MBGame.Console.Funcs.ContainsKey("SetSpeed") ) {
        MBGame.Console.AddFunc("SetSpeed", SetSpeed);
      }

      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Sets the speed of the player via the debug console
    /// </summary>
    /// <param name="args">Arguments. Will be null.</param>
    private void SetSpeed(string[] args)
    {
      if ( args.Length > 0 ) {
        var newSpeed = 0.0f;
        var movement = GameObjects["player"].GetComponent<Movement>();
        if ( float.TryParse(args[0], out newSpeed) && movement != null ) {
          movement.Speed = newSpeed;
        }
      }
    }

    /// <summary>
    /// Handles the input for the scene.
    /// </summary>
    public override void HandleInput() { }

    /// <summary>
    /// Updates the scene.
    /// </summary>
    public override void Update() { }

    /// <summary>
    /// Draws the scene to the uiSpriteBatch
    /// </summary>
    /// <param name="spriteBatch">Sprite batch for world-based entities.</param>
    /// <param name="uiSpriteBatch">User interface sprite batch.</param>
    public override void Draw(SpriteBatch spriteBatch, SpriteBatch uiSpriteBatch)
    {
    }

    /// <summary>
    /// Exits the scene.
    /// </summary>
    public override void Exit()
    {
      TransitionState = TransitionState.Null;
    }

    /// <summary>
    /// Pauses the scene.
    /// </summary>
    public override void Pause()
    {
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Resumes the scene.
    /// </summary>
    public override void Resume()
    {
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Defines a blueprint for creating the player ship to be called from
    /// the entity map.
    /// </summary>
    /// <param name="entity">Entity to transform.</param>
    private void MakeGalaxyPlayership(Entity entity)
    {
      var utilityController = entity.GetComponent<UtilityController>() as UtilityController;
      utilityController.InputMap.Search<MenuCommand>().Disabled = false;

      // Setup sprite
      var sprite = entity.Attach<SpriteTransform>(
        Content.Load<Texture2D>("Images/playership_blue"),
        new Vector2(MBGame.Camera.Position.X + 100, MBGame.Camera.Position.Y + 100),
        new Vector2(0.3f, 0.3f)
      ) as SpriteTransform;
      sprite.Z = 1;

      // Setup AABB
      entity.Attach<CollisionComponent>(
        new RectangleF[] { sprite.Target.GetBoundingRectangle() }
      );

      var shipController = entity.Attach<ShipController>() as ShipController;
      shipController.State = ShipState.Normal;

      var physics = entity.Attach<PhysicsComponent>() as PhysicsComponent;
      physics.Power = 0;
      physics.Velocity = new Vector2(0, 0);

      // Setup initial inventory
      var inventory = entity.Attach<Inventory>() as Inventory;
      if ( !inventory.Items.ContainsKey(typeof(Fuel)) ) {
        inventory.Items.Add(typeof(Fuel), new Fuel(10000));
      }

      var movement = entity.Attach<Movement>(3.0f, 0.02f) as Movement;
      movement.Speed = 3.0f;
      movement.RotationSpeed = 0.02f;
    }

    /// <summary>
    /// Defines a blueprint to use when altering the players ship when
    /// entering a start system view.
    /// </summary>
    /// <param name="entity">Entity.</param>
    private void MakeStarSystemPlayerShip(Entity entity)
    {
      var movement = entity.GetComponent<Movement>();
      movement.Speed = 25.0f;
      movement.RotationSpeed = 0.05f;

      var sprite = entity.GetComponent<SpriteTransform>();
      sprite.Target.Scale = new Vector2(0.3f, 0.3f);
      entity.GetComponent<ShipController>().State = ShipState.Normal;

      if ( !entity.HasComponent<CollisionComponent>() ) {
        entity.Attach<CollisionComponent>(new RectangleF[] {
          sprite.Target.GetBoundingRectangle()
        });
      }
    }

    /// <summary>
    /// Defines a blueprint that gives the entity the necessary 
    /// components to become a controllable player.
    /// </summary>
    /// <param name="entity">Entity to change.</param>
    private void MakePlayer(Entity entity)
    {
      entity.Detach<SpriteTransform>(); // resets the sprite
      entity.Detach<ShipController>(); // remove any other controllers if they exist

      var movement = entity.GetComponent<Movement>();
      movement.Speed = 200;

      var physics = entity.GetComponent<PhysicsComponent>();
      physics.Velocity = new Vector2(0, 0);

      var sprite = entity.Attach<SpriteTransform>(
        Content.Load<Texture2D>("Images/bkspr01"),
        new Vector2(MBGame.Camera.Position.X, MBGame.Camera.Position.Y),
        new Vector2(0.4f, 0.4f)
      ) as SpriteTransform;

      entity.Attach<CollisionComponent>(new RectangleF[] {
        sprite.Target.GetBoundingRectangle()
      });

      // Setup new controller components
      var playerController = entity.Attach<PlayerController>() as PlayerController;
      playerController.InputMap.Assign<LaunchCommand>(
        Keys.Space, CommandType.Trigger
      );

      entity.Attach<UtilityController>();
    }

    /// <summary>
    /// Defines a blueprint that gives the entity the correct 
    /// components to become a controllable ship in the planet scene
    /// </summary>
    /// <param name="entity">Entity to change.</param>
    private void MakeShip(Entity entity)
    {
      var lastPos = MBGame.Camera.GetBoundingRectangle().Center;
      var lastAngle = 0.0f;

      // Remember position and angle from being a non-ship
      // for setting up the new sprite transform
      if ( entity.HasComponent<Movement>() ) {
        lastPos = entity.GetComponent<Movement>().Position;
        lastAngle = entity.GetComponent<Movement>().Angle;
      }

      entity.DetachAll();

      // Setup sprite transform facing the same way as before in the same position
      var sprite = entity.Attach<SpriteTransform>(
        Content.Load<Texture2D>("Images/playership_blue"),
        new Vector2(MBGame.Camera.Position.X, MBGame.Camera.Position.Y),
        new Vector2(0.8f, 0.8f)
      ) as SpriteTransform;

      sprite.Z = 1;
      sprite.Rotation = lastAngle;

      entity.Attach<PhysicsComponent>();

      // Attach movement component
      var movement = entity.Attach<Movement>(1000.0f, 0.1f) as Movement;
      movement.Position = lastPos;

      entity.Attach<ShipController>();
      var utility = entity.Attach<UtilityController>() as UtilityController;
      AssignMenu(utility);

      GameObjects.UpdateSystems(entity);
    }

    /// <summary>
    /// Assigns the menu command to a utility controller
    /// </summary>
    /// <param name="utility">Utility controller to assign to.</param>
    private void AssignMenu(UtilityController utility)
    {
      var menuCommand = utility.InputMap.Assign<MenuCommand>(
          Keys.Escape, CommandType.Trigger, SceneController, Content
        );
      menuCommand.Disabled = false;
    }
  }
}


