using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Slorpus.Objects;
using Slorpus.Interfaces;
using Slorpus.Interfaces.Base;
using Slorpus.Statics;
using Slorpus.Utils;

namespace Slorpus.Managers
{
    class PhysicsManager: IUpdate
    {
        private List<Wall> wallList;
        private List<Wall> bowList;
        private List<IPhysics> physicsObjects;
        private BulletManager bulletManager;

        public PhysicsManager(List<IPhysics> physicsObjects, List<Wall> wallList, BulletManager bulletManager, List<Wall> bowList)
        {
            this.physicsObjects = physicsObjects;
            this.wallList = wallList;
            this.bowList = bowList;
            this.bulletManager = bulletManager;
        }

        private void ApplyUniversalImpulses(IPhysics body, float deltaTime)
        {
            // calculate drag (percentage reduction in velocity each frame)
            Vector2 drag = new Vector2(body.Velocity.X, body.Velocity.Y);
            drag *= Constants.UNIVERSAL_DRAG * -1 * deltaTime;

            // scale to the number of collision iterations (only apply 1/3) of drag per iteration
            // if there are 3 iterations
            drag *= 1.0f / Constants.COLLISION_ITERATIONS;

            body.Impulses += drag;
        }

        private bool ApplyConstraintImpulses(IPhysics body, Wall wall, float deltaTime)
        {
            CollisionInfo collision = wall.Collision(body.Position);
            if (!collision.Collided) { return false; }
            
            // call the body's collision handler
            body.OnCollision(wall, collision);

            // if masked, then just call the handler and be done with it
            if ((wall.Mask & body.Mask) != 0) { return false; }

            // begin calculating corrective impulses ------------------------------

            // impulse begins as the depth along the normal
            Vector2 impulse = new Vector2(collision.Normal.X, collision.Normal.Y);
            impulse *= collision.Depth;
            // scale downwards by the amount we actually want to correct
            impulse *= Constants.PHYSICS_CORRECTION_AMOUNT;
            // scale impulse by mass
            impulse *= body.Mass;
            
            // this is a really weird way of reflecting the velocit over the normal, but it only works for AABBs
            Vector2 axisToAdjust = new Vector2(Math.Abs(collision.Normal.X), Math.Abs(collision.Normal.Y));
            impulse += body.Velocity * axisToAdjust * -1;

            // apply the impulses
            body.Impulses += impulse *= deltaTime;

            return true;
        }

        private bool CallCollisionHandlers(IPhysics bodyA, IPhysics bodyB)
        {
            CollisionInfo collision = CollisionMath.Between(bodyA.Position, bodyB.Position);
            if (!collision.Collided) { return false; }

            bodyA.OnCollision(bodyB, collision);
            bodyB.OnCollision(bodyA, collision);

            return true;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // keep track of collision information for every physics object.
            CollisionHistory[] collisions = new CollisionHistory[physicsObjects.Count];
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                collisions[i] = new CollisionHistory(wallList.Count, bowList.Count, physicsObjects.Count);
            }

            // begin physics correction iterations
            for (int i = 0; i < Constants.COLLISION_ITERATIONS; i++)
            {
                for (int physObjIndex = 0; physObjIndex < physicsObjects.Count; physObjIndex++)
                {
                    IPhysics physicsObject = physicsObjects[physObjIndex];
                    // get and apply impulses for forces like gravity and drag
                    ApplyUniversalImpulses(physicsObject, deltaTime);
                    physicsObject.ApplyImpulses();
                    
                    // apply impulses for constraining bodies to walls
                    for (int wallIndex = 0; wallIndex < wallList.Count; wallIndex++)
                    {
                        if (collisions[physObjIndex].wallsCollidedWith[wallIndex])
                            continue;
                        Wall wall = wallList[wallIndex];
                        collisions[physObjIndex].wallsCollidedWith[wallIndex] = ApplyConstraintImpulses(physicsObject, wall, deltaTime);
                    }
                    // same thing but for BOWs (bullet-only walls)
                    for (int wallIndex = 0; wallIndex < bowList.Count; wallIndex++)
                    {
                        if (collisions[physObjIndex].bowsCollidedWith[wallIndex])
                            continue;
                        Wall wall = bowList[wallIndex];
                        collisions[physObjIndex].bowsCollidedWith[wallIndex] = ApplyConstraintImpulses(physicsObject, wall, deltaTime);
                    }
                    physicsObject.ApplyImpulses();

                    // do collision handler between dynamic bodies but don't actually make them collide (once per frame)
                    for (int otherIndex = 0; otherIndex < physicsObjects.Count; otherIndex++)
                    {
                        if (collisions[physObjIndex].physicsObjectsCollidedWith[otherIndex])
                            continue;
                        IPhysics other = physicsObjects[otherIndex];
                        if (other == physicsObject) { continue; }
                        collisions[physObjIndex].physicsObjectsCollidedWith[otherIndex] = CallCollisionHandlers(physicsObject, other);
                    }

                    // actually move the body
                    physicsObject.Move(physicsObject.Velocity);
                }
            }
            CollideAndMoveBullets(gameTime, new Point(Constants.BULLET_SIZE, Constants.BULLET_SIZE));
        }

        public void AddPhysicsObject(IPhysics physicsObject)
        {
            physicsObjects.Add(physicsObject);
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
    /// <summary>
    /// Stores information about what objects any given physics object has collided with.
    /// Used to prevent the collision handler between two objects from being called twice in a frame.
    /// </summary>
    struct CollisionHistory
    {
        public bool[] wallsCollidedWith;
        public bool[] bowsCollidedWith;
        public bool[] physicsObjectsCollidedWith;
        public CollisionHistory(int numberOfWalls, int numberOfBows, int numberOfPhysicsObjects)
        {
            wallsCollidedWith = new bool[numberOfWalls];
            bowsCollidedWith = new bool[numberOfBows];
            physicsObjectsCollidedWith = new bool[numberOfPhysicsObjects];
        }
    }
}
