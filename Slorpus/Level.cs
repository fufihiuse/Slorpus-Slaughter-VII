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
        private static char[,] level;
        private List<Wall> walls;
        private Texture2D wallAsset;
        private Texture2D mirrorAsset;
        private Texture2D invisWallAsset;
        private Texture2D gridAsset;

        public static Point Size { get { return new Point(level.Length/level.GetLength(0), level.GetLength(0)); } }

        //Constructor
        public Level(List<Wall> walls, Texture2D wallAsset, Texture2D mirrorAsset, Texture2D invisWallAsset, Texture2D gridAsset)
        {
            this.walls = walls;
            this.wallAsset = wallAsset;
            this.mirrorAsset = mirrorAsset;
            this.invisWallAsset = invisWallAsset;
            this.gridAsset = gridAsset;
        }

        //Methods 

        public List<GenericEntity> LoadFromFile(string filepath)
        {
            string line;
            string[] data;
            List<GenericEntity> entityList = new List<GenericEntity>();

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
                        
                        entityList.Add(
                            new GenericEntity(
                                line[column],
                                column * Constants.WALL_SIZE,
                                row * Constants.WALL_SIZE));
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
            Rectangle target = Rectangle.Empty;
            foreach (Wall w in walls)
            {
                target = w.Position;
                target.Location -= Camera.Offset;
                if (w.IsMirror)
                {
                    sb.Draw(wallAsset, target, Color.Green);
                }
                else if (w.IsBulletCollider)
                {
                    sb.Draw(wallAsset, target, Color.White);
                }
                else
                {
                    sb.Draw(wallAsset, target, Color.Blue);
                }
            }

            //TODO: TOGGLE FALSE BEFORE BUILD
            if (true)
            {
                for (int i = 0; i < level.GetLength(0); i++)
                {
                    for (int j = 0; j < level.GetLength(1); j++)
                    {
                        sb.Draw(gridAsset, new Rectangle(j * Constants.WALL_SIZE, i * Constants.WALL_SIZE, Constants.WALL_SIZE, Constants.WALL_SIZE), Color.White);
                    }
                }
            }
        }
    }
}
