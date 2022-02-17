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
        Point pos;
        Vector2 vel;
        int width;
        int height;


        public Vector2 GetVelocity()
        {
            return vel;
        }

        public Point GetSize()
        {
            return new Point(width, height);
        }

        public void Move(Vector2 distance)
        {
            pos.X += (int)distance.X;
            pos.Y += (int)distance.Y;
        }
    }
}
