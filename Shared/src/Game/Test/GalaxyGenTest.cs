using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MidnightBlue
{
  public class GalaxyGenTest : Scene
  {
    private Galaxy _galaxy;
    private SpriteFont _font;
    private int _seed;

    public GalaxyGenTest(ECSMap map, int size, int radius, int seed = 0) : base(map)
    {
      _galaxy = new Galaxy(size, radius);
      _seed = seed;
    }

    public override void Initialize()
    {
      _font = Content.Load<SpriteFont>("SourceCode");
      _galaxy.SetTexture(Content.Load<Texture2D>("Images/Galaxy"), 32);
      _galaxy.Generate(_seed);
    }

    public override void HandleInput()
    {
      GameObjects.GetSystem<InputSystem>().Run();
    }

    public override void Update()
    {

    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      _galaxy.Draw(spriteBatch, _font);
    }
  }
}

