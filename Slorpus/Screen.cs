using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * used to universally allow access to read the screen size
     * and other information about the camera
     */
    class Screen
    {
        Point localSize;
        static private Point size;
        static public Point Size { get { return size; } }

        public void SetScreenSize(Point size)
        {
            Screen.size = size;
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
