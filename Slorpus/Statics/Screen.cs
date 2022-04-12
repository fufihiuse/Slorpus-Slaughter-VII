using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Slorpus.Statics
{
    /*
     * used to universally allow access to read the screen size
     * and other information about the camera
     */
    class Screen
    {
        static Screen current;
        GraphicsDeviceManager graphics;

        public Point targetSize;
        public Point trueSize;
        
        static public Point Size { get { return current.targetSize; } }

        static public Point TrueSize { get { return current.trueSize; } }
        
        /// <summary>
        /// The rectangle encompassing where the target size screen should be scaled
        /// and drawn to.
        /// </summary>
        static public Rectangle Target
        {
            get
            {
                return new Rectangle(
                    0, 0,
                    TrueSize.X,
                    TrueSize.Y
                    );
            }
        }
        
        /// <summary>
        /// The ratio of the actual screen size to the target screen size.
        /// Less than 1 if the screen is bigger than default,
        /// Greater than 1 if the screen is smaller.
        /// </summary>
        static public Vector2 Scale { get { return TrueSize.ToVector2() / Size.ToVector2(); } }

        /// <summary>
        /// Creates a new screen.
        /// </summary>
        /// <param name="size">The size of the screen in pixels.</param>
        public Screen(GraphicsDeviceManager graphics, bool use=true)
        {
            this.graphics = graphics;
            targetSize = new Point(
                Constants.SCREEN_WIDTH,
                Constants.SCREEN_HEIGHT
                );
            trueSize = targetSize;
            // set graphics size to the correct constants
            this.graphics.PreferredBackBufferWidth = targetSize.X;
            this.graphics.PreferredBackBufferHeight = targetSize.Y;
            this.graphics.ApplyChanges();
            
            if (use)
                Use();
        }
        
        /// <summary>
        /// Selects this instance of the Screen to have its variables as the static values.
        /// </summary>
        public void Use()
        {
            current = this;
        }
        
        public void OnResize(Object sender, EventArgs e)
        {
            if ((graphics.PreferredBackBufferWidth != graphics.GraphicsDevice.Viewport.Width) ||
                (graphics.PreferredBackBufferHeight != graphics.GraphicsDevice.Viewport.Height))
            {
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Viewport.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Viewport.Height;
                graphics.ApplyChanges();
            }

            // update screen variable
            trueSize = new Point(
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight
                );
        }
        
        /// <summary>
        /// Get the mouse position on the screen.
        /// </summary>
        /// <returns>The true position scaled downwards into the actual size.</returns>
        public Point GetMousePosition()
        {
            MouseState ms = Mouse.GetState();
            return new Vector2(ms.X * Scale.X, ms.Y * Scale.Y).ToPoint();
        }
    }
}
