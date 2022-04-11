using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * Interface for special objects not already drawn by their own manager classes.
     */
    interface IDraw
    {
        public int Layer { get; }
        public void Draw(SpriteBatch spriteBatch);
    }

    /// classes' implementation of layer
    
    partial class PlayerProjectile
    {
        private int layer = 0;
        public int Layer { get { return layer; } }
    }
    partial class Enemy
    {
        private int layer = 0;
        public int Layer { get { return layer; } }
    }
    partial class Player
    {
        private int layer = 0;
        public int Layer { get { return layer; } }
    }
}
