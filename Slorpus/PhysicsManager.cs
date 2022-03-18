using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    class PhysicsManager
    {
        private List<Wall> wallList;
        private List<IPhysics> physicsObjects;
        private BulletManager bulletManager;

        public PhysicsManager(List<IPhysics> physicsObjects, List<Wall> wallList, BulletManager bulletManager)
        {
            this.physicsObjects = physicsObjects;
            this.wallList = wallList;
            this.bulletManager = bulletManager;
        }

        public void AddPhysicsObject(IPhysics physicsObject)
        {
            physicsObjects.Add(physicsObject);
        }

        /// <summary>
        /// Moves objects which implement IPhysics and calls their respective collision handlers if necessary (STRETCH GOAL)
        /// </summary>
        public void MovePhysics(GameTime gameTime)
        {
            // declare only once to reduce memory reallocation
            // distance going to be moved
            Vector2 dist = new Vector2();
            
            // loop through each physics object and move it
            foreach (IPhysics physicsObject in physicsObjects)
            {
                // status of collision, used to break out of two loops
                bool no_collision = true;
                
                // distance we are going to move the object
                dist = physicsObject.GetVelocity();

                foreach (Wall wall in wallList)
                {
                    // check if currently colliding with wall, and if so then move away from it
                    // this is a failsafe, should not be necessary if the physics are working
                    if (physicsObject.Position.Intersects(wall.Position))
                    {
                        int adjustment_amount = 3;
                        // create vector pointing away from wall
                        double dir = Math.Atan2((physicsObject.Position.Y - wall.Position.Y), (physicsObject.Position.X - wall.Position.X));

                        physicsObject.Move(
                            new Vector2(
                                (float)Math.Cos(dir)*adjustment_amount,
                                (float)Math.Sin(dir)*adjustment_amount
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
                    physicsObject.Move(dist);
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
        private void moveWithoutCollision(IPhysics physicsObject, Vector2 velocity, Rectangle avoidBox)
        {           
            // which direction to increment in
            int sign = Math.Sign(velocity.X);
            // if we are moving on X, then move as far as we can
            if (sign != 0)
            {
                for (int i = 0; i < Math.Abs(velocity.X); i++)
                {
                    physicsObject.Move(new Vector2(sign, 0));
                    if (physicsObject.Position.Intersects(avoidBox))
                    {
                        // undo movement bc we're hitting now
                        physicsObject.Move(new Vector2(-sign, 0));
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
                    physicsObject.Move(new Vector2(0, sign));
                    if (physicsObject.Position.Intersects(avoidBox))
                    {
                        // undo movement bc we're hitting now
                        physicsObject.Move(new Vector2(0, -sign));
                        // we've hit a wall, lets lose our velocity
                        physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Moves bullets and calls bulletManager.Destroy if they hit a wall.
        /// </summary>
        /// <param name="size">The size that every physics object has.</param>
        public void CollideAndMoveBullets(GameTime gametime, Point size)
        {
            // indexes of bullets that need to be removed after this loop
            List<int> queuedBullets = new List<int>();
            
            for (int i = 0; i < bulletManager.Length; i++)
            {
                // placeholder, no collision
                // TODO : check if this works on value types, or if the changes pass out of scope after this loop
                bulletManager[i].Move(
                    new Point(
                        (int)bulletManager[i].Velocity.X,
                        (int)bulletManager[i].Velocity.Y
                        )
                    );
                
                foreach (IPhysics hit in physicsObjects)
                {
                    if (hit.Position.Contains(bulletManager[i].Position))
                    {
                        bulletManager[i].OnCollision<IPhysics>(hit);
                    }
                }
                for (int w = 0; w < wallList.Count; w++)
                {
                    if (wallList[w].Position.Contains(bulletManager[i].Position))
                    {
                        bulletManager[i].OnCollision<Wall>(wallList[w]);
                        queuedBullets.Add(i);
                    }
                }
            }
            bulletManager.DestroyBatch(queuedBullets.ToArray());
        }
    }
}
