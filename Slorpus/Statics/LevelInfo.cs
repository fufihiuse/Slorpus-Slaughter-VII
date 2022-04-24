using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Slorpus.Interfaces.Base;
using System;

namespace Slorpus.Statics
{
    class LevelInfo
    {
        private static Game1 game;
        public static int CurrentLevel { get { return currentLevel; } }
        private static int currentLevel;

        private static bool _paused = false;
        public static bool Paused { get { return _paused; } }

        static int pauseTimer;
        const int length = Constants.LEVEL_COMPLETE_SPLASH_SCREEN_LENGTH * 60;

        static Action<SpriteBatch> draw;

        public LevelInfo(Game1 game)
        {
            currentLevel = 0;
            LevelInfo.game = game;

            pauseTimer = length;

            draw = DrawNone;
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

        public static void Update(GameTime gameTime)
        {
            if (pauseTimer > 0)
            {
                pauseTimer--;
            }
            else if (_paused)
            {
                DisableLevelSplash();
            }
        }

        public static void LevelCompleted()
        {
            pauseTimer = length;
            _paused = true;
            draw = DrawLevelComplete;
        }

        static void DisableLevelSplash()
        {
            draw = DrawNone;
            _paused = false;
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
