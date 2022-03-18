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
        Ensconcing,         //any shooting pattern that doesn't track the enemy
        HomingAttack        //goop ball that tracks the player
    }
    public class Enemy: PhysicsObject
    {
        //fields
        private Texture2D enemyAsset;
        private int health;
        private int damage;
        private ShootingPattern shootingPattern;
        private bool isDead;

        private EnemyBullet[] wantedBullets;


        //properties
        public int Health
        {
            get { return health; }
            set
            {
                health = value;
                if(health <= 0)
                {
                    isDead = true;
                }
            }
        }

        public int Damage
        {
            get { return damage; }
        }

        public ShootingPattern ShootingPattern
        {
            get { return shootingPattern; }
        }

        /// <summary>
        /// parameterized constructor
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        /// <param name="enemyAsset"></param>
        /// <param name="shootingPattern"></param>
        public Enemy(Rectangle pos, Vector2 vel, Texture2D enemyAsset ,ShootingPattern shootingPattern)
            : base(pos, vel)
        {
            this.enemyAsset = enemyAsset;
            this.shootingPattern = shootingPattern;
            health = 1;
            damage = 1;
            isDead = false;
        }

        /// <summary>
        /// Draws enemy to screen
        /// </summary>
        /// <param name="sb"></param>
        public void DrawEnemy(SpriteBatch sb)
        {
            sb.Draw(enemyAsset, Position, Color.White);
        }

        /// <summary>
        /// when enemy DIES do this
        /// </summary>
        public void DeathAnimation()
        {
            if (isDead)
            {

            }
            //  WHEN ENEMY HEALTH 0
            //  PLAY DEATH ANIMATION
        }

        /// <summary>
        /// enemy shoots goop, depending on enemy diff goop patterns
        /// </summary>
        /// <param name="enemyGoop"></param>
        /// <returns></returns>
        public EnemyBullet[] FireBullets(ShootingPattern enemyGoop)
        {
            switch (enemyGoop)
            {
                case ShootingPattern.Ensconcing:
                    //wantedBullets = new EnemyBullet();    //TODO: fix wtf
                    //  CODE FOR SPIRAL SHOOTY
                    break;
                case ShootingPattern.HomingAttack:
                    //  CODE FOR HOMING SHOOTY
                    //  NEED PLAYER COORDS
                    break;
            }

            // return bullets that we want to fire
            return new EnemyBullet[0];
        }

        public void Update()
        {
            //checks if enemy is dead or alive
            //
            // enemy logic, called by enemy manager
        }
    }
}
