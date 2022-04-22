using System;
using System.Collections.Generic;
using System.IO;
namespace Slorpus.Statics
{
    class LevelInfo
    {
        private static Game1 game;
        public static int CurrentLevel { get { return currentLevel; } }
        private static int currentLevel;
        private static List<string> levels;
        private static bool isCustom = false;
        private static string customPath;

        public List<string> Levels { get { return levels;} }

        public LevelInfo(Game1 game)
        {
            currentLevel = 0;
            LevelInfo.game = game;
            levels = new List<string>();
            foreach(string s in Constants.LEVELS)
            {
                levels.Add(s);
            }
        }

        public static void LoadCustomLevel(string filepath)
        {
            customPath = filepath;
            filepath = $"..\\..\\..\\customlevels\\{filepath}\\info.wal"; //UPDATE FOR BUILD
            StreamReader input = new StreamReader(filepath);
            string line;
            try
            {
                line = input.ReadLine();
                string[] tempLevels = line.Split(',');
                levels.Clear();
                foreach(string s in tempLevels)
                {
                    levels.Add(s);
                }
                isCustom = true;
            }
            catch(Exception)
            {
                throw new Exception("Error: file not found");
            }
        }

        public static void IncrementLevel()
        {
            currentLevel += 1;
            currentLevel %= levels.Count;
        }
        public static void ReloadLevel()
        {
            if (!isCustom)
            {
                game.LoadLevel(levels[CurrentLevel]);
            }
            else
            {
                game.LoadLevel(levels[CurrentLevel], customPath);
            }
        }
        
        /// <summary>
        /// Increments LevelInfo.CurrentLevel and loads that consecutive level.
        /// </summary>
        public static void LoadNextLevel()
        {
            IncrementLevel();
            if (!isCustom)
            {
                game.LoadLevel(levels[CurrentLevel]);
            }
            else
            {
                game.LoadLevel(levels[CurrentLevel], customPath);
            }
        }
    }
}
