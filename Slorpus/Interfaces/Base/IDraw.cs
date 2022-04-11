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
        public void Draw(SpriteBatch spriteBatch);
    }
}
