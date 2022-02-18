using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public class Player
    {
        // Fields
        Texture2D playerAsset;
        Rectangle rectangle;

        // Constructor
        public Player(Texture2D playerAsset, int x, int y, int width, int height)
        {
            this.playerAsset = playerAsset;
            rectangle = new Rectangle(x, y, width, height);
        }
    }
}
