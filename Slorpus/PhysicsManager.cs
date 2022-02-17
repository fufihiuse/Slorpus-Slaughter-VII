using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public static class PhysicsManager
    {
        /// <summary>
        /// Moves objects which implement IPhysics and calls their respective collision handlers if necessary (STRETCH GOAL)
        /// </summary>
        /// <param name="physicObjects">List of physics objects with speeds and sizes that need to be moved.</param>
        /// <param name="walls">List of walls that should stop movement.</param>
        public static void MovePhysics(List<IPhysics> physicObjects, Wall[,] walls)
        {
            foreach (IPhysics p in physicObjects)
            {
                // placeholder, no collision
                // TODO : check if this works on value types, or if the changes pass out of scope after this loop
                p.Move(p.GetVelocity());
            }
        }
        
        /// <summary>
        /// Physics object movement for if a long list of physics objects have identical sizes.
        /// </summary>
        /// <param name="physicObjects">Objects that implement IPointPhysics at least.</param>
        /// <param name="size">The size that every physics object has.</param>
        /// <param name="walls">List of walls that should stop movement.</param>
        public static void MovePointPhysics(List<IPointPhysics> physicObjects, Point size, Wall[,] walls)
        {
            foreach (IPhysics p in physicObjects)
            {
                // placeholder, no collision
                // TODO : check if this works on value types, or if the changes pass out of scope after this loop
                p.Move(p.GetVelocity());
            }
        }
    }
}
