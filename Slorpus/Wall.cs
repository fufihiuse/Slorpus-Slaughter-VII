using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public struct Wall
    {
        //fields
        private Rectangle position;
        private bool collidable;

        public Rectangle Position { get { return position; } }

        //constuctor
        public Wall(Rectangle position, bool collidable=true)
        {
            this.position = position;
            this.collidable = collidable;
        }

        /// <summary>
        /// returns if the hitbox has collided with the wall
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        public bool Collision(Rectangle hitbox)
        {
            return position.Intersects(hitbox);
        }
    }
}
