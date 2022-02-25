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
    public class Level
    {
        //Fields
        private char[,] level;
        private List<Wall> walls;
        private int tileSize;
        private Texture2D wallTexture;

        //Properties
        public List<Wall> WallList
        {
            get { return walls; }
        }

        //Constructor
        public Level(int tileSize, Texture2D wallTexture)
        {
            this.tileSize = tileSize;
            walls = new List<Wall>();
            this.wallTexture = wallTexture;
        }

        //Methods
        public void LoadFromFile(string filepath)
        {
            string line;
            string[] data;

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

                        if (line[column] == 'W')
                        {
                            walls.Add(
                                new Wall(
                                    new Rectangle(
                                        column * tileSize,
                                        row * tileSize,
                                        tileSize,
                                        tileSize)));
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

        public void Draw(SpriteBatch sb)
        {
            foreach(Wall w in walls)
            {
                sb.Draw(wallTexture, w.Position, Color.White);
            }
        }
    }
}
