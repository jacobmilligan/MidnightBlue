using MB2D.EntityComponent;
using MB2D.Scenes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleMB2D
{
  /// <summary>
  /// My first scene
  /// </summary>
  public class HelloScene : Scene
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:MyGame.HelloScene"/> class.
    /// </summary>
    /// <param name="gameObjects">Game objects.</param>
    /// <param name="content">Content manager for loading textures and sounds.</param>
    public HelloScene(EntityMap gameObjects, ContentManager content) : base(gameObjects, content)
    {

    }

    /// <summary>
    /// Runs any initialization logic and transitions until
    /// the transition state is changed
    /// </summary>
    public override void Initialize()
    {

      // Required otherwise the initialize transition will never end
      TransitionState = TransitionState.None;
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
    /// Exits the scene and runs any transitions until
    /// the transition state is changed. Once this transition
    /// finishes the scene will be gone for good.
    /// </summary>
    public override void Exit()
    {
      // Required otherwise the exit transition will never end
      TransitionState = TransitionState.Null;
    }

    /// <summary>
    /// Pauses the scene and runs any transitions until
    /// the transition state is changed
    /// </summary>
    public override void Pause()
    {
      // Required otherwise the pause transition will never end
      TransitionState = TransitionState.None;
    }

    /// <summary>
    /// Resumes the scene and runs any transitions until
    /// the transition state is changed
    /// </summary>
    public override void Resume()
    {
      // Required otherwise the resume transition will never end
      TransitionState = TransitionState.None;
    }
  }
}