using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public enum ShootingPattern
    {
        Ensconcing,         //any shooting pattern that doesn't track the enemy
        HomingAttack        //goop ball that tracks the player
    }
    class Enemy: PhysicsObject, IUpdate, IDraw, IDestroyable, ILoad
    {
        // counter for how many enemies exist
        private static int count = 0;
        public static int Count { get { return count; } }

        // enemy fields
        private Texture2D enemyAsset;
        private int damage;
        private ShootingPattern shootingPattern;

        //bullet fields
        EnemyBullet[] wantedBullets;
        float direction;
        private float shoot;
        
        // time in seconds betweens shots for an ensconsing enemy
        private const float escShotInterval = 0.1f;
        // same for homing
        private const float homeShotInterval = 1.0f;

        private const float escBulletSpeed = 1.0f;
        // NOT IMPLEMENTED
        private const float homeBulletSpeed = 1.0f;

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
        /// Enemy class of variable shooting pattern.
        /// </summary>
        /// <param name="pos">Starting position of the enemy.</param>
        /// <param name="content">ContentManager for loading the enemy textures.</param>
        /// <param name="shootingPattern">The logic the enemy uses to shoot bullets.</param>
        /// <param name="destroy">Function that queues this object for dereferencing.</param>
        public Enemy(
            Rectangle pos,
            ContentManager content,
            ShootingPattern shootingPattern)
            // default to 0,0 velocity
            : base(pos, new Vector2(0,0))
        {
            this.shootingPattern = shootingPattern;

            damage = 1;

            wantedBullets = new EnemyBullet[0];
            direction = 0;
            shoot = 0;
            
            // increase the number of enemies that exist
            count++;

            LoadContent(content);
        }

        public void LoadContent(ContentManager content)
        {
            // add stuff for different textures per ShootingPattern
            enemyAsset = Game1.SquareTexture; // content.Load<Texture2D>("square");
        }

        /// <summary>
        /// Draws enemy
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            sb.Draw(enemyAsset, target, Color.Red);
        }

        /// <summary>
        /// Written to be called each frame and makes enemies shoot a bullet in their respective pattern.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            BulletManager.FireBullets(wantedBullets);
            wantedBullets = new EnemyBullet[0];


            shoot += (float)gameTime.ElapsedGameTime.TotalSeconds;
            wantedBullets = new EnemyBullet[1];         //clears array, loads in new bullet
            Vector2 toTarget =
                new Vector2(Player.Position.Location.X, Player.Position.Location.Y) - new Vector2(Position.Location.X, Position.Location.Y);  //gets distance of enemy from player

            switch (shootingPattern)
            {
                case ShootingPattern.Ensconcing:
                    if(shoot >= escShotInterval && toTarget.Length() <= Constants.MIN_DETECTION_DISTANCE)   //if enough time has passed and player is in range, shoots
                    {
                        wantedBullets[0] = new EnemyBullet(
                                new Point(Position.Location.X + Constants.ENEMY_SIZE / 2, (Position.Location.Y + Constants.ENEMY_SIZE / 2) - 2),
                                UpdateBulletDirection());
                        shoot = 0;
                    }
                    break;

                case ShootingPattern.HomingAttack:
                    if(shoot >= homeShotInterval && toTarget.Length() <= Constants.MIN_FOLLOW_DISTANCE)    // diff constant, homing should have greater range
                    {
                        wantedBullets[0] = new EnemyBullet(
                            new Point((Position.Location.X + Constants.ENEMY_SIZE/2), (Position.Location.Y + Constants.ENEMY_SIZE/2)-2),
                            TargetPlayer());
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
        // TODO: having a roatating origin or something could make a nice varied bullet pattern
        // hardcoded, probs not most efficient but this is what midnight brain yazz could pump out
        public Vector2 UpdateBulletDirection()
        {
            direction += MathHelper.ToRadians(22.5f);
            direction = MathHelper.WrapAngle(direction);

            return new Vector2(
                MathF.Cos(direction) * escBulletSpeed,
                MathF.Sin(direction) * escBulletSpeed
                );
        }

        /// <summary>
        /// Tracks player position and returns updated homing bullet direction.
        /// </summary>
        /// <param name="_player"></param>
        /// <returns></returns>
        public Vector2 TargetPlayer()
        {
            Vector2 targetDirection =
                new Vector2(Player.Position.Location.X, Player.Position.Location.Y) - new Vector2(Position.Location.X, Position.Location.Y);

            targetDirection.Normalize();

            return targetDirection * homeBulletSpeed;
        }

        /// <summary>
        /// Destroy this enemy.
        /// </summary>
        public void Destroy()
        {
            count--;
            Dereferencer.Destroy(this);
        }
    }
}
