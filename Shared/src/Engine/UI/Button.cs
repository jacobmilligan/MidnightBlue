//
// 	Button.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 3/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MidnightBlue.Engine.IO;
using MidnightBlue.Engine.UI;

namespace MidnightBlue.Engine.UI
{
  public class Button : UIControlElement
  {
    public Button(Texture2D normal, Texture2D selected, Texture2D pressed
    ) : base(normal, selected, pressed)
    { }

    public Button() : base(null, null, null) { }

    public override void Update()
    {
      base.Update();

      if ( _currentState == UIState.Selected && IOUtil.LeftMouseClicked() ) {
        if ( OnPress != null ) {
          if ( PressedSound != null && _previousState != UIState.Pressed ) {
            PressedSound.Play();
          }
          OnPress(this, EventArgs.Empty);
        }
      }
    }

    public event EventHandler OnPress;

    public SoundEffectInstance PressedSound { get; set; }
  }
}
