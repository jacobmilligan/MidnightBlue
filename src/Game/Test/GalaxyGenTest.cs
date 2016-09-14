using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MidnightBlueMono
{
  public class GalaxyGenTest : Scene
  {
    private Galaxy _galaxy;
    private SpriteFont _font;

    public GalaxyGenTest(ECSMap map, int size, int radius, int seed = 0) : base(map)
    {
      _galaxy = new Galaxy(size, radius);
      _galaxy.Generate(seed);
    }

    public override void Initialize()
    {
      _font = Content.Load<SpriteFont>("SourceCode");
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

