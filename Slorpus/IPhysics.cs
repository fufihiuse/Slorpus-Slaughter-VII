using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * Interface that exposes a class or struct's physics information.
     * Adds the size information to IPointPhysics
     */
    public interface IPhysics: IPointPhysics
    {
        public Point Size { get; }

        /// <summary>
        /// get hitbox size
        /// only here for backwards compatibility, use the Point property instead
        /// </summary>
        /// <returns></returns>
        public Point GetSize();
    }
}
