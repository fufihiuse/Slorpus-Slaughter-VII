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
        // get location and hitbox size
        public Point GetSize();
    }
}
