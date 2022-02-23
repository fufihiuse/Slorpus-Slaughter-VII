using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public interface IPointPhysics
    {
        public Point Position { get; }
        public Vector2 Velocity { get; set; }


        /// <summary>
        /// returns velocity
        /// this is just here for backwards compatibility, use the Velocity property instead.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVelocity();
        /// <summary>
        /// moves object
        /// </summary>
        /// <param name="distance"></param>
        public void Move(Vector2 distance);
    }
}
