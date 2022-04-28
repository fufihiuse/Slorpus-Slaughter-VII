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

                // SOLID SHAPES ----------------------------
                case 0b000111111:
                    // top edge
                    return finalize(1, 0);
                case 0b011011011:
                    // left edge
                    return finalize(0, 1);
                case 0b110110110:
                    // right edge
                    return finalize(2, 1);
                case 0b111111000:
                    // bottom edge
                    return finalize(1, 2);

                // convex corners
                case 0b000110110:
                    // top right corner
                    return finalize(2, 0);
                case 0b000011011:
                    // top left corner
                    return finalize(0, 0);
                case 0b110110000:
                    // bottom right corner
                    return finalize(2, 2);
                case 0b011011000:
                    // bottom left corner
                    return finalize(0, 2);

                // indentation
                case 0b110111111:
                    // top right indent
                    return finalize(5, 2);
                case 0b011111111:
                    // top left indent
                    return finalize(6, 2);
                case 0b111111011:
                    // bottom left indent
                    return finalize(6, 1);
                case 0b111111110:
                    // bottom right indent
                    return finalize(5, 1);
                case 0b101111111:
                    // top indent
                    return finalize(1, 0);
                case 0b111111101:
                    // bottom indent
                    return finalize(1, 2);
                case 0b111011111:
                    // left indent
                    return finalize(0, 1);
                case 0b111110111:
                    // right indent
                    return finalize(2, 1);

                // DOUBLE INDENTATION
                case 0b001111111:
                    // top left and top indent
                    return finalize(1, 0);
                case 0b011011111:
                    // top left and left indent
                    return finalize(0, 1);
                case 0b111011011:
                    // left and bottom left indent
                    return finalize(0, 1);
                case 0b111111001:
                    // bottom left and bottom indent
                    return finalize(1, 2);
                case 0b111111100:
                    // bottom right and bottom indent
                    return finalize(1, 2);
                case 0b111110110:
                    // bottom right and right indent
                    return finalize(2, 1);
                case 0b110110111:
                    // top right and right indent
                    return finalize(2, 1);
                case 0b100111111:
                    // top and top right indent
                    return finalize(1, 0);

                // 4-indent
                case 0b110110010:
                    // bottom left corner and right side gone
                    return finalize(7, 1);
                case 0b110010110:
                    // middle left and right side gone
                    return finalize(3, 1);
                case 0b010110110:
                    // top left and right side gone
                    return finalize(7, 2);
                case 0b000011111:
                    // top side and left middle gone
                    return finalize(0, 0);
                case 0b000110111:
                    // top side and right middle gone
                    return finalize(2, 0);
                case 0b000111011:
                    // top side and bottom left gone
                    return finalize(6, 0);
                case 0b011011010:
                    // left side and bottom right gone
                    return finalize(4, 1);
                case 0b011010011:
                    // left side and right middle gone
                    return finalize(3, 1);
                case 0b010011011:
                    // left side and top right gone
                    return finalize(4, 2);
                case 0b111011000:
                    // bottom side and left middle gone
                    return finalize(0, 1);
                case 0b111110000:
                    // bottom side and right middle gone
                    return finalize(2, 2);
                case 0b011111000:
                    // bottom side and top left gone
                    return finalize(6, 3);

                // 2 opposite-corner indents
                case 0b101111110:
                    return finalize(5, 0);

                
                // REMAINING 3-ADJACENT PATTERNS
                case 0b000010111:
                    return finalize(3, 0);
                case 0b100110100:
                    return finalize(2, 3);
                case 0b111010000:
                    return finalize(3, 2);
                case 0b001011001:
                    return finalize(0, 3);
                case 0b000011101:
                    return finalize(0, 3);
                case 0b000110011:
                    return finalize(2, 0);
                case 0b100010110:
                    return finalize(3, 0);
                case 0b010110100:
                    return finalize(2, 2);
                case 0b101110000:
                    return finalize(2, 3);
                case 0b110011000:
                    return finalize(0, 2);
                case 0b011010001:
                    return finalize(3, 0);
                case 0b001011010:
                    return finalize(2, 2);
                
                // 3-indent
                case 0b011111100:
                    return finalize(6, 3);
                case 0b111110010:
                    return finalize(3, 1);
                case 0b111011100:
                    return finalize(0, 2);
                case 0b011111001:
                    return finalize(1, 2);
                case 0b101011011:
                    return finalize(0, 0);
                case 0b010011111:
                    return finalize(4, 2);
                case 0b001110111:
                    return finalize(2, 0);
                case 0b100111110:
                    return finalize(5, 0);
                case 0b110110101:
                    return finalize(2, 2);

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
                    if (tile == '0')
                    {
                        if (adjacent == tile || adjacent == 'P' || adjacent == 'H' || adjacent == 'E')
                            final = final | ((uint)1 << bit);
                    }
                    else
                    {
                        if (adjacent == tile)
                            final = final | ((uint)1 << bit);
                    }
                    // increment to the next bit
                    bit++;
                }
            }
            return final;
        }
    }
}
