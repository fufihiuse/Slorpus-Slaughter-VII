﻿using System;
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

        int timesBounced = 0;

        public PlayerProjectile(Rectangle pos, Vector2 vel, ContentManager content): base(pos, vel)
        {
            LoadContent(content);
        }

        //get player proj pos?

        void IUpdate.Update(GameTime gameTime)
        {
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            sb.Draw(asset, target, Color.White);
        }

        public void LoadContent(ContentManager content)
        {
            asset = Game1.SquareTexture; // content.Load<Texture2D>("square");
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
                    timesBounced++;
                    SoundEffects.PlayEffect("reflect", Math.Min(-0.1f + timesBounced*0.1f, 1)); // Plays firing off mirror sound effect
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
                float shiftedPitch = Constants.ENEMY_VOLUME.MAX-Constants.ENEMY_VOLUME.MIN;
                float pitch =  shiftedPitch * ((float)Enemy.Count-1) / ((float)LevelInfo.InitialEnemyCount);
                // pitch = shiftedPitch - pitch; // inverts the pitch change
                pitch += Constants.ENEMY_VOLUME.MIN;
                SoundEffects.PlayEffect("enemy_death", pitch, 0.0f);
                Enemy tempEnemy = (Enemy)(object)other;
                tempEnemy.Destroy();
            }
            return false;
        }
    }
}
