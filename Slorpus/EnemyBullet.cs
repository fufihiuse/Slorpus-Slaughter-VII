using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * struct for the enemy bullet which includes minimal information about the bullet itself
     * drawn in batches
     * includes some copy pasted code from PhysicsObject, but structs can't inheirit so oh well
     */
    public struct EnemyBullet: IPointPhysics
    {
        Point pos;
        Vector2 vel;

        public EnemyBullet(Point position, Vector2 velocity)
        {
            pos = position;
            vel = velocity;
        }
        public Vector2 GetVelocity()
        {
            return vel;
        }

        public void Move(Vector2 distance)
        {
            pos.X += (int)distance.X;
            pos.Y += (int)distance.Y;
        }
    }
}
