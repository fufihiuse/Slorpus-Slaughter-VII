using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * Stores a reference to the bulletList and collects the "wanted bullets"
     * that enemies request, and then adds them to the bullet list.
     * Also provides bullet and enemy update calls
     */
    class EnemyManager
    {
        List<EnemyBullet> bulletList;
        List<Enemy> enemyList;
        Texture2D bulletAsset;
        Texture2D enemyAsset;

        public EnemyManager(List<EnemyBullet> bulletList, List<Enemy> enemyList, Texture2D enemyAsset, Texture2D bulletAsset)
        {
            this.bulletList = bulletList;
            this.enemyList = enemyList;
            this.enemyAsset = enemyAsset;
            this.bulletAsset = bulletAsset;
        }

        /// <summary>
        /// Draws all enemies.
        /// </summary>
        /// <param name="sb">Spritebatch used to draw the enemy textures.</param>
        public void DrawEnemies(SpriteBatch sb)
        {
            foreach (Enemy e in enemyList)
            {
                sb.Draw(enemyAsset, e.Position, Color.White);
            }
        }
        /// <summary>
        /// Draws all bullets.
        /// </summary>
        /// <param name="sb">Spritebatch used to draw the bullet textures.</param>
        public void DrawBullets(SpriteBatch sb)
        {
            Rectangle drawRect = new Rectangle(new Point(), new Point(bulletAsset.Width, bulletAsset.Height));
            foreach (EnemyBullet eb in bulletList)
            {
                drawRect.X = eb.Position.X;
                drawRect.Y = eb.Position.Y;
                sb.Draw(enemyAsset, drawRect, Color.White);
            }
        }
        
        /// <summary>
        /// Updates all enemies, and adding the returned bullets to the bulletlist.
        /// </summary>
        public void UpdateEnemies()
        {
            EnemyBullet[] new_bullets;
            // call update and then accept the returned bullet
            foreach (Enemy e in enemyList)
            {
                e.Update();
                new_bullets = e.FireBullets();
                // "fire" the bullets
                bulletList.AddRange(new_bullets);
            }
        }
    }
}
