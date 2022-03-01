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
                    // check if currently colliding with wall, and if so then move away from it
                    // this is a failsafe, should not be necessary if the physics are working
                    if (physicsObject.Position.Intersects(wall.Position))
                    {
                        int adjustment_amount = 3;
                        // create vector pointing away from wall
                        double dir = Math.Atan2((physicsObject.Position.Y - wall.Position.Y), (physicsObject.Position.X - wall.Position.X));

                        physicsObject.Move(
                            new Point(
                                (int)Math.Cos(dir)*adjustment_amount,
                                (int)Math.Sin(dir)*adjustment_amount
                                )
                            );
                    }

                    Rectangle new_loc = physicsObject.Position;
                    new_loc.X += (int)dist.X;
                    new_loc.Y += (int)dist.Y;
                    // TODO: cache all collided walls, and call moveWithoutCollision with the closest one
                    if (wall.Collision(new_loc))
                    {
                        // call the physic object's collision handler
                        physicsObject.OnCollision<Wall>(wall);
                        
                        moveWithoutCollision(physicsObject, dist, wall.Position);

                        // we hit something, give up for efficiency's sake
                        no_collision = false;
                        break;
                    }
                }
                
                // move if we didnt already
                if (no_collision)
                {
                    Point real_dist = new Point(
                        (int)dist.X,
                        (int)dist.Y
                        );
                    physicsObject.Move(real_dist);
                }
            }
        }
         
        /// <summary>
        /// Moves an object that implements IPhysics by a velocity, but stops if it hits a rectangle.
        /// </summary>
        /// <param name="physicsObject">Physics object to move.</param>
        /// <param name="velocity">Velocity by which the object should be moved.</param>
        /// <param name="avoidBox">Rectangle hitbox to collide with.</param>
        /// <returns>Distance to move the object.</returns>
        private static void moveWithoutCollision(IPhysics physicsObject, Vector2 velocity, Rectangle avoidBox)
        {           
            // which direction to increment in
            int sign = Math.Sign(velocity.X);
            // if we are moving on X, then move as far as we can
            if (sign != 0)
            {
                for (int i = 0; i < Math.Abs(velocity.X); i++)
                {
                    physicsObject.Move(new Point(sign, 0));
                    if (physicsObject.Position.Intersects(avoidBox))
                    {
                        // undo movement bc we're hitting now
                        physicsObject.Move(new Point(-sign, 0));
                        // we've hit a wall, lets lose our velocity
                        physicsObject.Velocity = new Vector2(0, physicsObject.Velocity.Y);
                        break;
                    }
                }
            }

            sign = Math.Sign(velocity.Y);
            // if we are moving on Y, then move as far as we can
            if (sign != 0)
            {
                for (int i = 0; i < Math.Abs(velocity.Y); i++)
                {
                    physicsObject.Move(new Point(0, sign));
                    if (physicsObject.Position.Intersects(avoidBox))
                    {
                        // undo movement bc we're hitting now
                        physicsObject.Move(new Point(0, -sign));
                        // we've hit a wall, lets lose our velocity
                        physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Physics object movement for if a long list of physics objects have identical sizes.
        /// Does not prevent them from colliding but does call OnCollision
        /// </summary>
        /// <param name="physicObjects">Objects that implement IPointPhysics at least.</param>
        /// <param name="size">The size that every physics object has.</param>
        /// <param name="walls">List of walls that should stop movement.</param>
        public static void CollideAndMoveBullets(EnemyBullet[] bullets, Point size, List<Wall> wallList, List<IPhysics> physicsList)
        {
            // indexes of bullets that need to be removed after this loop
            List<int> queuedBullets = new List<int>();

            for (int i = 0; i < bullets.Length; i++)
            {
                // whether or not current bullet should be removed at the end of the loop
                bool removed = false;
                // placeholder, no collision
                // TODO : check if this works on value types, or if the changes pass out of scope after this loop
                bullets[i].Move(
                    new Point(
                        (int)bullets[i].Velocity.X,
                        (int)bullets[i].Velocity.Y
                        )
                    );
                
                foreach (IPhysics hit in physicsList)
                {
                    if (hit.Position.Contains(bullets[i].Position))
                    {
                        bullets[i].OnCollision<IPhysics>(hit);
                    }
                }
                for (int w = 0; w < wallList.Count; w++)
                {
                    if (wallList[w].Position.Contains(bullets[i].Position))
                    {
                        bullets[i].OnCollision<Wall>(wallList[w]);
                        removed = true;
                    }
                }

                if (removed)
                {
                    queuedBullets.Add(i);
                }
            }
            // remove all queued bullets
            EnemyBullet[] narray = new EnemyBullet[bullets.Length - queuedBullets.Count];
            int counter = 0;

            for (int i = 0; i < bullets.Length; i++)
            {
                // dont copy over any of the queued to be removed bullets
                if (queuedBullets.Contains(i))
                {
                    continue;
                }
                narray[counter] = bullets[i];
                counter++;
            }

            // replace bullets with the new array
            bullets = narray;
        }
    }
}
