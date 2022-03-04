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
        private bool isMirror;

        public Rectangle Position { get { return position; } }
        public bool IsMirror { get { return isMirror; } }

        //constuctor
        public Wall(Rectangle position, bool collidable = true, bool isMirror = true)
        {
            this.position = position;
            this.collidable = collidable;
            this.isMirror = isMirror;
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
