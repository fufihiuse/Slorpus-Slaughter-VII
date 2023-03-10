﻿using System;
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
        private int maxRecursion; //Tracks amount of corrections

        public PhysicsManager(List<IPhysics> physicsObjects, List<Wall> wallList, BulletManager bulletManager, List<Wall> bowList)
        {
            this.physicsObjects = physicsObjects;
            this.wallList = wallList;
            this.bowList = bowList;
            this.bulletManager = bulletManager;
            maxRecursion = 0;
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

        private void ApplyConstraintImpulses(IPhysics body, Wall wall, float deltaTime)
        {
            CollisionInfo collision = wall.Collision(body.Position);
            if (!collision.Collided) { return; }
            
            // call the body's collision handler
            body.OnCollision(wall, collision);

            // if masked, then just call the handler and be done with it
            if ((wall.Mask & body.Mask) != 0) { return; }

            // begin calculating corrective impulses ------------------------------

            // impulse begins as the depth along the normal
            Vector2 impulse = new Vector2(collision.Normal.X, collision.Normal.Y);
            impulse *= collision.Depth;
            // scale downwards by the amount we actually want to correct
            impulse *= Constants.PHYSICS_CORRECTION_AMOUNT;
            // scale impulse by mass
            impulse *= body.Mass;
            
            // adjust  so that we cancel out the body's current velocity along the normal
            impulse += Vector2.Reflect(body.Velocity, collision.Normal);

            // apply the impulses
            body.Impulses += impulse *= deltaTime;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int i = 0; i < Constants.COLLISION_ITERATIONS; i++)
            {
                foreach (IPhysics physicsObject in physicsObjects)
                {
                    // get and apply impulses for forces like gravity and drag
                    ApplyUniversalImpulses(physicsObject, deltaTime);
                    physicsObject.ApplyImpulses();
                    
                    // apply impulses for constraining bodies to walls
                    foreach (Wall wall in wallList)
                    {
                        ApplyConstraintImpulses(physicsObject, wall, deltaTime);
                    }
                    foreach (Wall wall in bowList)
                    {
                        ApplyConstraintImpulses(physicsObject, wall, deltaTime);
                    }
                    physicsObject.ApplyImpulses();

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
}
