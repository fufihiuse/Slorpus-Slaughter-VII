using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /* Class created out of necessity, due to all the operations that
     * needed to be done to the bullets array. This class has functions
     * which wrap around things like selecting a certain bullet from
     * the array and moving it.
     */
    class BulletManager
    {
        private EnemyBullet[] bullets;
        private Texture2D bulletAsset;

        // get a reference to a given bullet
        public ref EnemyBullet this[int i]
        {
            get { return ref bullets[i]; }
        }

        public int Length { get { return bullets.Length; } }
 
        // read-only property, looped over in PhysicsManager
        public EnemyBullet[] Bullets { get { return bullets; } }

        public BulletManager(EnemyBullet[] bullets, Texture2D bulletAsset)
        {
            this.bullets = bullets;
            this.bulletAsset = bulletAsset;
            
            Random gen = new Random();
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i] = new EnemyBullet(new Point(gen.Next(Constants.WALL_SIZE, 600), gen.Next(Constants.WALL_SIZE, 600)), new Vector2(1f, 2f));
            }
        }

        public void Move(int index, Point motion)
        {
            bullets[index].Move(motion);
        }

        /// <summary>
        /// Draws all bullets with a specific size.
        /// </summary>
        /// <param name="sb">Spritebatch used to draw the bullet textures.</param>
        public void DrawBullets(SpriteBatch sb, Point size)
        {
            Rectangle drawRect = new Rectangle(new Point(), size);
            for (int i = 0; i < bullets.Length; i ++)
            {
                drawRect.Location = bullets[i].Position;
                sb.Draw(bulletAsset, drawRect, Color.White);
            }
        }
        /// <summary>
        /// Draws all bullets.
        /// </summary>
        /// <param name="sb">Spritebatch used to draw the bullet textures.</param>
        public void DrawBullets(SpriteBatch sb)
        {
            DrawBullets(sb, new Point(bulletAsset.Width, bulletAsset.Height));
        }

        public void Fire(EnemyBullet bullet)
        {
            //
        }

        public void FireBatch(EnemyBullet[] bullets)
        {
            //
        }

        public void Destroy(int bulletIndex)
        {
            //
        }

        public void DestroyBatch(int[] bulletIndices, bool clean=false)
        {
            //
            if (clean)
            {
                Clean();
            }
        }
        
        /// <summary>
        /// Reallocates the bullet array, potentially making any stored bullet indices invalid.
        /// </summary>
        public void Clean()
        {
            //
        }
    }
}
