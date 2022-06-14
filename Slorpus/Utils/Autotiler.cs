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
        /// <summary>
        /// Broken an unused. Meant to intelligently find the best matching
        /// tile for autotiling, without brute forcing every possible combination.
        /// </summary>
        /// <param name="index">Binary value representing the arrangement of tiles.</param>
        /// <returns>Sub-rectangle of a blob tileset.</returns>
        public static Rectangle GetWallTile(uint index)
        {
            Func<int, int, Rectangle> finalize = (int row, int column) =>
            {
                return new Rectangle(
                    row*Constants.WALL_SIZE,
                    column*Constants.WALL_SIZE,
                    Constants.WALL_SIZE, Constants.WALL_SIZE);
            };
            
            // dictionary of a bunch of possible orientations of walls
            Dictionary<uint, Point> possibleStates = new Dictionary<uint, Point>();
            
            // possible states
            // 0 = no tile
            // 1 = tile
            // 2 = doesnt matter
            possibleStates.Add(222222222, new Point(3, 3)); // default
            possibleStates.Add(202010202, new Point(3, 3));
            possibleStates.Add(212110202, new Point(0, 0));
            possibleStates.Add(212010202, new Point(1, 0));
            possibleStates.Add(202110202, new Point(0, 1));
            possibleStates.Add(212111212, new Point(1, 1));
            possibleStates.Add(212011202, new Point(2, 0));
            possibleStates.Add(202110212, new Point(0, 2));
            possibleStates.Add(202011202, new Point(2, 1));
            possibleStates.Add(202010212, new Point(1, 2));
            possibleStates.Add(202011212, new Point(2, 2));
            
            // go through all the possible states and find the one that best matches
            int bestScore = 0;
            uint bestMatch = 222222222;
            foreach (uint possibleState in possibleStates.Keys)
            {
                // score this number based on how similar it is to the actual state of the tile
                int score = 0;
                for (int i = 0; i < 9; i++)
                {
                    uint checkDigit = (possibleState / (uint)Math.Pow(10, i)) % 10;
                    // if (checkDigit == 2) { continue; } // skip tiles that dont matter for this potential state

                    // get the i'th digit of the index (its in binary though)
                    uint actualDigit = ((index & ((uint)1 << i)) != 0) ? (uint)1 : 0;

                    //if (i == 4 && actualDigit == 0) { return finalize(8, 4); }
                    
                    // readable code for once
                    if (checkDigit == actualDigit)
                    {
                        score++;
                    }
                    else
                    {
                        score--;
                    }

                    // also stop if we found the best possible one
                    if (score == 9) { break; }
                }

                // update the best found state so far
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = possibleState;
                }
            }

            Point subRectCoords = possibleStates[bestMatch];
            return finalize(subRectCoords.X, subRectCoords.Y);
        }

        /// <summary>
        /// Brute forces every possible combination/arrangement of tiles.
        /// </summary>
        /// <param name="index">Binary value representing the arrangement of tiles.</param>
        /// <returns>Sub-rectangle texture of a blob tileset.</returns>
        public static Rectangle GetWallTileSimple(uint index)
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
                
                // 2 opposite-corner indents
                case 0b101111110:
                    return finalize(5, 0);

                case 0b010111111:
                    return finalize(8, 2);

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
                case 0b100110110:
                    // top and right side gone
                    return finalize(2, 0);
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
                case 0b011011001:
                    // left side and bottom middle gone
                    return finalize(0, 2);
                case 0b011010011:
                    // left side and right middle gone
                    return finalize(3, 1);
                case 0b010011011:
                    // left side and top right gone
                    return finalize(4, 2);
                case 0b111011000:
                    // bottom side and left middle gone
                    return finalize(0, 2);
                case 0b111110000:
                    // bottom side and right middle gone
                    return finalize(2, 2);
                case 0b011111000:
                    // bottom side and top left gone
                    return finalize(6, 3);
                case 0b000111110:
                    // top side and bottom right gone
                    return finalize(5, 0);

                // 4 indent, no complete sides gone
                // parallel cases are actually covered by the T junctions
                case 0b001110110:
                    // top left side and right bottom side gone
                    return finalize(2, 0);
                case 0b001111100:
                    // top left side and bottom right side gone
                    return finalize(1, 3);
                case 0b100011011:
                    // top right side and left bottom side gone
                    return finalize(0, 0);
                case 0b100111001:
                    // top right side and bottom left side gone
                    return finalize(1, 3);
                case 0b011010110:
                    // left top side and right bottom side gone
                    return finalize(3, 1);
                case 0b011011100:
                    // left top side and bottom right side gone
                    return finalize(0, 2);
                case 0b110010011:
                    // right top side and left bottom side gone
                    return finalize(3, 1);
                case 0b110110001:
                    // right top side and bottom left side gone
                    return finalize(2, 2);

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
                    return finalize(7, 0);
                case 0b100010110:
                    return finalize(3, 0);
                case 0b010110100:
                    return finalize(2, 2);
                case 0b101110000:
                    return finalize(2, 3);
                case 0b110011000:
                    return finalize(4, 3);
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
                
                // corners, no extensions
                // like diagonals if you turned off "pixel perfect" mode in aseprite
                case 0b100110111:
                    // top right corner
                    return finalize(2, 0);
                case 0b111110100:
                    // bottom right corner
                    return finalize(2, 2);
                case 0b111011001:
                    // bottom left corner
                    return finalize(0, 2);
                case 0b001011111:
                    // top left corner
                    return finalize(0, 0);

                // stuff in the 7 on the last level
                // two adjacent empty tiles, both diagonal
                case 0b101011111:
                    return finalize(0, 0);
                case 0b111110101:
                    return finalize(2, 2);
                case 0b101110111:
                    return finalize(2, 0);
                case 0b111011101:
                    return finalize(0, 2);

                case 0b110010111:
                    return finalize(3, 1);
                case 0b001111011:
                    return finalize(6, 0);
                case 0b111111010:
                    return finalize(8, 1);
                case 0b111010110:
                    return finalize(3, 1);
                case 0b001010011:
                    return finalize(3, 0);
                case 0b110010100:
                    return finalize(3, 2);
                case 0b010010001:
                    return finalize(3, 2);
                case 0b001010010:
                    return finalize(3, 0);

                // stuff from yellow spiral level
                case 0b110110100:
                    return finalize(2, 2);
                case 0b011011101:
                    return finalize(0, 2);
                case 0b011110000:
                    return finalize(7, 3);
                case 0b001011110:
                    return finalize(4, 0);
                case 0b011110100:
                    return finalize(7, 3);
                case 0b011011110:
                    return finalize(4, 1);
                case 0b110011001:
                    return finalize(4, 3);
                case 0b100111011:
                    return finalize(6, 0);
                case 0b110111011:
                    return finalize(9, 0);
                
                // the end
                case 0b011110111:
                    return finalize(7, 2);
                case 0b111011110:
                    return finalize(4, 1);
                case 0b000110101:
                    return finalize(2, 3);

                // penis level
                case 0b000011100:
                    return finalize(0, 3);
                case 0b101110110:
                    return finalize(2, 0);
                case 0b100010010:
                    return finalize(3, 0);
                case 0b100011001:
                    return finalize(0, 3);
                case 0b000110001:
                    return finalize(2, 3);
                case 0b100011000:
                    return finalize(0, 3);
                case 0b110010001:
                    return finalize(3, 2);
                case 0b100110010:
                    return finalize(7, 0);

                // the hard level
                case 0b001011011:
                    return finalize(0, 0);


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
                        final = final | ((uint)1 << bit); // comment this out if outside tiles should look empty
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
