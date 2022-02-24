using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /* Identical to IPointPhysics but with a Rectangle instead of a Point (added width and height information)
     * 
     * 
     */
    public interface IPhysics
    {
        public Rectangle Position { get; }
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// returns velocity
        /// this is just here for backwards compatibility, use the Velocity property instead.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetVelocity();
        /// <summary>
        /// Moves the object relative to its current position.
        /// </summary>
        /// <param name="distance"></param>
        public void Move(Point distance);
        
        /// <summary>
        /// Moves the object to a location.
        /// </summary>
        /// <param name="location">The coordinates to move to.</param>
        public void Teleport(Point location);
        
        /// <summary>
        /// Called by the PhysicsManager whenever this object collides with a wall.
        /// </summary>
        /// <param name="other">The rectangle of the other object collided with.</param>
        public void OnCollision(Rectangle other);
    }
}
