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
        /// Updates all enemies, and adding the returned bullets to the bulletlist.
        /// </summary>
        public void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy e in enemyList)
            {
                e.Update();
                bulletManager.FireBatch(e.FireBullets(e.ShootingPattern));
            }
        }
    }
}
