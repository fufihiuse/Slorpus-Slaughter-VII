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
        private Texture2D wallAsset;
        private Texture2D mirrorAsset;
        private Texture2D invisWallAsset;

        public Point Size { get { return new Point(level.GetLength(0), level.Length); } }

        //Constructor
        public Level(List<Wall> walls, Texture2D wallAsset, Texture2D mirrorAsset, Texture2D invisWallAsset)
        {
            this.walls = walls;
            this.wallAsset = wallAsset;
            this.mirrorAsset = mirrorAsset;
            this.invisWallAsset = invisWallAsset;
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
        public void Draw(SpriteBatch sb, Point offset)
        {
            Rectangle target = Rectangle.Empty;
            foreach (Wall w in walls)
            {
                target = w.Position;
                target.Location -= offset;
                if (w.IsMirror)
                {
                    sb.Draw(wallAsset, target, Color.Green);
                }
                else if (w.IsInvis)
                {
                    sb.Draw(wallAsset, target, Color.Blue);
                }
                else
                {
                    sb.Draw(wallAsset, target, Color.White);
                }
            }
        }
    }
}
