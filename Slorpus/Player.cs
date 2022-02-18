using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public class Player : PhysicsObject
    {
        // Fields
        Texture2D playerAsset;
        Rectangle rectangle;
        int x;
        int y;

        // Constructor
        public Player(Point pos, Vector2 vel, int width, int height): base(pos, vel, width, height)
        {
            rectangle = new Rectangle(x, y, width, height);
        }
    }
}
