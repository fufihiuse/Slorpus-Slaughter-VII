﻿
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

        public struct ENEMY_VOLUME {
            public const float MAX = 1.0f;
            public const float MIN = -0.5f;
        }

        public struct PLAYER
        {
            public const int STEP_SPEED = 20;
            public const int SIZE = 16;
            public const int BULLET_SIZE = 10;
            public const float BULLET_SPEED = 6f;
        }

        //Array of levels to load
        public static readonly string[] LEVELS = { 
            "ItBegins", //1
            "ReflectOnIt", //2
            "OutsideTheBox", //3
            "Frittata", //4
            "Diamond", //5
            "TMI", //6
            "Exeunt" }; //End
        //Awesome idea: seven room either gives you god mode and has a bunch of enemies, OR, counter for how many bullet bounces in 7 room
    }
}
