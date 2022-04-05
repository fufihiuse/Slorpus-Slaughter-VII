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
        // enemy fields
        private Texture2D enemyAsset;
        private int damage;
        private ShootingPattern shootingPattern;

        //bullet fields
        Action<IDestroyable> destroy;
        EnemyBullet[] wantedBullets;
        Vector2 direction;
        private float shoot;

        //properties

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

            damage = 1;

            wantedBullets = new EnemyBullet[0];
            direction = new Vector2(1, 0);
            shoot = 0;
        }

        //methods:
        //  shooting goop method, should be two shooting patterns, does damage
        //  can take damage
        //  draw enemy
        //  how to make enemy disappear when dead???

        /// <summary>
        /// Draws enemy
        /// </summary>
        /// <param name="sb"></param>
        public void DrawEnemy(SpriteBatch sb)
        {
            sb.Draw(enemyAsset, Position, Color.White);
        }

        public EnemyBullet[] FireBullets()
        {
            // return bullets that we want to fire
            EnemyBullet[] temp = wantedBullets;
            wantedBullets = new EnemyBullet[0];
            return temp;
        }

        /// <summary>
        /// Written to be called each frame and makes enemies shoot a bullet in their respective pattern.
        /// </summary>
        /// <param name="shootingPattern"></param>
        /// <param name="_player"></param>
        /// <param name="gameTime"></param>
        public void Update(ShootingPattern shootingPattern, Player _player, GameTime gameTime)
        {
            shoot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            wantedBullets = new EnemyBullet[1];         //clears array, loads in new bullet
            Vector2 toTarget =
                new Vector2(_player.Position.Location.X, _player.Position.Location.Y) - new Vector2(Position.Location.X, Position.Location.Y);  //gets distance of enemy from player

            switch (shootingPattern)
            {
                case ShootingPattern.Ensconcing:
                    if(shoot >= 0.1 && toTarget.Length() <= Constants.MIN_DETECTION_DISTANCE)   //if enough time has passed and player is in range, shoots
                    {
                        wantedBullets[0] = new EnemyBullet(
                                new Point(Position.Location.X + Constants.ENEMY_SIZE / 2, (Position.Location.Y + Constants.ENEMY_SIZE / 2) - 2),
                                UpdateBulletDirection());
                        shoot = 0;
                    }
                    break;

                case ShootingPattern.HomingAttack:
                    if(shoot >= 1 && toTarget.Length() <= Constants.MIN_FOLLOW_DISTANCE)    // diff constant, homing should have greater range
                    {
                        wantedBullets[0] = new EnemyBullet(
                            new Point((Position.Location.X + Constants.ENEMY_SIZE/2), (Position.Location.Y + Constants.ENEMY_SIZE/2)-2),
                            TargetPlayer(_player));
                        shoot = 0;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Changes direction vector to make bullets shoot in a spiral pattern and returns new direction.
        /// </summary>
        /// //
        // TODO: having a roatating origin or something could make a nice varied bullet pattern
        // hardcoded, probs not most efficient but this is what midnight brain yazz could pump out
        public Vector2 UpdateBulletDirection()
        {
            switch (direction)
            {
                case (1, 0):
                    direction = new Vector2(1, 1);
                    break;
                case (1, 1):
                    direction = new Vector2(0, 1);
                    break;
                case (0, 1):
                    direction = new Vector2(-1, 1);
                    break;
                case (-1, 1):
                    direction = new Vector2(-1, 0);
                    break;
                case (-1, 0):
                    direction = new Vector2(-1, -1);
                    break;
                case (-1, -1):
                    direction = new Vector2(0, -1);
                    break;
                case (0, -1):
                    direction = new Vector2(1, -1);
                    break;
                case (1, -1):
                    direction = new Vector2(1, 0);
                    break;
                default:
                    break;
            }
            return direction;
        }
        

        /// <summary>
        /// Tracks player position and returns updated homing bullet direction.
        /// </summary>
        /// <param name="_player"></param>
        /// <returns></returns>
        public Vector2 TargetPlayer(Player _player)
        {
            Vector2 targetDirection =
                new Vector2(_player.Position.Location.X, _player.Position.Location.Y) - new Vector2(Position.Location.X, Position.Location.Y);
            float projSpeed = 4f;

            targetDirection.Normalize();
            direction = targetDirection * projSpeed; // set to pursuit the target, set speed

            return direction;
        }

        /// <summary>
        /// Destroy this enemy.
        /// </summary>
        public void Destroy()
        {
            destroy(this);
        }

        //for each enemy in list, should check if bullet is intersecting, remove enemy from list if so
        public void EnemyDie(Rectangle projectilePos)
        {
            if (projectilePos.Intersects(this.Position))
            {
                destroy(this);
            }
        }
    }
}
