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
    class Enemy: PhysicsObject, IDestroyable
    {
        //fields
        private Texture2D enemyAsset;
        private int health;
        private int damage;
        private ShootingPattern shootingPattern;
        private bool isDead;
        Action<IDestroyable> destroy;

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
        /// <param name="destroyFunc">Action delegate that destroys this enemy.</param>
        public Enemy(Rectangle pos, Vector2 vel,
            Texture2D enemyAsset, ShootingPattern shootingPattern,
            Action<IDestroyable> destroy)
            : base(pos, vel)
        {
            this.enemyAsset = enemyAsset;
            this.shootingPattern = shootingPattern;
            this.destroy = destroy;
            health = 1;
            damage = 1;
            isDead = false;
        }

        //methods:
        //  shooting goop method, should be two shooting patterns, does damage
        //  can take damage
        //  draw enemy
        //  how to make enemy disappear when dead???

        /// <summary>
        /// draws enemy
        /// </summary>
        /// <param name="sb"></param>
        public void DrawEnemy(SpriteBatch sb)
        {
            sb.Draw(enemyAsset, Position, Color.White);
        }

        /// <summary>
        /// enemy shoots goop, depending on enemy diff goop patterns
        /// </summary>
        /// <param name="enemyGoop"></param>
        public void ShootGoop(ShootingPattern enemyGoop)
        {
            switch (enemyGoop)
            {
                case ShootingPattern.Ensconcing:
                    wantedBullets = new EnemyBullet[1];
                    wantedBullets[0].Position = new Point(Position.X, Position.Y);
                    
                    //  bullet is shot from enemy position, velocity is added to pos x and y
                    break;
                case ShootingPattern.HomingAttack:
                    wantedBullets = new EnemyBullet[1];

                    break;
            }
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

        public EnemyBullet[] FireBullets()
        {
            // return bullets that we want to fire
            return new EnemyBullet[0];
        }

        public void Update()
        {
            //checks if enemy is dead or alive
            FireBullets(shootingPattern);
            // enemy logic, called by enemy manager

            FireBullets();
        }
        
        /// <summary>
        /// Destroy this enemy.
        /// </summary>
        public void Destroy()
        {
            destroy(this);
        }
    }
}
