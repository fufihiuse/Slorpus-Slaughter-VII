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
            
            // loop through each physics object and move it
            foreach (IPhysics physicsObject in physicObjects)
            {
                // status of collision, used to break out of two loops
                bool no_collision = true;
                
                // distance we are going to move the object
                dist = physicsObject.GetVelocity();

                foreach (Wall wall in walls)
                {
                    // TODO: cache all collided walls, and call moveWithoutCollision with the closest one
                    if (wall.Collision(physicsObject.Position))
                    {
                        physicsObject.Teleport(moveWithoutCollision(physicsObject, dist, wall.Position));
                        physicsObject.OnCollision(wall.Position);
                        no_collision = false;
                        // we hit something, give up for efficiency's sake
                        break;
                    }
                }
                
                // move if we didnt already
                if (no_collision)
                    physicsObject.Move(dist);
            }
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
            // rectangle used to check collisions
            Rectangle oRect = physicsObject.Position;            
            // which direction to increment in
            int sign = Math.Sign(velocity.X);
            // if we are moving on X, then move as far as we can
            if (sign != 0)
            {
                for (int i = 0; i < Math.Abs(velocity.X); i++)
                {
                    oRect.X += sign;
                    if (!oRect.Intersects(avoidBox))
                    {
                        // we've hit a wall, lets lose our velocity
                        oRect.X -= sign;
                        physicsObject.Velocity = new Vector2(0, physicsObject.Velocity.Y);
                        break;
                    }
                }
            }

            sign = Math.Sign(velocity.Y);
            // if we are moving on X, then move as far as we can
            if (sign != 0)
            {
                for (int i = 0; i < Math.Abs(velocity.Y); i++)
                {
                    oRect.Y += sign;
                    if (!oRect.Intersects(avoidBox))
                    {
                        // we've hit a wall, lets lose our velocity
                        oRect.Y -= sign;
                        physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
                        break;
                    }
                }
            }
            // return the new location
            return new Vector2(oRect.X, oRect.Y);
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
