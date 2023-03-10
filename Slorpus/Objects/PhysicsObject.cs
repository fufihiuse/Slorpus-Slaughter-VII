using Microsoft.Xna.Framework;

using Slorpus.Interfaces;

namespace Slorpus.Objects
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
        protected Vector2 impulses;
        protected Vector2 subPixelOffset;
        protected float mass;

        public Vector2 SubpixelOffset { get { return subPixelOffset; } }
        public virtual Rectangle Position { get { return pos;  } }
        public Vector2 Velocity { get { return vel;  } set { vel = value; } }
        public Vector2 Impulses { get { return impulses;  } set { impulses = value; } }
        public float Mass { get { return mass; } }
        public Vector2 SubpixelCoords { get
            {
                return new Vector2(
                    pos.X + subPixelOffset.X,
                    pos.Y + subPixelOffset.Y );
            }
        }

        public void ApplyImpulses()
        {
            // scale by inverse of mass
            impulses.X *= 1.0f / mass;
            impulses.Y *= 1.0f / mass;
            // apply
            vel += impulses;
            // no warm starting (impulses don't carry over)
            impulses.X = 0;
            impulses.Y = 0;
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
