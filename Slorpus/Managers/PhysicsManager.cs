using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    class PhysicsManager: IUpdate
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

        public void Update(GameTime gameTime)
        {
            MovePhysics(gameTime);
            CollideAndMoveBullets(gameTime, new Point(Constants.BULLET_SIZE, Constants.BULLET_SIZE));
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

                Point wantedLocation = physicsObject.Position.Location;
                Vector2 previousVelocity = physicsObject.Velocity;

                // now check if it's colliding with any walls n stuff
                foreach (Wall wall in wallList)
                {
                    if (wall.Collision(physicsObject.Position))
                    {
                        // skip bullet hitting non bullet collider walls
                        if (!wall.IsBulletCollider && physicsObject is PlayerProjectile) { continue; }
                        // correct the location of the object to no be colliding
                        CorrectObject(wall, prev, physicsObject);

                        // handlers
                        bool cancel_collision_complex = physicsObject.OnCollisionComplex(wall, previousVelocity, wantedLocation);
                        bool cancel_collision = physicsObject.OnCollision(wall);

                        if (!cancel_collision && !cancel_collision_complex)
                        {
                            // if you collide, remove sub pixel collision so as to prevent
                            // the object "technically" being inside the wall but not really
                            physicsObject.Move(-physicsObject.SubpixelOffset);
                            break;
                        }
                        else
                        {
                            // cancel the collision by returning to previous state
                            Vector2 wantedLocationV = new Vector2(wantedLocation.X, wantedLocation.Y);
                            Vector2 newLoc = new Vector2(physicsObject.Position.X, physicsObject.Position.Y);
                            physicsObject.Move(-(newLoc - wantedLocationV));
                            physicsObject.Velocity = previousVelocity;
                        }
                    }
                }

                foreach (IPhysics other in physicsObjects)
                {
                    if (physicsObject == other)
                    {
                        continue;
                    }
                    if (physicsObject.Position.Intersects(other.Position))
                    {
                        // note: its not consistent which object will have its handler called first
                        physicsObject.OnCollision(other);
                        physicsObject.OnCollisionComplex(other, previousVelocity, wantedLocation);
                        other.OnCollision(physicsObject);
                        other.OnCollisionComplex(physicsObject, previousVelocity, wantedLocation);
                    }
                }
            }
        }
        
        /// <summary>
        /// Moves an object so that is it no longer colliding with a certain wall,
        /// and then recurses to ensure it is not colliding with any other walls.
        /// </summary>
        /// <param name="collided">The wall being collided with.</param>
        /// <param name="previousPos">Position of the object before it began overlapping.</param>
        /// <param name="physicsObject">The object that is colliding.</param>
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
                bulletManager[i].Move(bulletManager[i].Velocity);
                
                foreach (IPhysics hit in physicsObjects)
                {
                    if (hit.Position.Contains(bulletManager[i].Position))
                    {
                        bulletManager[i].OnCollision(hit);
                    }
                }
                for (int w = 0; w < wallList.Count; w++)
                {
                    if (wallList[w].Position.Contains(bulletManager[i].Position))
                    {
                        bool destroyed = bulletManager[i].OnCollision(wallList[w]);
                        if (destroyed)
                        {
                            queuedBullets.Add(i);
                        }
                    }
                }
            }
            bulletManager.DestroyBatch(queuedBullets.ToArray());
        }
    }
}
