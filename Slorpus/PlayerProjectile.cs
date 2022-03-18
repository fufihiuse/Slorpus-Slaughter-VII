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
        Vector2 subPix;

        public PlayerProjectile(Rectangle pos, Vector2 vel, Texture2D asset): base(pos, vel)
        {
            this.asset = asset;
        }

        public override void Move(Vector2 distance)
        {
            Point final_distance = new Point((int)distance.X, (int)distance.Y);
            // get just the decimal aspect of the movement vector and add it to subpixel values
            Vector2 new_sub = distance - new Vector2(final_distance.X, final_distance.Y);
            subPix += new_sub;
            // add the whole number values which may have been created by adding to subpixel
            final_distance.X += (int)new_sub.X;
            final_distance.Y += (int)new_sub.Y;
            // remove those integer parts from the subpixel value
            subPix.X %= 1;
            subPix.Y %= 1;

            base.Move(new Vector2(final_distance.X, final_distance.Y));
        }

        void IUpdate.Update(GameTime gameTime)
        {
            // perform per-frame game logic
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            sb.Draw(asset, Position, Color.White);
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
