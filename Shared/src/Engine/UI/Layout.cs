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

namespace MidnightBlue.Engine.UI
{
  public class Layout : UIElement
  {
    public Layout(int rows, int cols) : base(rows, cols) { }

    public override void Update()
    {
      var rowLen = Bounds.Content.GetLength(0);
      var colLen = Bounds.Content.GetLength(1);

      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = Bounds.Content[row, col];
          if ( currGrid != null ) {
            currGrid.Update();
          }
        }
      }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if ( (bool)MBGame.Console.Vars["drawBorders"] ) {
        DrawBorder(spriteBatch);
      }
      if ( (bool)MBGame.Console.Vars["drawGrids"] ) {
        spriteBatch.DrawGrid(Bounds.Bounds, new Point(Bounds.Rect.X, Bounds.Rect.Y), Color.Red);
      }

      var rowLen = Bounds.Content.GetLength(0);
      var colLen = Bounds.Content.GetLength(1);

      for ( int row = 0; row < rowLen; row++ ) {
        for ( int col = 0; col < colLen; col++ ) {
          var currGrid = Bounds.Content[row, col];
          if ( currGrid != null ) {
            currGrid.Draw(spriteBatch);
          }
        }
      }
    }

    public void Add(UIElement element, int atRow, int atCol, int rowSpan, int colSpan)
    {
      element.SetRelativeSize(Bounds, atRow, atCol, rowSpan, colSpan);
      Bounds.Content[atRow, atCol] = element;
    }
  }
}
