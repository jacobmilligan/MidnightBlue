//
// 	Layout.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 27/09/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MidnightBlue.Engine.Geometry;
using MonoGame.Extended.Shapes;

namespace MidnightBlue.Engine.UI
{
  public class Layout : UIElement
  {
    private UIGrid _grid;

    public Layout(int rows, int cols) : base(rows, cols) { }

    public override void Draw(SpriteBatch spriteBatch)
    {
      DrawBorder(spriteBatch);
    }
  }
}
