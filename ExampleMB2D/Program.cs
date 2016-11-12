using MB2D;

namespace ExampleMB2D
{
  static class ExampleMB2D
  {
    private static MBGame game;
    static void Main(string[] args)
    {
      game = new MBGame(typeof(HelloScene));
      game.Run();
      game.Dispose();
    }
  }
}
