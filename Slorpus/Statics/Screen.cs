using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Slorpus.Interfaces;

namespace Slorpus.Statics
{
    /*
     * used to universally allow access to read the screen size
     * and other information about the camera
     */
    class Screen: IKeyPress
    {
        static Screen current;
        GraphicsDeviceManager graphics;
        //GameWindow window;

        public Point targetSize;
        public Point trueSize;

        bool _isFullscreen = false;
        bool _isBorderless = false;

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
        static public Vector2 Scale { get { return Size.ToVector2() / TrueSize.ToVector2(); } }

        // stolen from : https://learn-monogame.github.io/how-to/fullscreen/
        private void SetFullscreen()
        {
            // width = Window.ClientBounds.Width;
            // height = Window.ClientBounds.Height;

            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.HardwareModeSwitch = !_isBorderless;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            
            _isFullscreen = true;
        }

        private void UnsetFullscreen()
        {
            graphics.PreferredBackBufferWidth = targetSize.X;
            graphics.PreferredBackBufferHeight = targetSize.Y;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            
            _isFullscreen = false;
        }

        public static void ToggleFullscreenSignal()
        {
            current.ToggleFullscreen();
        }

        private void ToggleFullscreen()
        {
            if (_isFullscreen)
            {
                UnsetFullscreen();
            }
            else
            {
                SetFullscreen();
            }
            trueSize = new Point(
                graphics.PreferredBackBufferWidth,
                graphics.PreferredBackBufferHeight
                );
        }

        /// <summary>
        /// Creates a new screen.
        /// </summary>
        public Screen(GraphicsDeviceManager graphics, GameWindow window, bool use=true)
        {
            this.graphics = graphics;
            // this.window = window;
            targetSize = new Point(
                Constants.SCREEN_WIDTH,
                Constants.SCREEN_HEIGHT
                );
            trueSize = targetSize;
            // set graphics size to the correct constants
            this.graphics.PreferredBackBufferWidth = targetSize.X;
            this.graphics.PreferredBackBufferHeight = targetSize.Y;
            this.graphics.ApplyChanges();
            
            // window settings
            window.AllowUserResizing = true;

            // subscribe OnResize method to resizing event
            window.ClientSizeChanged += OnResize;
            
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
            HandleShaderViewProjection();
        }
        
        /// <summary>
        /// Sets view projection of shader effects to match screen size.
        /// </summary>
        public void HandleShaderViewProjection()
        {
            Matrix view = Matrix.Identity;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.Size.X, Screen.Size.Y, 0, 0, 1);
            Matrix mul = view * projection;
            Game1.CRTFilter.Parameters["view_projection"].SetValue(mul);
            Game1.CRTFilterFullres.Parameters["view_projection"].SetValue(mul);
            Game1.WhiteFlash.Parameters["view_projection"].SetValue(mul);
        }
        
        /// <summary>
        /// Get the mouse position on the screen.
        /// </summary>
        /// <returns>The true position scaled downwards into the actual size.</returns>
        public static Point GetMousePosition()
        {
            MouseState ms = Mouse.GetState();
            return GetMousePosition(ms);
        }
        /// <summary>
        /// Get the mouse position on the screen.
        /// </summary>
        /// <param name="ms">Mouse State, unscaled to the actual screen size.</param>
        /// <returns>The true position scaled downwards into the actual size.</returns>
        public static Point GetMousePosition(MouseState ms)
        {
            /*
             * SHADER CODE IN crt-fullres.fx
            xy -= 0.5f;				// offcenter screen
            float r = xy.x * xy.x + xy.y * xy.y; // get ratio, x^2 + y^2
            xy *= (4.2f / CURVE_INTENSITY) + r; // apply ratio (curves the screen)
            xy *= 0.245f * CURVE_INTENSITY;				// zoom
            xy += 0.5f;				// move back to center
            */
            Vector2 pos = new Vector2(ms.X, ms.Y);
            // convert pos to propotion of the screen
            pos = Vector2.Divide(pos, TrueSize.ToVector2());
            pos -= new Vector2(0.5f, 0.5f);
            // dot product with itself
            float r = (pos.X * pos.X) + (pos.Y + pos.Y);
            // 0.13 is a hardcoded value that best makes the mouse feel good
            pos = Vector2.Multiply(pos, (4.2f / 0.13f) + r);
            pos = Vector2.Multiply(pos, 0.245f * 0.13f);
            
            pos += new Vector2(0.5f, 0.5f);
            pos = Vector2.Multiply(pos, TrueSize.ToVector2());
            return (pos * Scale).ToPoint();
        }
        
        // keypress handler for setting full screen
        void IKeyPress.OnKeyPress(KeyboardState previous)
        {
            KeyboardState kb = Keyboard.GetState();
            if (kb.IsKeyDown(Keys.LeftControl) && kb.IsKeyDown(Keys.F))
            {
                ToggleFullscreen();
            }
        }
    }
}
