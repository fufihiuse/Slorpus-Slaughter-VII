using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Slorpus.Utils;
using Slorpus.Objects;
using Slorpus.Statics;
using Slorpus.Interfaces.Base;

namespace Slorpus.Managers
{
    //Jackson Majewski
    class Level: IDraw
    {
        //Fields
        private static char[,] level;
        private List<Wall> floors;
        private List<Wall> walls;
        private Texture2D wallAsset;
        private Texture2D mirrorAsset;
        private Texture2D invisWallAsset;
        private Texture2D gridAsset;
        private Texture2D wallTileset;
        private Texture2D floorTileset;

        public static Point Size { get { return new Point(level.Length/level.GetLength(0), level.GetLength(0)); } }
        public static char[,] Map { get { return level; } }

        //Constructor
        public Level(List<Wall> walls, List<Wall> floors, ContentManager content)
        {
            this.walls = walls;
            this.floors = floors;
            wallAsset = Game1.SquareTexture; // content.Load<Texture2D>("square");
            mirrorAsset = Game1.SquareTexture; //  content.Load<Texture2D>("square");
            invisWallAsset = Game1.SquareTexture; // content.Load<Texture2D>("square");
            wallTileset = content.Load<Texture2D>("tile/floor-tilemap");
            floorTileset = content.Load<Texture2D>("tile/wall-tilemap");
            gridAsset = content.Load<Texture2D>("grid");
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

                        GenericEntity e = new GenericEntity(
                                line[column],
                                column * Constants.WALL_SIZE,
                                row * Constants.WALL_SIZE,
                                column, row);

                        entityList.Add(e);
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
                    sb.Draw(wallAsset, target, Color.White);
                }
                else if (!w.IsBulletCollider)
                {
                    sb.Draw(wallAsset, target, Color.Yellow);
                }
                else
                {
                    sb.Draw(wallTileset, target, w.SubTex, Color.White);
                }
            }
            
            // draw floors
            foreach (Wall f in floors)
            {
                target = f.Position;
                target.Location -= Camera.Offset;
                sb.Draw(floorTileset, target, f.SubTex, Color.White);
            }

            //TODO: TOGGLE FALSE BEFORE BUILD
            /*
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
            */
        }
    }
}
