using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Slorpus.Interfaces.Base;
using Slorpus.Statics;

namespace Slorpus.Objects
{
    class PlayerProjectile : PhysicsObject, IUpdate, IDraw, IDestroyable, ILoad
    {
        Texture2D asset;
        int currentFrame;
        double timer;
        double frameLength;

        public PlayerProjectile(Rectangle pos, Vector2 vel, ContentManager content): base(pos, vel)
        {
            currentFrame = 0;
            timer = 0;
            frameLength = 0.1;
            LoadContent(content);
        }

        //get player proj pos?

        void IUpdate.Update(GameTime gameTime)
        {
            // perform per-frame game logic
            UpdateFrame(gameTime);
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            sb.Draw(
                asset,
                new Vector2(target.X, target.Y),
                new Rectangle(16 * currentFrame, 0, 16, 16),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0.0f
                );
        }

        public void LoadContent(ContentManager content)
        {
            asset = content.Load<Texture2D>("bullet/cross");
        }

        public void Destroy()
        {
            Dereferencer.Destroy(this);
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

        /// <summary>
        /// updates the frame
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateFrame(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if (timer >= frameLength)
            {
                currentFrame++;
                if (currentFrame >= 4)
                {
                    currentFrame = 0;
                }
                timer -= frameLength;
            }
        }
    }
}
