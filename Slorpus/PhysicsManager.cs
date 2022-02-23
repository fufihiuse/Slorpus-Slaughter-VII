using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public static class PhysicsManager
    {
        /// <summary>
        /// Moves objects which implement IPhysics and calls their respective collision handlers if necessary (STRETCH GOAL)
        /// </summary>
        /// <param name="physicObjects">List of physics objects with speeds and sizes that need to be moved.</param>
        /// <param name="walls">List of walls that should stop movement.</param>
        public static void MovePhysics(List<IPhysics> physicObjects, List<Wall> walls)
        {
            // declare only once to reduce memory reallocation
            // distance going to be moved
            Vector2 dist = new Vector2();
            // fake hitbox for where the object is going to be (check if it will intersect with walls)
            Rectangle check = new Rectangle();
            // the rectangle of the hit wall
            Rectangle hitbox = new Rectangle();

            foreach (IPhysics p in physicObjects)
            {
                // status of collision, used to break out of two loops
                bool no_collision = true;
                // placeholder, no collision
                // TODO : check if this works on value types, or if the changes pass out of scope after this loop
                dist = p.GetVelocity();

                // set the values of check rectangle to match prospective player position
                check.X = p.Position.X + (int)dist.X;
                check.Y = p.Position.Y + (int)dist.Y;

                foreach (Wall w in walls)
                {
                    // check to see if any walls are where we are trying to go, if not then don't worry
                    hitbox.X = w.Position.X;
                    hitbox.Y = w.Position.Y;

                    // TODO: cache all collided walls, and call moveWithoutCollision with the closest one
                    if (check.Intersects(hitbox))
                    {
                        p.Move(moveWithoutCollision(p, dist, hitbox));
                        // we hit something, give up for efficiency's sake
                        no_collision = false;
                        break;
                    }
                }
                
                // move if we didnt already
                if (no_collision)
                    p.Move(dist);
            }
        }
        
        /// <summary>
        /// Caculates the distance between two monogame Points.
        /// </summary>
        /// <param name="a">Point A.</param>
        /// <param name="b">Point B.</param>
        /// <returns>Double distance between two points.</returns>
        public static double Distance(Point a, Point b)
        {
            return Math.Sqrt( (b.X - a.X) + (b.Y - a.Y) );
        }
        
        /// <summary>
        /// Moves an object that implements IPhysics by a velocity, but stops if it hits a rectangle.
        /// </summary>
        /// <param name="physicsObject">Physics object to move.</param>
        /// <param name="velocity">Velocity by which the object should be moved.</param>
        /// <param name="avoidBox">Rectangle hitbox to collide with.</param>
        /// <returns>Distance to move the object.</returns>
        private static Vector2 moveWithoutCollision(IPhysics physicsObject, Vector2 velocity, Rectangle avoidBox)
        {
            float new_x = Math.Max(physicsObject.Position.X + velocity.X,
                avoidBox.X + (Math.Sign(avoidBox.X - physicsObject.Position.X)*(avoidBox.Width/2)));
            float new_y = Math.Max(physicsObject.Position.Y + velocity.Y,
                avoidBox.X + (Math.Sign(avoidBox.Y - physicsObject.Position.Y)*(avoidBox.Width/2)));
            // return the new location minus the old
            return new Vector2(new_x - physicsObject.Position.X, new_y - physicsObject.Position.Y);
        }
        
        /// <summary>
        /// Physics object movement for if a long list of physics objects have identical sizes.
        /// </summary>
        /// <param name="physicObjects">Objects that implement IPointPhysics at least.</param>
        /// <param name="size">The size that every physics object has.</param>
        /// <param name="walls">List of walls that should stop movement.</param>
        public static void MovePointPhysics(List<IPointPhysics> physicObjects, Point size, Wall[,] walls)
        {
            foreach (IPhysics p in physicObjects)
            {
                // placeholder, no collision
                // TODO : check if this works on value types, or if the changes pass out of scope after this loop
                p.Move(p.GetVelocity());
            }
        }
    }
}
