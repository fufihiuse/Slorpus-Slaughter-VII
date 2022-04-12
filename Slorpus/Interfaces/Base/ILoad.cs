using Microsoft.Xna.Framework.Content;

namespace Slorpus.Interfaces.Base
{
    /// <summary>
    /// Designates an object that has assets 
    /// </summary>
    interface ILoad
    {
        public void LoadContent(ContentManager content);
    }
}
