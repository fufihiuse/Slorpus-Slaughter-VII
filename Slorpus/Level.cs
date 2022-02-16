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
        private Wall[,] walls;

        //Constructor
        public Level()
        {

        }

        //Methods
        public void LoadFromFile(string filepath)
        {
            string line;
            string[] data;
            /*
             * Load file into level array
             * TODO: Loop through level, check if I or W, if so load into walls array
             * 
             */

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
                        /*
                        switch (line[column])
                        {
                            case 'W':
                                walls[row, column] = new Wall();
                                break;
                            case 'I':
                                walls[row, column] = new Wall();
                                break;
                            default:
                                walls[row, column] = null;
                        }
                        *May not work due to walls array not accepting nulls (Wall is a struct)
                        */
                    }
                    line = input.ReadLine();
                }

                input.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }

            //Loop through level, check if I or W, if so load into walls array
            

        }

        public void Draw()
        {

        }
    }
}
