using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Slorpus
{
    /// <summary>
    /// Designates an object that has assets 
    /// </summary>
    interface ILoad
    {
        public void LoadContent(ContentManager content);
    }
}
