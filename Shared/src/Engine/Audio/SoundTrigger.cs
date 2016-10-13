//
// 	SoundTrigger.cs
// 	Midnight Blue
//
// 	--------------------------------------------------------------
//
// 	Created by Jacob Milligan on 13/10/2016.
// 	Copyright (c) Jacob Milligan All rights reserved
//
using System;
using Microsoft.Xna.Framework.Audio;

namespace MidnightBlue.Engine
{
  public class SoundTrigger
  {
    private SoundEffect _sound;
    private SoundEffectInstance _instance;

    public SoundTrigger(SoundEffect sound)
    {
      _sound = sound;
      _instance = _sound.CreateInstance();
      FadeSpeed = 0.05f;
      MaxVolume = 0.5f;
    }

    public void Trigger()
    {
      if ( _instance.State == SoundState.Stopped ) {
        _instance.Play();
      }
    }

    public void FadeUp()
    {
      if ( _instance.State == SoundState.Stopped ) {
        _instance.Play();
        _instance.Volume = 0.0f;
      }
      if ( _instance.Volume < MaxVolume ) {
        _instance.Volume += FadeSpeed;
      }
    }

    public void FadeDown()
    {
      if ( _instance.Volume > FadeSpeed ) {
        _instance.Volume -= FadeSpeed;
      } else {
        _instance.Stop();
      }
    }

    public float FadeSpeed { get; set; }
    public float MaxVolume { get; set; }
    public bool IsLooped
    {
      get { return _instance.IsLooped; }
      set { _instance.IsLooped = value; }
    }
  }
}
