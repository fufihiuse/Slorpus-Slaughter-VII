using Microsoft.Xna.Framework.Graphics;

namespace Slorpus.Interfaces.Base
{
    /*
     * Interface for special objects not already drawn by their own manager classes.
     */
    interface IDraw
    {
        public void Draw(SpriteBatch spriteBatch);
    }
}
