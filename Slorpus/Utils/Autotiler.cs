using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

using Slorpus.Managers;
using Slorpus.Statics;

namespace Slorpus.Utils
{
    class Autotiler
    {
        public static Rectangle GetWallTile(uint index)
        {
            Func<int, int, Rectangle> finalize = (int row, int column) =>
            {
                return new Rectangle(
                    row*Constants.WALL_SIZE,
                    column*Constants.WALL_SIZE,
                    Constants.WALL_SIZE, Constants.WALL_SIZE);
            };

            switch (index)
            {
                case 0b000000000:
                    return finalize(0, 0);
                case 0b000000001:
                    return finalize(0, 1);
                default:
                    return finalize(0, 0);
            }
        }

        /// <summary>
        /// Produces a unique unsigned integer based on the walls adjacent to a given wall.
        /// </summary>
        /// <param name="tile_col">Column in Level.Map where the wall is found.</param>
        /// <param name="tile_row">Row in Level.Map where the wall is found.</param>
        /// <returns></returns>
        public static uint GetTileIndex(int tile_col, int tile_row)
        {
            uint final = 0;
            // index of the bit to turn on if there is a wall adjacent
            int bit = 0;
            // loop through the 3x3 area around a tile to check for walls
            for (int row = -1; row < 2; row++)
            {
                for (int col = -1; col < 2; col++)
                {
                    char adjacent;
                    try
                    {
                        adjacent = Level.Map[tile_row + row, tile_col + col];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // no wall there, try the next one
                        bit++;
                        continue;
                    }
                    // set the bit at index "bit" to 1 if there's a wall
                    if (adjacent == 'W')
                        final = final | ((uint)1 << bit);
                    // increment to the next bit
                    bit++;
                }
            }
            return final;
        }
    }
}
