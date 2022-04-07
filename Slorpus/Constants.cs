using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    // only stores elements of type public const
    public static class Constants
    {
        // true constants
        public const int WALL_SIZE = 16;
        public const int ENEMY_SIZE = 16;
        public const int PLAYER_SIZE = 16;
        public const int BULLET_SIZE = 5;
        public const int PLAYER_BULLET_SIZE = 10;
        public const float PLAYER_BULLET_SPEED = 6f;
        public const float CAMERA_SPEED = 0.05f;
        public const float MIN_FOLLOW_DISTANCE = 250f;
        public const float MIN_DETECTION_DISTANCE = 400;

        //Array of levels to load
        public static readonly string[] LEVELS = { "1", "2", "3", "5", "6", "end" };
    }
}
