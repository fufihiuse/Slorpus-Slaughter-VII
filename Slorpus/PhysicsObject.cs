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
        Rectangle pos;
        Vector2 vel;

        public Rectangle Position { get { return pos;  } }
        public Vector2 Velocity { get { return vel;  } set { vel = value; } }

        //constructor
        public PhysicsObject(Rectangle pos, Vector2 vel, int width, int height)
        {
            this.pos = pos;
            this.vel = vel;
        }
        
        // simple overload constructor that accepts a rectangle instead of separate width and heights
        public PhysicsObject(Rectangle pos, Vector2 vel)
        {
            this.pos = pos;
            this.vel = vel;
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
        /// Moves the object to a set of absolute coordinates
        /// </summary>
        /// <param name="location"></param>
        public void Teleport(Vector2 location)
        {
            pos.X = (int)location.X;
            pos.X = (int)location.X;
        }
        /// <summary>
        /// Moves the object to a set of absolute coordinates
        /// </summary>
        /// <param name="location"></param>
        public void Teleport(Point location)
        {
            pos.X = location.X;
            pos.X = location.X;
        }

        public virtual void OnCollision(Rectangle other) { }

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
