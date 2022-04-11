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
    class PhysicsObject: IPhysics
    {
        //fields
        protected Rectangle pos;
        protected Vector2 vel;
        protected Vector2 subPixelOffset;

        public Vector2 SubpixelOffset { get { return subPixelOffset; } }
        public virtual Rectangle Position { get { return pos;  } }
        public Vector2 Velocity { get { return vel;  } set { vel = value; } }
        public Vector2 SubpixelCoords { get
            {
                return new Vector2(
                    pos.X + subPixelOffset.X,
                    pos.Y + subPixelOffset.Y );
            }
        }

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
        public virtual Vector2 GetVelocity()
        {
            return vel;
        }
        /// <summary>
        /// Moves the object to a set of absolute coordinates
        /// </summary>
        /// <param name="location"></param>
        public virtual void Teleport(Point location)
        {
            pos.X = location.X;
            pos.X = location.X;
        }

        public virtual bool OnCollision<T>(T other) { return false; }
        public virtual bool OnCollisionComplex<T>(T other, Vector2 previousVelocity, Point wantedPosition) { return false; }

        /// <summary>
        /// moves the object a certain distance
        /// </summary>
        /// <param name="distance"></param>
        public virtual void Move(Vector2 distance)
        {
            // whole number element of the distance to move
            Vector2 intDist = new Vector2((int)distance.X, (int)distance.Y);
            // decimal element of the distance to move
            Vector2 floatDist = new Vector2(distance.X - intDist.X, distance.Y - intDist.Y);
            // add the decimal element to the accumulated offset
            subPixelOffset += floatDist;
            // if abs(subPixelOffset) > 1, then it needs to be added to the intDist and
            // removed from subpixel (at that point it is no longer a decimal)
            Vector2 overflow = new Vector2((int)subPixelOffset.X, (int)subPixelOffset.Y);
            intDist += overflow;
            subPixelOffset -= overflow;
            // intDist is now the proper distance to add to position with everythin accounted for
            pos.X += (int)intDist.X;
            pos.Y += (int)intDist.Y;
        }
    }
}
