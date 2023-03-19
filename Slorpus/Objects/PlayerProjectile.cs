using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Slorpus.Interfaces.Base;
using Slorpus.Statics;
using Slorpus.Utils;
using Slorpus.Managers;

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
        public override ushort Bit { get { return Constants.PLAYER_BULLET_COLLISION_BIT; } }
        
        private float mass = Constants.PLAYER_BULLET_MASS;
        public override float Mass { get { return mass; } }

        private Vector2 intendedVelocity;

        private Vector2 lastNormal;
        private Point lastWallPosition;

        public PlayerProjectile(Rectangle pos, Vector2 vel, ContentManager content): base(pos, vel)
        {
            currentFrame = 0;
            timer = 0;
            frameLength = 0.1;
            LoadContent(content);
            intendedVelocity = vel;

            // two impossible values for both of the following variables
            lastNormal = Vector2.Zero;
            lastWallPosition = new Point(int.MaxValue, int.MaxValue);
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
                    if (collision.Normal != lastNormal && NotHittingCornerOfWall(tempWall, collision.Normal))
                    {
                        timesBounced++;
                        SoundEffects.PlayEffectVolume("reflect", 0.6f, Math.Min(-0.1f + timesBounced * 0.1f, 1)); // Plays firing off mirror sound effect
                        LevelInfo.Pause(3);
                        Camera.Shake(3, 5);

                        vel = Vector2.Reflect(intendedVelocity, collision.Normal);
                        lastNormal = collision.Normal;
                        lastWallPosition = tempWall.Position.Location;
                        intendedVelocity = vel;
                    }
                }
                else if ((Mask & tempWall.Bit) > 0)
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
        /// Used to mitigate the effects of tiling, where a bullet can collide with the corner
        /// of a tile in a wall, getting a normal different than the one the whole wall obviously
        /// has. Could also be accomplished by fusing adjacent wall tiles into one rectangle during
        /// level load.
        /// </summary>
        /// <param name="newCollision">The wall being collided with, which may be invalid</param>
        /// <param name="normal">The normal of the collision with newCollision.</param>
        /// <returns>True if this collision should cause a reflection.</returns>
        private bool NotHittingCornerOfWall(Wall newCollision, Vector2 normal)
        {
            Point loc = newCollision.Position.Location;
            Point tileCoords = new Point(loc.X / Constants.WALL_SIZE, loc.Y / Constants.WALL_SIZE);

            // now ensure that there is not another mirror wall in the direction of the normal
            int X = tileCoords.X + (Math.Sign(normal.X) * (int)Math.Ceiling(Math.Abs(normal.X)));
            int Y = tileCoords.Y + (Math.Sign(normal.Y) * (int)Math.Ceiling(Math.Abs(normal.Y)));
            X = Math.Clamp(X, 1, Level.Size.X);
            Y = Math.Clamp(Y, 1, Level.Size.Y);
            char adjacent = Level.Map[Y-1, X-1];

            Console.WriteLine(adjacent);

            Console.WriteLine("this tiles coords: " + tileCoords);
            Console.WriteLine("this tile: " + Level.Map[tileCoords.Y, tileCoords.X]);
            Console.WriteLine("adjacent tiles coords: " + "X: " + X + "Y: " + Y);
            Console.WriteLine("adjacent tile: " + adjacent);
            
            return adjacent != 'M' && adjacent != 'W';
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
