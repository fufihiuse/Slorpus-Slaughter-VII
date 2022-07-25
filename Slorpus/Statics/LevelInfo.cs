using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slorpus.Interfaces.Base;

namespace Slorpus.Statics
{
    class LevelInfo
    {
        private Game1 game;
        private static LevelInfo current;
        public int initialEnemyCount;

        public static int InitialEnemyCount { get { return current.initialEnemyCount; } }
        public static Game1 Game { get { return current.game; } }
        public static int CurrentLevel { get { return currentLevel; } }
        private static int currentLevel;
        private static List<string> levels;
        private static bool isCustom = false;
        private static string customPath;

        private static bool _paused = false;
        public static bool Paused { get { return _paused; } }

        static int pauseTimer;
        const int length = Constants.LEVEL_COMPLETE_SPLASH_SCREEN_LENGTH * 60;

        static Action<SpriteBatch> draw;

        static bool InLevelSplash = false;

        public List<string> Levels { get { return levels;} }

        public LevelInfo(Game1 game)
        {
            currentLevel = 0;
            initialEnemyCount = 0;
            this.game = game;
            current = this;

            pauseTimer = length;

            draw = DrawNone;
            
            levels = new List<string>();
            foreach(string s in Constants.LEVELS)
            {
                levels.Add(s);
            }
        }

        public static void LoadCustomLevel(string filepath)
        {
            customPath = filepath;
            filepath = $"customlevels\\{filepath}\\info.wal";
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
                Game.LoadLevel(levels[CurrentLevel]);
            }
            else
            {
                Game.LoadLevel(levels[CurrentLevel], customPath);
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
                Game.LoadLevel(levels[CurrentLevel]);
            }
            else
            {
                Game.LoadLevel(levels[CurrentLevel], customPath);
            }
        }

        public static void LevelCompleted()
        {
            SoundEffects.PlayEffectVolume("levelcomplete", 0.8f, 0, 0);
            pauseTimer = length;
            InLevelSplash = true;
            _paused = true;
            draw = DrawLevelComplete;
        }

        public static void Update(GameTime gameTime)
        {
            if (pauseTimer > 0)
            {
                pauseTimer--;
            }
            else if (_paused)
            {
                if (InLevelSplash)
                    DisableLevelSplash();
                else
                    _paused = false;
            }
        }

        public static void Pause(int frames)
        {
            pauseTimer = frames;
            _paused = true;
        }

        static void DisableLevelSplash()
        {
            draw = DrawNone;
            _paused = false;
            InLevelSplash = false;
            LoadNextLevel();
        }

        public static void Draw(SpriteBatch sb)
        {
            draw(sb);
        }

        static void DrawLevelComplete(SpriteBatch sb)
        {
            sb.Draw(Game1.LevelCompleteSplash, Vector2.Zero, Color.White);
        }
        static void DrawNone(SpriteBatch sb) { }
    }
}
