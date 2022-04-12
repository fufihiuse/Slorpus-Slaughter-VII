using Microsoft.Xna.Framework;

namespace Slorpus.Statics
{
    /*
     * used to universally allow access to read the screen size
     * and other information about the camera
     */
    class Screen
    {
        Point localSize;
        static private Point size;
        static private Point trueSize;
        static public Point Size { get { return size; } }

        static public Point TrueSize { get { return trueSize; } }

        static public Vector2 Scale { get { return trueSize.ToVector2() / size.ToVector2(); } }

        public void SetScreenSize(Point size)
        {
            Screen.size = size;
        }
        public void SetTrueScreenSize(Point size)
        {
            Screen.trueSize = size;
        }
        /// <summary>
        /// Creates a new screen.
        /// </summary>
        /// <param name="size">The size of the screen in pixels.</param>
        public Screen(Point size)
        {
            localSize = size;
        }
        
        /// <summary>
        /// Sets the global values for screen size equal to this screen's size.
        /// </summary>
        public void Use()
        {
            size = localSize;
        }
    }
}
