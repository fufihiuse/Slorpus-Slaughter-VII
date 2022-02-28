using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public enum ShootingPattern
    {
        Spiral,
        HomingAttack
    }
    public class Enemy: PhysicsObject
    {
        private Texture2D enemyAsset;
        private int health;
        private int damage;
        private ShootingPattern shootingPattern;
        //private bool isDead;

        //properties
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public int Damage
        {
            get { return damage; }
        }

        public ShootingPattern ShootingPattern
        {
            get { return shootingPattern; }
        }

        public Enemy(Rectangle pos, Vector2 vel, Texture2D enemyAsset, ShootingPattern shootingPattern) : base(pos, vel)
        {
            this.shootingPattern = shootingPattern;
            this.enemyAsset = enemyAsset;
            health = 1;
            damage = 1;
        }

        //methods:
        //  shooting goop method, should be two shooting patterns, should do damage
        //  can take damage
        //  draw enemy
        //  how to make enemy disappear when dead???

        public void DrawEnemy(SpriteBatch sb)
        {
            sb.Draw(enemyAsset, Position, Color.White);
        }

        public void ShootGoop(ShootingPattern enemyGoop)
        {
            switch (enemyGoop)
            {
                case ShootingPattern.Spiral:
                    //CODE FOR SPIRAL SHOOTY
                    break;
                case ShootingPattern.HomingAttack:
                    //CODE FOR HOMING SHOOTY
                    //NEED PLAYER COORDS
                    break;
            }
        }
    }
}
