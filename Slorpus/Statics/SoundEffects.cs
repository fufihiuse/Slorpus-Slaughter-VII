using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Slorpus.Statics
{
    /// <summary>
    /// Holds a list of SoundEffects that are used based on different situations in the game
    /// </summary>
    class SoundEffects
    {
        // Field
        //Microsoft.Xna.Framework.Content.ContentManager content;
        static Dictionary<string, SoundEffect> soundEffectsBank;

        // Constructor
        /// <summary>
        /// Creates a new SoundEffects object and declares a new list that can be filled with SoundEffects
        /// </summary>
        public SoundEffects()
        {
            soundEffectsBank = new Dictionary<string, SoundEffect>(); 
        }

        // Methods
        /// <summary>
        /// Adds each of the sounds needed in the game
        /// </summary>
        /// <param name="content"></param>
        public static void AddSounds(ContentManager content)
        {
            Action<string> Add = (string name) =>
            {
                string path = $"sound/{name}";
                soundEffectsBank.Add(name, content.Load<SoundEffect>(path));
            };

            Add("bullet");
            Add("click");
            Add("enemy_bullet");
            Add("enemy_death");
            Add("reflect");
            Add("walk1");
            Add("walk2");
            Add("levelcomplete");
            Add("levelcomplete-alt");
            Add("title-card");
            Add("startbutton");
        }

        /// <summary>
        /// Play a sound by name.
        /// </summary>
        /// <param name="name">Name of the sound in the dictionary.</param>
        public static void PlayEffect(string name)
        {
            soundEffectsBank[name].Play();
        }
        
        /// <summary>
        /// Play an effect with a specified pitch.
        /// </summary>
        /// <param name="name">Name of the effect</param>
        /// <param name="pitch">Pitch of the effect, between 0.0 and 1.0</param>
        public static void PlayEffect(string name, float pitch, float pan=0.0f)
        {
            soundEffectsBank[name].Play(1.0f, pitch, pan);
        }

        public static void Stop(string name)
        {
            soundEffectsBank[name].Dispose();
        }

        public static void PlayEffectVolume(string name, float volume, float pitch=0.0f, float pan=0.0f)
        {
            soundEffectsBank[name].Play(volume, pitch, pan);
        }
    }
}
