using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public class Enemy: PhysicsObject
    {
        public Enemy(Rectangle pos, Vector2 vel): base(pos, vel)
        {
            // placeholder
        }

        public EnemyBullet[] FireBullets()
        {
            // return bullets that we want to fire
            return new EnemyBullet[0];
        }

        public void Update()
        {
            // enemy logic, called by enemy manager
        }
    }
}
