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
            // declare stuff once to reduce memory allocation and reallocation
            Vector2 prev = new Vector2();
            // loop through each physics object and move it
            foreach (IPhysics physicsObject in physicsObjects)
            {
                // record the previous position
                prev = physicsObject.SubpixelCoords;
                prev.X += physicsObject.Position.Width / 2;
                prev.Y += physicsObject.Position.Height / 2;
                // move the physics object
                physicsObject.Move(physicsObject.Velocity);

                // now check if it's colliding with any walls n stuff
                foreach (Wall wall in wallList)
                {
                    if (wall.Collision(physicsObject.Position))
                    {
                        // handler
                        physicsObject.OnCollision<Wall>(wall);
                        // correct the location of the object to no be colliding
                        if (physicsObject.GetType() != typeof(PlayerProjectile))
                        {
                            CorrectObject(wall, prev, physicsObject);
                        }

                        // if you collide, remove sub pixel collision so as to prevent
                        // the object "technically" being inside the wall but not really
                        physicsObject.Move(-physicsObject.SubpixelOffset);
                        break;
                    }
                }
            }
        }

        private void CorrectObject(Wall collided, Vector2 previousPos, IPhysics physicsObject)
        {
            // this will get multiplied by the overlap amount to create the correction
            Vector2 correctionCoeff;
            // distance from where the object was last from to the wall
            Vector2 diff = new Vector2(
                previousPos.X,
                previousPos.Y
                ) - new Vector2(
                    collided.Position.Center.X,
                    collided.Position.Center.Y
                    );
            // amount the wall and object are currently overlapping
            Rectangle overlapRect = Rectangle.Intersect(collided.Position, physicsObject.Position);
            Vector2 overlap = new Vector2(overlapRect.Width, overlapRect.Height);

            // generate correction coefficients
            float absX = Math.Abs(diff.X);
            float absY = Math.Abs(diff.Y);
            if (absX > absY)
            {
                correctionCoeff = new Vector2(Math.Sign(diff.X), 0);
                // we are correcting X which means we collided on that axis
                // remove X velocity
                physicsObject.Velocity = new Vector2(0, physicsObject.Velocity.Y);
            }
            else
            {
                correctionCoeff = new Vector2(0, Math.Sign(diff.Y));
                // we are correcting Y which means we collided on that axis
                // remove Y velocity
                physicsObject.Velocity = new Vector2(physicsObject.Velocity.X, 0);
            }

            // move object by the right correction amount
            physicsObject.Move(Vector2.Multiply(correctionCoeff, overlap));

            // check if recursion is necessary
            foreach (Wall wall in wallList)
            {
                if (wall.Collision(physicsObject.Position))
                {
                    physicsObject.OnCollision<Wall>(wall);
                    CorrectObject(wall, previousPos, physicsObject);
                    break;
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
