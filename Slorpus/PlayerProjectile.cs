using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    class PlayerProjectile : PhysicsObject, IUpdate, IDraw, IDestroyable
    {
        Texture2D asset;
        Action<IDestroyable> destroy;
        
        /// <summary>
        /// Creates a bullet projectile which would be fired by the player.
        /// </summary>
        /// <param name="pos">Starting position of the bullet.</param>
        /// <param name="vel">Starting velocity of the bullet, usually constant.</param>
        /// <param name="asset">The bullet's texture.</param>
        /// <param name="destroy">Function that is called on the bullet to destroy it.</param>
        public PlayerProjectile(Rectangle pos, Vector2 vel, Texture2D asset, Action<IDestroyable> destroy): base(pos, vel)
        {
            this.asset = asset;
            this.destroy = destroy;
        }

        //get player proj pos?

        void IUpdate.Update(GameTime gameTime)
        {
            // perform per-frame game logic
            
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            sb.Draw(asset, target, Color.White);
        }

        public void Destroy()
        {
            destroy(this);
        }
        
        // Handles bouncing off mirrors.
        public override bool OnCollisionComplex<T>(T other, Vector2 prevVel, Point wantedPosition) 
        {
            if (other is Player)
            {
                return false;
            }
            // get destroyed or play an effect or something when colliding with a wall
            if (other is Wall)
            {
                Wall tempWall = (Wall)(object)other;
                if (tempWall.IsMirror)
                {
                    SoundEffects.PlayEffect(2); // Plays firing off mirror sound effect
                    // get if the normal is primarily X or Y
                    if(Math.Abs(Position.X - wantedPosition.X) > Math.Abs(Position.Y - wantedPosition.Y))
                    {
                        // reflect across the Y axis
                        vel = prevVel * new Vector2(-1, 1);
                    }
                    else
                    {
                        // reflect across the X axis
                        vel = prevVel * new Vector2(1, -1);
                    }
                }
                else
                {
                    if (!tempWall.IsBulletCollider)
                    {
                        // cancel collision
                        return true;
                    }
                    else
                    {
                        // just hit a regular wall, now destroy
                        Destroy();
                    }
                }
            }
            else if (other is Enemy)
            {
                Enemy tempEnemy = (Enemy)(object)other;
                tempEnemy.Destroy();
            }
            return false;
        }
    }
}
