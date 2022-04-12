using Microsoft.Xna.Framework;

namespace Slorpus.Interfaces
{
    /* Identical to IPointPhysics but with a Rectangle instead of a Point (added width and height information)
     */
    interface IPhysics : IPosition
    {
        public Vector2 Velocity { get; set; }
        public Vector2 SubpixelOffset { get; }
        public Vector2 SubpixelCoords { get; }

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
        public void Move(Vector2 distance);
        
        /// <summary>
        /// Moves the object to a location.
        /// </summary>
        /// <param name="location">The coordinates to move to.</param>
        public void Teleport(Point location);
        
        /// <summary>
        /// Called by the PhysicsManager whenever this object collides with a wall.
        /// </summary>
        /// <param name="other">The rectangle of the other object collided with.</param>
        public bool OnCollision<T>(T other);
        
        /// <summary>
        /// Called by the PhysicsManager whenever this object collides with a wall.
        /// </summary>
        /// <param name="other">The rectangle of the other object collided with.</param>
        /// <param name="previousVelocity">The velocity before being corrected for collision.</param>
        /// <param name="wantedPosition">Position of the object before being corrected to prevent collision.</param>
        public bool OnCollisionComplex<T>(T other, Vector2 previousVelocity, Point wantedPosition);
    }
}
