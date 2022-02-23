using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * Every class that needs to implement IPhysics but doesn't already have a parent class
     * should inherit from this so as to minimize copypasting code
     */
    public class PhysicsObject: IPhysics
    {
        //fields
        Point pos;
        Vector2 vel;
        int width;
        int height;

        public Point Position { get { return pos;  } }
        public Point Size { get { return new Point(width, height);  } }
        public Vector2 Velocity { get { return vel;  } set { vel = value; } }

        //constructor
        public PhysicsObject(Point pos, Vector2 vel, int width, int height)
        {
            this.pos = pos;
            this.vel = vel;
            this.width = width;
            this.height = height;
        }
        
        // simple overload constructor that accepts a rectangle instead of separate width and heights
        public PhysicsObject(Point pos, Vector2 vel, Rectangle rect)
        {
            this.pos = pos;
            this.vel = vel;
            this.width = rect.Width;
            this.height = rect.Height;
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
        /// returns size
        /// </summary>
        /// <returns></returns>
        public Point GetSize()
        {
            return new Point(width, height);
        }

        /// <summary>
        /// moves the object a certian distance
        /// </summary>
        /// <param name="distance"></param>
        public void Move(Vector2 distance)
        {
            pos.X += (int)distance.X;
            pos.Y += (int)distance.Y;
        }
    }
}
