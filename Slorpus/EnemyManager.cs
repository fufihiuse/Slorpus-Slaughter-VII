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
        List<Enemy> enemyList;
        Texture2D enemyAsset;

        BulletManager bulletManager;

        public EnemyManager(List<Enemy> enemyList, Texture2D enemyAsset, BulletManager bulletManager)
        {
            this.enemyList = enemyList;
            this.enemyAsset = enemyAsset;
            this.bulletManager = bulletManager;
        }

        /// <summary>
        /// Draws all enemies.
        /// </summary>
        /// <param name="sb">Spritebatch used to draw the enemy textures.</param>
        public void DrawEnemies(SpriteBatch sb)
        {
            Rectangle target = Rectangle.Empty;
            foreach (Enemy e in enemyList)
            {
                target.Location = e.Position.Location - Camera.Offset;
                target.Size = e.Position.Size;
                sb.Draw(enemyAsset, target, Color.Red);
            }
        }
        
        /// <summary>
        /// Updates all enemies, and adding the returned bullets to the bulletlist.
        /// </summary>
        public void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy e in enemyList)
            {
                e.Update();
                bulletManager.FireBatch(e.FireBullets());
            }
        }
    }
}
