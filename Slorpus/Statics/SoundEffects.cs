using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Slorpus.Statics
{
    /// <summary>
    /// Holds a list of SoundEffects that are used based on different situations in the game
    /// </summary>
    class SoundEffects
    {
        // Field
        //Microsoft.Xna.Framework.Content.ContentManager content;
        static List<SoundEffect> soundEffectsBank;

        // Constructor
        /// <summary>
        /// Creates a new SoundEffects object and declares a new list that can be filled with SoundEffects
        /// </summary>
        public SoundEffects()
        {
            soundEffectsBank = new List<SoundEffect>();
        }

        // Methods
        /// <summary>
        /// Adds each of the sounds needed in the game
        /// </summary>
        /// <param name="content"></param>
        public static void AddSounds(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            soundEffectsBank.Add(content.Load<SoundEffect>("Bullet Noise"));
            soundEffectsBank.Add(content.Load<SoundEffect>("Enemy Bullet Noise"));
            soundEffectsBank.Add(content.Load<SoundEffect>("Bullet bounces off wall"));
            soundEffectsBank.Add(content.Load<SoundEffect>("Enemy Death"));
        }

        /// <summary>
        /// Plays a specific sound in the list decided by the parameter
        /// </summary>
        /// <param name="index"></param>
        public static void PlayEffect(int index)
        {
            soundEffectsBank[index].Play();
        }
    }
}
