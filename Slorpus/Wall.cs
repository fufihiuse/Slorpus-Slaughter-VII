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
        Rectangle position;
        Texture2D texture;

        //property
        public Rectangle Position
        {
            get
            {
                return position;
            }
        }

        //constuctor
        public Wall(Rectangle position, Texture2D texture)
        {
            this.position = position;
            this.texture = texture;
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
    }
}
