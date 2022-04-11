using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    class PlayerProjectile : PhysicsObject, IUpdate, IDraw, IDestroyable, ILoad
    {
        Texture2D asset;
        
        public PlayerProjectile(Rectangle pos, Vector2 vel, ContentManager content): base(pos, vel)
        {
            LoadContent(content);
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
