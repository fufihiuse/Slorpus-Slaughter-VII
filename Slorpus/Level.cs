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
        private Texture2D invisWallTexture;

        public List<Wall> Walls { get { return walls; } }

        //Constructor
        public Level(int tileSize, Texture2D wallTexture, Texture2D invisWallTexture)
        {
            this.tileSize = tileSize;
            walls = new List<Wall>();
            this.wallTexture = wallTexture;
            this.invisWallTexture = invisWallTexture;
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

                        //Loop through level, check if I or W, if so load into walls array
                        switch(line[column])
                        {
                        case 'W':
                            //Create new wall with proper position, and add it to walls List
                            walls.Add(
                                new Wall(
                                    new Rectangle(
                                        column * tileSize,
                                        row * tileSize,
                                        tileSize,
                                        tileSize),
                                    wallTexture));
                                break;

                        case 'I':
                            //Create new invisible wall with proper position, and add it to walls List
                            walls.Add(
                                new Wall(
                                    new Rectangle(
                                        column * tileSize,
                                        row * tileSize,
                                        tileSize,
                                        tileSize),
                                    invisWallTexture));
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

        public void Draw(SpriteBatch sb)
        {
            foreach(Wall w in walls)
            {
                w.Draw(sb);
            }
        }
    }
}
