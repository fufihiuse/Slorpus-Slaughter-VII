using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    class PlayerProjectile : PhysicsObject, IUpdate, IDraw
    {
        Texture2D asset;
        public PlayerProjectile(Rectangle pos, Vector2 vel, Texture2D asset): base(pos, vel)
        {
            this.asset = asset;
        }

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

        public override void OnCollision<T>(T other) 
        {
            // get destroyed or play an effect or something when colliding with a wall
            if (typeof(T) == typeof(Wall))
            {
                Wall tempWall = (Wall)(object)other;
                if (tempWall.IsMirror)
                {
                    if(Math.Abs(vel.X) > Math.Abs(vel.Y))
                    {
                        vel *= new Vector2(-1, 1);
                    }
                    else
                    {
                        vel *= new Vector2(1, -1);
                    }
                }
            }
        }
    }
}
