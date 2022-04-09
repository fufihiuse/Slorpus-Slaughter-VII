using Microsoft.Xna.Framework;

namespace Slorpus
{
    /*
     * simple interface that "special" objects like the player use.
     * That is, objects that dont have managers but do have unique per-frame logic.
     * NOT bullets, enemies, or walls.
     */
    interface IUpdate
    {
        public void Update(GameTime gameTime);
    }
}
