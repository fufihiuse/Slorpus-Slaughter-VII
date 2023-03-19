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
        // this gets filled and then cleared each frame
        private Dictionary<int, Action> collisionHandlers;

        public PhysicsManager(List<IPhysics> physicsObjects, List<Wall> wallList, BulletManager bulletManager, List<Wall> bowList)
        {
            this.physicsObjects = physicsObjects;
            this.wallList = wallList;
            this.bowList = bowList;
            this.bulletManager = bulletManager;

            collisionHandlers = new Dictionary<int, Action>();
        }

        private void ApplyConstraintImpulses(IPhysics body, Wall wall, float deltaTime)
        {
            CollisionInfo collision = wall.Collision(body.Position, body.SubpixelOffset);
            if (!collision.Collided) { return; }

            // store the body's collision handler
            Action handler = () => { body.OnCollision(wall, collision); };
            int hash = HashDynamicOnStaticCollision(wall, body);
            collisionHandlers[hash] = handler;

            // if masked, then just make the handler and be done with it
            // (wall does not get collision handlers, wall.Mask is unused)
            if ((body.Mask & wall.Bit) == 0) { return; }

            // begin calculating corrective impulses ------------------------------

            // impulse begins as the depth along the normal
            Vector2 impulse = new Vector2(collision.Normal.X, collision.Normal.Y);
            impulse *= collision.Depth;
            // scale downwards by the amount we actually want to correct
            impulse *= Constants.PHYSICS_CORRECTION_AMOUNT;

            // this is a really weird way of reflecting the velocity over the normal, but it only works for AABBs
            Vector2 axisToAdjust = new Vector2(Math.Abs(collision.Normal.X), Math.Abs(collision.Normal.Y));
            impulse += body.Velocity * axisToAdjust * -1;
            
            // scale impulse by mass (impulses are all inversely scaled by mass, so this cancels that out)
            impulse *= body.Mass;

            // apply the impulses
            body.Impulses += impulse * deltaTime;
        }

        private void CreateCollisionHandlers(IPhysics bodyA, IPhysics bodyB)
        {
            CollisionInfo collision = CollisionMath.Between(bodyA.Position, bodyB.Position);
            if (!collision.Collided) { return; }

            Action handlerA = () => {
                if ((bodyA.Mask & bodyB.Bit) != 0)
                    bodyA.OnCollision(bodyB, collision);
            };
            int hashA = HashDynamicOnDynamicCollision(bodyA, bodyB);
            collisionHandlers[hashA] = handlerA;
            
            Action handlerB = () => {
                if ((bodyB.Mask & bodyA.Bit) != 0)
                    bodyB.OnCollision(bodyA, collision);
            };
            int hashB = HashDynamicOnDynamicCollision(bodyB, bodyA);
            collisionHandlers[hashB] = handlerB;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // keep track of where each object started
            foreach (IPhysics physicsObject in physicsObjects)
            {
                physicsObject.SyncPositionAtBeginningOfFrame();
            }

            // physics loop
            for (int i = 0; i < Constants.COLLISION_ITERATIONS; i++)
            {
                foreach (IPhysics physicsObject in physicsObjects)
                {
                    // try to resolve any collisions that have occurred as a result of movement
                    // apply impulses for constraining bodies to walls
                    foreach (Wall wall in wallList)
                    {
                        ApplyConstraintImpulses(physicsObject, wall, deltaTime);
                    }
                    // same but for bullet only walls
                    foreach (Wall wall in bowList)
                    {
                        ApplyConstraintImpulses(physicsObject, wall, deltaTime);
                    }
                    // do collision handler between dynamic bodies but don't actually make them collide
                    foreach (IPhysics other in physicsObjects)
                    {
                        if (other == physicsObject) { continue; }
                        CreateCollisionHandlers(physicsObject, other);
                    }

                    // cause the impulses on the object to manipulate the body
                    physicsObject.ApplyImpulses();
                    // actually move the body
                    physicsObject.Move(physicsObject.Velocity);
                }
            }
            CollideAndMoveBullets(gameTime, new Point(Constants.BULLET_SIZE, Constants.BULLET_SIZE));
            // call all the collision handlers and refresh for next frame
            foreach (KeyValuePair<int, Action> handler in collisionHandlers)
            {
                handler.Value();
            }
            collisionHandlers.Clear();
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
                        // everything in wallList should destroy bullets
                        queuedBullets.Add(i);
                    }
                }
            }
            bulletManager.DestroyBatch(queuedBullets.ToArray());
        }
        private int HashPhysicsBody(IPhysics body)
        {
            int result = body.ID;
            // physics bodies tend to never occupy the same location so
            // using XY for this should usually generate unique numbers
            result = (result * 397) ^ body.PositionAtBeginningOfFrame.X;
            result = (result * 397) ^ body.PositionAtBeginningOfFrame.Y;
            return result;
        }
        private int HashWall(Wall wall)
        {
            int result = wall.ID;
            // assumes that walls will not move during a physics iteration
            result = (result * 397) ^ wall.Position.Bottom;
            result = (result * 397) ^ wall.Position.Left;
            return result;
        }

        private int HashDynamicOnStaticCollision(Wall wall, IPhysics body)
        {
            int result = HashWall(wall);
            result = (result * 397) ^ HashPhysicsBody(body);
            return result;
        }
        private int HashDynamicOnDynamicCollision(IPhysics b1, IPhysics b2)
        {
            int result = HashPhysicsBody(b1);
            result = (result * 397) ^ HashPhysicsBody(b2);
            return result;
        }
    }
}
