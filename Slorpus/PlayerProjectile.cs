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
        private bool canReflect;
        private int timer;
        public PlayerProjectile(Rectangle pos, Vector2 vel, Texture2D asset): base(pos, vel)
        {
            this.asset = asset;
            canReflect = true;
            timer = 60;
        }

        void IUpdate.Update(GameTime gameTime)
        {
            // perform per-frame game logic
            if (!canReflect)
            {
                if(timer > 0)
                {
                    timer--;
                }
                else
                {
                    timer = 60;
                    canReflect = true;
                }
            }
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            sb.Draw(asset, target, Color.White);
        }
        public override void OnCollision<T>(T other) { }

        public override void OnCollisionComplex<T>(T other, Vector2 prevVel, Point wantedPosition) 
        {
            // get destroyed or play an effect or something when colliding with a wall
            if (typeof(T) == typeof(Wall))
            {
                Wall tempWall = (Wall)(object)other;
                if (tempWall.IsMirror && canReflect)
                {
                    if(Math.Abs(Position.X - wantedPosition.X) > Math.Abs(Position.Y - wantedPosition.Y))
                    {
                        vel = prevVel * new Vector2(-1, 1);
                        Console.WriteLine("Bounced on X");
                    }
                    else
                    {
                        vel = prevVel * new Vector2(1, -1);
                        Console.WriteLine("Bounced on Y");
                    }
                    //canReflect = false;
                }
            }
        }
    }
}
