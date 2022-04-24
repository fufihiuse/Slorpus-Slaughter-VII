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
            
            switch(index)
            {
                // BASIC WALLS
                case 0b000010000:
                    return finalize(3, 3);
                case 0b111111111:
                    return finalize(1, 1);
                case 0b010010010:
                    return finalize(3, 1);
                case 0b000111000:
                    return finalize(1, 3);
                
                // ENDCAPS
                case 0b000010010:
                    // downwards endcap
                    return finalize(3, 0);
                case 0b010010000:
                    // upwards endcap
                    return finalize(3, 2);
                case 0b000011000:
                    // leftwards endcap
                    return finalize(0, 3);
                case 0b000110000:
                    // rightwards endcap
                    return finalize(2, 3);

                // VERTICAL PRE-CORNERS
                case 0b110010010:
                    // corner going up and to the right
                    return finalize(3, 1);
                case 0b011010010:
                    // corner going up and to the left
                    return finalize(3, 1);
                case 0b010010110:
                    // corner going down and to the left
                    return finalize(3, 1);
                case 0b010010011:
                    // corner going down and to the right
                    return finalize(3, 1);

                // HORIZONTAL PRE-CORNERS
                case 0b001111000:
                    // corner going right and then up
                    return finalize(1, 3);
                case 0b000111001:
                    // corner going right and then down
                    return finalize(1, 3);
                case 0b100111000:
                    //corner going left and then up
                    return finalize(1, 3);
                case 0b000111100:
                    // corner going left and then down
                    return finalize(1, 3);
                
                // CUT-OFF CORNERS
                case 0b000110100:
                    // cutoff corner going left and then down
                    return finalize(2, 3);
                case 0b000011001:
                    // cutoff corner going right and then down
                    return finalize(0, 3);
                case 0b100110000:
                    // cutoff corner going left and then up
                    return finalize(2, 3);
                case 0b001011000:
                    // cutoff corner going right and then up
                    return finalize(0, 3);

                case 0b110010000:
                    // cutoff corner going up and then left
                    return finalize(3, 2);
                case 0b011010000:
                    // cutoff corner going up and then right
                    return finalize(3, 2);
                case 0b000010110:
                    // cutoff corner going down and then left
                    return finalize(3, 0);
                case 0b000010011:
                    return finalize(3, 0);

                // WALL CORNERS
                case 0b000110010:
                    // left and down corner
                    return finalize(7, 0);
                case 0b000011010:
                    // right and down corner
                    return finalize(4, 0);
                case 0b010011000:
                    // right and up corner
                    return finalize(4, 3);
                case 0b010110000:
                    // left and up corner
                    return finalize(7, 3);

                // PRE-T JUNCTIONS
                case 0b111010010:
                    // upwards T junction
                    return finalize(3, 1);
                case 0b010010111:
                    // downwards T junction
                    return finalize(3, 1);
                case 0b100111100:
                    // leftwards T junction
                    return finalize(1, 3);
                case 0b001111001:
                    // rightwards T junction
                    return finalize(1, 3);

                // T JUNCTIONS
                case 0b000111010:
                    // upwards T junction (literal T shape)
                    return finalize(8, 0);
                case 0b010111000:
                    // downwards T junctions (upside down T)
                    return finalize(8, 3);
                case 0b010110010:
                    // rightwards T junction
                    return finalize(7, 4);
                case 0b010011010:
                    // leftwards T junction
                    return finalize(4, 4);

                // CROSS
                case 0b010111010:
                    return finalize(8, 4);

                // fallback
                default:
                    return finalize(3, 3);
            }
        }

        /// <summary>
        /// Produces a unique unsigned integer based on the walls adjacent to a given wall.
        /// </summary>
        /// <param name="tile_col">Column in Level.Map where the wall is found.</param>
        /// <param name="tile_row">Row in Level.Map where the wall is found.</param>
        /// <returns></returns>
        public static uint GetTileIndex(int tile_col, int tile_row, char tile='W')
        {
            uint final = 0;
            // index of the bit to turn on if there is a wall adjacent
            int bit = 0;
            // loop through the 3x3 area around a tile to check for walls
            for (int row = 1; row > -2; row--)
            {
                for (int col = 1; col > -2; col--)
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
                    if (adjacent == tile)
                        final = final | ((uint)1 << bit);
                    // increment to the next bit
                    bit++;
                }
            }
            return final;
        }
    }
}
