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
        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        public Player(Rectangle pos, Vector2 vel): base(pos, vel)
        {}

        /// <summary>
        /// Updates the player position by detecting the keys pressed by the player
        /// </summary>
        /// <param name="kb"></param>
        public void UpdatePlayerPosition(KeyboardState kb)
        {
            kb = Keyboard.GetState();
            float speed = 0.5f;
            float xin = 0;
            float yin = 0;
            float xTemp = 0;
            float yTemp = 0;

            if (kb.IsKeyDown(Keys.W))
            { yTemp = -1; }

            if (kb.IsKeyDown(Keys.S))
            { yTemp = 1; }

            if (kb.IsKeyDown(Keys.A))
            { xTemp = -1; }

            if (kb.IsKeyDown(Keys.D))
            { xTemp = 1; }

            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.D))
            {
                xTemp = 0.707f;
                yTemp = -0.707f;
            }
            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.A))
            {
                xTemp = -0.707f;
                yTemp = -0.707f;
            }
            if (kb.IsKeyDown(Keys.A) && kb.IsKeyDown(Keys.S))
            {
                xTemp = -0.707f;
                yTemp = 0.707f;
            }
            if (kb.IsKeyDown(Keys.S) && kb.IsKeyDown(Keys.D))
            {
                xTemp = 0.707f;
                yTemp = 0.707f;
            }
            if (kb.IsKeyDown(Keys.A) && kb.IsKeyDown(Keys.D))
            {
                xTemp = 0f;
            }
            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.S))
            {
                yTemp = 0f;
            }

            xin += xTemp;
            yin += yTemp;

            Velocity = new Vector2((Velocity.X + (xin * speed)) * 0.9f, (Velocity.Y + (yin * speed)) * 0.9f);
        }
    }
}
