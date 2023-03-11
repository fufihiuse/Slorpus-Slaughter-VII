using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Slorpus.Interfaces.Base;
using Slorpus.Statics;
using Slorpus.Utils;

namespace Slorpus.Objects
{
    class PlayerProjectile : PhysicsObject, IUpdate, IDraw, IDestroyable, ILoad
    {
        Texture2D asset;

        int timesBounced = 0;

        int currentFrame;
        double timer;
        double frameLength;
        public override ushort Mask { get { return Constants.PLAYER_BULLET_COLLISION_MASK; } }
        
        private float mass = Constants.PLAYER_BULLET_MASS;
        public override float Mass { get { return mass; } }

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
                Game1.SquareTexture,
                target,
                Color.White
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
        public override bool OnCollision<T>(T other, CollisionInfo collision) 
        {
            // TODO: figure out why where are multiple conditionals here that check for if the other is player
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
                    timesBounced++;
                    SoundEffects.PlayEffectVolume("reflect", 0.6f, Math.Min(-0.1f + timesBounced*0.1f, 1)); // Plays firing off mirror sound effect
                    LevelInfo.Pause(3);
                    Camera.Shake(3, 5);

                    vel = Vector2.Reflect(vel, collision.Normal);
                }
                else if ((Mask & tempWall.Mask) > 0)
                {
                    Destroy();
                } else {
                    return false;
                }
            }
            else if (other is Enemy)
            {
                float shiftedPitch = Constants.ENEMY_VOLUME.MAX-Constants.ENEMY_VOLUME.MIN;
                float pitch =  shiftedPitch * ((float)Enemy.Count-1) / ((float)LevelInfo.InitialEnemyCount);
                // pitch = shiftedPitch - pitch; // inverts the pitch change
                pitch += Constants.ENEMY_VOLUME.MIN;
                Console.WriteLine("Pitch: " + pitch);
                SoundEffects.PlayEffectVolume("enemy_death", 0.6f, pitch, 0.0f);
                Enemy tempEnemy = (Enemy)(object)other;
                tempEnemy.Destroy();
            }
            else if (other is Player)
            {
                // KILL YOURSELF
                LevelInfo.ReloadLevel();
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
