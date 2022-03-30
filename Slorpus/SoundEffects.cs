﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Slorpus
{
    class SoundEffects
    {
        // Field
        //Microsoft.Xna.Framework.Content.ContentManager content;
        List<SoundEffect> soundEffectsBank;

        // Constructor
        public SoundEffects()
        {
            soundEffectsBank = new List<SoundEffect>();
            //soundEffectsBank.Add(content.Load<SoundEffect>("Bullet Noise"));
        }

        // Method
        public void AddPlayerSounds(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            soundEffectsBank.Add(content.Load<SoundEffect>("Bullet Noise"));
        }

        public void AddEnemySounds(Microsoft.Xna.Framework.Content.ContentManager content)
        {

        }
        public void PlayEffect(int index)
        {
            soundEffectsBank[index].Play();
        }
    }
}
