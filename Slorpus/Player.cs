using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public class Player : PhysicsObject
    {
        // Fields
        Texture2D playerAsset;
        Rectangle rectangle;
        int x;
        int y;
        KeyboardState keyboardState;

        // Constructor
        public Player(Point pos, Vector2 vel, int width, int height, Texture2D playerAsset): base(pos, vel, width, height)
        {
            rectangle = new Rectangle(x, y, width, height);
            this.playerAsset = playerAsset;
        }

        // Method
        /// <summary>
        /// Processes the Keyboard Input and adds it to velocity
        /// </summary>
        public void ProcessKeyboardInput()
        {
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.W) == true)
            {
                
            }
            if (keyboardState.IsKeyDown(Keys.A) == true)
            {

            }
            if (keyboardState.IsKeyDown(Keys.S) == true)
            {

            }
            if (keyboardState.IsKeyDown(Keys.D) == true)
            {

            }
        }
    }
}
