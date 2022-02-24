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
        private Texture2D texture;
        private bool collidable;

        //property
        public Rectangle Position
        {
            get
            {
                return position;
            }
        }

        //constuctor
        public Wall(Rectangle position, Texture2D texture, bool collidable=true)
        {
            this.position = position;
            this.texture = texture;
            this.collidable = collidable;
        }

        //methods
        /// <summary>
        /// draws wall
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            sb.Draw(
                texture, 
                position, 
                Color.White
                );
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
