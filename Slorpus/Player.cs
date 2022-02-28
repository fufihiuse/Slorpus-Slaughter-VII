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
        public Player(Rectangle pos, Vector2 vel): base(pos, vel)
        {
            // placeholder
        }

        public void UpdatePlayerPosition()
        {
            KeyboardState kb = Keyboard.GetState();

            int xin = 0;
            int yin = 0;
            float speed = 0.5f;

            if (kb.IsKeyDown(Keys.W))
                yin -= 1;
            if (kb.IsKeyDown(Keys.S))
                yin += 1;
            if (kb.IsKeyDown(Keys.A))
                xin -= 1;
            if (kb.IsKeyDown(Keys.D))
                xin += 1;

            Velocity = new Vector2((Velocity.X + (xin * speed)) * 0.9f, (Velocity.Y + (yin * speed)) * 0.9f);

            
        }
    }
}
