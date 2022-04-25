using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Slorpus.Objects;
using Slorpus.Statics;
using Slorpus.Interfaces.Base;

namespace Slorpus.Managers
{
    /* Class created out of necessity, due to all the operations that
     * needed to be done to the bullets array. This class has functions
     * which wrap around things like selecting a certain bullet from
     * the array and moving it.
     */
    class BulletManager: IDraw, IDestroyable
    {
        private EnemyBullet[] bullets;
        private Texture2D bulletAsset;
        // list of bullets queued for removal
        private List<int> queuedBullets;

        private static BulletManager current;

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
            queuedBullets = new List<int>();

            current = this;
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
                drawRect.Location = bullets[i].Position - Camera.Offset;
                sb.Draw(bulletAsset, drawRect, Color.White);
            }
        }

        /// <summary>
        /// Draws all bullets.
        /// </summary>
        /// <param name="sb">Spritebatch used to draw the bullet textures.</param>
        public void Draw(SpriteBatch sb)
        {
            //DrawBullets(sb, new Point(bulletAsset.Width, bulletAsset.Height));
            DrawBullets(sb, new Point(Constants.BULLET_SIZE, Constants.BULLET_SIZE));
        }
        
        /// <summary>
        /// Expensive way of adding a bullet to the screen. Use FireBatch instead.
        /// </summary>
        /// <param name="bullet">Bullet to add into the bulletManager's list.</param>
        public void Fire(EnemyBullet bullet)
        {
            EnemyBullet[] original = bullets;
            bullets = new EnemyBullet[bullets.Length + 1];
            original.CopyTo(bullets, 0);
            bullets[original.Length] = bullet;
        }
        
        /// <summary>
        /// The most efficient way of adding bullets to the screen.
        /// </summary>
        /// <param name="new_bullets">Array of bullets to append to the current array.</param>
        public void FireBatch(EnemyBullet[] new_bullets)
        {
            EnemyBullet[] original = bullets;
            bullets = new EnemyBullet[bullets.Length + new_bullets.Length];
            original.CopyTo(bullets, 0);
            new_bullets.CopyTo(bullets, original.Length);
            
            //if (new_bullets.Length > 0)
            //    SoundEffects.PlayEffectVolume("enemy_bullet", 0.1f);
        }
        
        /// <summary>
        /// Static version of the BulletManager's FireBatch method.
        /// </summary>
        /// <param name="bullets">Bullets to add to the game.</param>
        public static void FireBullets(EnemyBullet[] bullets)
        {
            current.FireBatch(bullets);
        }

        public void Destroy(int bulletIndex, bool clean=true)
        {
            queuedBullets.Add(bulletIndex);
            if (clean)
            {
                Clean();
            }
        }

        public void DestroyBatch(int[] bulletIndices, bool clean=true)
        {
            queuedBullets.AddRange(bulletIndices);
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
            EnemyBullet[] narray = new EnemyBullet[bullets.Length - queuedBullets.Count];
            int counter = 0;
            // removed all bullets at indices in queuedBullets
            for (int i = 0; i < bullets.Length; i++)
            {
                if (queuedBullets.Contains(i))
                {
                    continue;
                }
                try
                {
                    narray[counter] = bullets[i];
                }
                catch (IndexOutOfRangeException)
                {
                    // probably a result of duplicate items in the queued bullets list. just skip
                    continue;
                }
                // we will only get to this point if a.) the bullet "i" is not queued to be destroyed
                // and b.) we successfully added it to the new bullets array.
                counter++;
            }
            // reset queuedbullets
            queuedBullets.Clear();
            bullets = narray;
        }

        public void Destroy()
        {
            if (current == this)
            {
                current = null;
            }
            Dereferencer.Destroy(this);
        }
    }
}
