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
        Vector2 subpixel_pos;
        public Point Position { get { return pos;  } set { pos = value;  } }
        public Vector2 Velocity { get { return vel;  } set { vel = value; } }

        //Constructor
        public EnemyBullet(Point position, Vector2 velocity)
        {
            pos = position;
            vel = velocity;
            subpixel_pos = new Vector2(0, 0);
        }
        /// <summary>
        /// Moves the object to a set of absolute coordinates
        /// </summary>
        /// <param name="location"></param>
        public void Teleport(Point location)
        {
            pos = new Point(location.X, location.Y);
        }

        public bool OnCollision<T>(T other)
        {
            // get destroyed or play an effect or something when colliding with a wall
            // hurt player when colliding with them
            if (typeof(T) == typeof(Wall))
            {
                Wall temp = (Wall)(object)other;
                // bullet hit wall, etc
                
                if (!temp.IsBulletCollider)
                {
                    return false;
                }
                // normal wall, destroy this bullet
                return true;
            }
            else if (typeof(T) == typeof(Player))
            {
                LevelInfo.ReloadLevel();
            }
            return false;
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
        public void Move(Vector2 distance)
        {
            // round down the distance to move
            Point whole_speed = distance.ToPoint();
            // sub-pixel speed ( decimal values missing from rounding )
            Vector2 sub_speed = distance - whole_speed.ToVector2();
            subpixel_pos += sub_speed;

            // cumulative change in position based on sub-pixel coordinates
            Point cummove = subpixel_pos.ToPoint();
            // remove the cumulative changes from the subpixel position
            subpixel_pos -= cummove.ToVector2();
            // add them to the actual position
            pos += cummove + whole_speed;
        }
    }
}
