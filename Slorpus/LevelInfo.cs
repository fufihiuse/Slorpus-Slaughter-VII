using System;
using System.Collections.Generic;
using System.Text;

namespace Slorpus
{
    class LevelInfo
    {
        private static Game1 game;
        public static int CurrentLevel { get { return currentLevel; } }
        private static int currentLevel;

        public LevelInfo(Game1 game)
        {
            currentLevel = 0;
            LevelInfo.game = game;
        }

        public static void IncrementLevel()
        {
            currentLevel += 1;
            currentLevel %= Constants.LEVELS.Length;
        }
        public static void ReloadLevel()
        {
            game.LoadLevel(Constants.LEVELS[CurrentLevel]);
        }
        
        /// <summary>
        /// Increments LevelInfo.CurrentLevel and loads that consecutive level.
        /// </summary>
        public static void LoadNextLevel()
        {
            IncrementLevel();
            game.LoadLevel(Constants.LEVELS[CurrentLevel]);
        }
    }
}
