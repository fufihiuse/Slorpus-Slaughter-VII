using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public class PlayerProjectile : PhysicsObject
    {
        public PlayerProjectile(Rectangle pos, Vector2 vel): base(pos, vel)
        {
            // placeholder
        }

        public void OnCollision<T>(T other) 
        {
            // get destroyed or play an effect or something when colliding with a wall
            if (typeof(T) == typeof(Wall))
            {
                Wall tempWall = (Wall)(object)other;
                if (tempWall.IsMirror)
                {
                    //TODO: Add logic
                }
            }
        }
    }
}
