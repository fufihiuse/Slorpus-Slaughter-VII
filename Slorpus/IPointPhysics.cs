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
        /// <summary>
        /// returns velocity
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
