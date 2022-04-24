
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

        public LevelInfo(Game1 game)
        {
            currentLevel = 0;
            initialEnemyCount = 0;
            this.game = game;
            current = this;
        }

        public static void IncrementLevel()
        {
            currentLevel += 1;
            currentLevel %= Constants.LEVELS.Length;
        }
        public static void ReloadLevel()
        {
            Game.LoadLevel(Constants.LEVELS[CurrentLevel]);
        }
        
        /// <summary>
        /// Increments LevelInfo.CurrentLevel and loads that consecutive level.
        /// </summary>
        public static void LoadNextLevel()
        {
            IncrementLevel();
            Game.LoadLevel(Constants.LEVELS[CurrentLevel]);
        }
    }
}
