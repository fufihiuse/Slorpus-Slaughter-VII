
namespace Slorpus.Statics
{
    // only stores elements of type public const
    public static class Constants
    {
        // true constants
        public const int WALL_SIZE = 16;
        public const int ENEMY_SIZE = 16;
        public const int BULLET_SIZE = 5;
        public const float CAMERA_SPEED = 0.05f;
        public const float MIN_FOLLOW_DISTANCE = 250f;
        public const float MIN_DETECTION_DISTANCE = 400;
        
        // this should be order of magnitudes bigger than the number of objects
        // ever expected to be on the screen (most possible UUIDs should be unused)
        public const int UUID_MAX = 10000;

        public const int COLLISION_ITERATIONS = 10;
        public const float PLAYER_DRAG = 0.5f;

        private const ushort PLAYER_COLLISION_BIT =          0b00001000;
        private const ushort WALL_COLLISION_BIT =            0b00000100;
        private const ushort BOW_COLLISION_BIT =             0b00000010;
        private const ushort PLAYER_BULLET_COLLISION_BIT =   0b00000001;
        private const ushort ENEMY_COLLISION_BIT =           0b00010000;

        public const float PHYSICS_CORRECTION_AMOUNT = 0.5f;

        public const float PLAYER_MASS = 1;
        public const float ENEMY_MASS = 1;
        public const float PLAYER_BULLET_MASS = 1;
        public const float PLAYER_MOVE_IMPULSE = 0.1f;
        public const float PLAYER_FRICTION_COEFFICIENT = 0.17f;
        
        // basically just a global scalar to friction
        public const float GRAVITY = 0.5f;
        
        // player only collides with walls
        public const ushort PLAYER_COLLISION_MASK =
            WALL_COLLISION_BIT |
            BOW_COLLISION_BIT |
            PLAYER_COLLISION_BIT;
        // bullet also collides with BOWs
        public const ushort PLAYER_BULLET_COLLISION_MASK =
            WALL_COLLISION_BIT |
            PLAYER_BULLET_COLLISION_BIT;
        // walls collide with everything
        public const ushort WALL_COLLISION_MASK = 254;
        // BOWs just collide with bullets
        public const ushort BOW_COLLISION_MASK =
            PLAYER_COLLISION_BIT |
            ENEMY_COLLISION_BIT |
            BOW_COLLISION_BIT;
        public const ushort ENEMY_COLLISION_MASK =
            PLAYER_BULLET_COLLISION_BIT |
            PLAYER_COLLISION_BIT |
            WALL_COLLISION_BIT |
            ENEMY_COLLISION_BIT;

        public struct ENEMY_VOLUME {
            public const float MAX = 1.0f;
            public const float MIN = -0.5f;
        }

        public struct PLAYER
        {
            public const int STEP_SPEED = 20;
            public const int SIZE = 16;
            public const int BULLET_SIZE = 10;
            public const float BULLET_SPEED = 1f;
        }
      
        // public const int SCREEN_WIDTH = 480;
        // public const int SCREEN_HEIGHT = 270;
        public const int SCREEN_WIDTH = 800;
        public const int SCREEN_HEIGHT = 480;

        public const int LEVEL_COMPLETE_SPLASH_SCREEN_LENGTH = 3;

        //Array of levels to load
        public static readonly string[] LEVELS = { 
            // "autotiling", // level containing every possible arrangement of tiles
            "ItBegins",
            "ReflectOnIt", //2
            "OutsideTheBox", //3
            "Frittata", //4
            "Spiral",
            "BlissArmy",//5
            "Diamond",
            //5
            "TMI", //6
            "Exeunt",
            "Smile"}; //End
        //Awesome idea: seven room either gives you god mode and has a bunch of enemies, OR, counter for how many bullet bounces in 7 room
    }
}
