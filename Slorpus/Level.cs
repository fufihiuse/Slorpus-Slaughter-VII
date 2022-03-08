using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Slorpus
{
    //Jackson Majewski
    class Level
    {
        //Fields
        private char[,] level;
        private List<Wall> walls;
        private int tileSize;
        private Texture2D wallAsset;
        private Texture2D mirrorAsset;
        private Texture2D invisWallAsset;

        //Properties
        public List<Wall> WallList
        {
            get { return walls; }
        }

        //Constructor

        /* OLD
        public Level(int tileSize, PhysicsManager physicsManager, Texture2D wallAsset, Texture2D playerAsset,
            Texture2D playerBulletAsset, Texture2D eEnemyAsset, Texture2D hEnemyAsset)
        {
            this.tileSize = tileSize;
            walls = new List<Wall>();
            this.wallAsset = wallAsset;
            this.playerAsset = playerAsset;
            this.eEnemyAsset = eEnemyAsset;
            this.hEnemyAsset = hEnemyAsset;
            this.playerBulletAsset = playerBulletAsset;
            this.physicsManager = physicsManager;
        }
        */

        public Level(int tileSize, Texture2D wallAsset, Texture2D mirrorAsset, Texture2D invisWallAsset)
        {
            this.tileSize = tileSize;
            walls = new List<Wall>();
            this.wallAsset = wallAsset;
            this.mirrorAsset = mirrorAsset;
            this.invisWallAsset = invisWallAsset;
        }

        //Methods

        /* OLD
        public void LoadFromFile(string filepath,  out Player player, out List<Enemy> enemyList)
        {
            string line;
            string[] data;
            player = null;
            enemyList = new List<Enemy>();

            //Loading from file
            try
            {
                //Attempt to load file
                StreamReader input = new StreamReader(filepath);

                //Get number of rows and columns
                line = input.ReadLine();
                data = line.Split(',');
                level = new char[int.Parse(data[0]), int.Parse(data[1])];

                //Load 2D array into level
                line = input.ReadLine();
                for (int row = 0; row < level.GetLength(0); row++)
                {
                    for (int column = 0; column < level.GetLength(1); column++)
                    {
                        level[row, column] = line[column];

                        switch(line[column])
                        {
                            case 'W':
                                walls.Add(
                                    new Wall(
                                        new Rectangle(
                                            column * tileSize,
                                            row * tileSize,
                                            tileSize,
                                            tileSize)));
                                break;

                            case 'E':
                                enemyList.Add(
                                    new Enemy(
                                        new Rectangle(
                                            column * tileSize,
                                            row * tileSize,
                                            tileSize,
                                            tileSize),
                                        Vector2.Zero,
                                        eEnemyAsset,
                                        ShootingPattern.Ensconcing));
                                break;

                            case 'H':
                                enemyList.Add(
                                    new Enemy(
                                        new Rectangle(
                                            column * tileSize,
                                            row * tileSize,
                                            tileSize,
                                            tileSize),
                                        Vector2.Zero,
                                        hEnemyAsset,
                                        ShootingPattern.HomingAttack));
                                break;

                            case 'M':
                                walls.Add(
                                    new Wall(
                                        new Rectangle(
                                            column * tileSize,
                                            row * tileSize,
                                            tileSize,
                                            tileSize), 
                                        true, //is Collidable
                                        true)); //is a mirror
                                break;

                            case 'P':
                                player = new Player(new Rectangle(
                                            column * tileSize,
                                            row * tileSize,
                                            tileSize,
                                            tileSize),
                                            Vector2.Zero,
                                            physicsManager,
                                            playerAsset,
                                            playerBulletAsset
                                            );
                                break;
                        }

                    }
                    line = input.ReadLine();
                }

                input.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }            

        }
        */

        public List<GenericEntity> LoadFromFile(string filepath, List<GenericEntity> entityList)
        {
            string line;
            string[] data;
            entityList = new List<GenericEntity>();

            //Loading from file
            try
            {
                //Attempt to load file
                StreamReader input = new StreamReader(filepath);

                //Get number of rows and columns
                line = input.ReadLine();
                data = line.Split(',');
                level = new char[int.Parse(data[0]), int.Parse(data[1])];

                //Load 2D array into level
                line = input.ReadLine();
                for (int row = 0; row < level.GetLength(0); row++)
                {
                    for (int column = 0; column < level.GetLength(1); column++)
                    {
                        level[row, column] = line[column];

                        switch (line[column])
                        {
                            case 'W':
                                walls.Add(
                                    new Wall(
                                        new Rectangle(
                                            column * tileSize,
                                            row * tileSize,
                                            tileSize,
                                            tileSize)));
                                break;

                            //Handle different entity types
                            case 'E':
                            case 'H':
                            case 'P':
                                entityList.Add(
                                    new GenericEntity(
                                        line[column],
                                        column * tileSize,
                                        row * tileSize));
                                break;

                            case 'M':
                                walls.Add(
                                    new Wall(
                                        new Rectangle(
                                            column * tileSize,
                                            row * tileSize,
                                            tileSize,
                                            tileSize),
                                        true, //is Collidable
                                        true)); //is a mirror
                                break;
                        }

                    }
                    line = input.ReadLine();

                }

                input.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return entityList;

        }

        /// <summary>
        /// Override method to draw the walls
        /// </summary>
        /// <param name="sb"></param>
        public void Draw(SpriteBatch sb)
        {
            foreach (Wall w in walls)
            {
                if (w.IsMirror)
                {
                    sb.Draw(wallAsset, w.Position, Color.Green);
                }
                else if (w.IsInvis)
                {
                    sb.Draw(wallAsset, w.Position, Color.Blue);
                }
                else
                {
                    sb.Draw(wallAsset, w.Position, Color.White);
                }
            }
        }
    }
}
