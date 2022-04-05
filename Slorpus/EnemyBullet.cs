using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * struct for the enemy bullet which includes minimal information about the bullet itself
     * drawn in batches
     * includes some copy pasted code from PhysicsObject, but structs can't inheirit so oh well
     */
    public struct EnemyBullet: IPointPhysics
    {
        //fields
        Point pos;
        Vector2 vel;
        public Point Position { get { return pos;  } set { pos = value;  } }
        public Vector2 Velocity { get { return vel;  } set { vel = value; } }

        //Constructor
        public EnemyBullet(Point position, Vector2 velocity)
        {
            pos = position;
            vel = velocity;
        }
        /// <summary>
        /// Moves the object to a set of absolute coordinates
        /// </summary>
        /// <param name="location"></param>
        public void Teleport(Point location)
        {
            pos = new Point(location.X, location.Y);
        }

        public void OnCollision<T>(T other)
        {
            // get destroyed or play an effect or something when colliding with a wall
            // hurt player when colliding with them
            if (typeof(T) == typeof(Wall))
            {
                // bullet hit wall, etc
            }
            else if(typeof(T) == typeof(Player))
            {
                
            }
        }
        
        /// <summary>
        /// returns velocity
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVelocity()
        {
            return vel;
        }
        /// <summary>
        /// moves object
        /// </summary>
        /// <param name="distance"></param>
        public void Move(Point distance)
        {
            pos.X += distance.X;
            pos.Y += distance.Y;
        }
    }
}
