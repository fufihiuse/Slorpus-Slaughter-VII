using Microsoft.Xna.Framework;
using Slorpus.Utils;

namespace Slorpus.Interfaces
{
    /* Identical to IPointPhysics but with a Rectangle instead of a Point (added width and height information)
     */
    interface IPhysics : IPosition
    {
        public Vector2 Velocity { get; set; }
        public Vector2 SubpixelOffset { get; }
        public Vector2 SubpixelCoords { get; }
        public Vector2 Impulses { get; set; }

        public ushort Mask { get; }

        public float Mass { get; }

        public int ID { get; }

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
        /// Adds the object's impulses to its velocity.
        /// </summary>
        public void ApplyImpulses();
        
        /// <summary>
        /// Called by the PhysicsManager whenever this object collides with a wall.
        /// </summary>
        /// <param name="other">The rectangle of the other object collided with.</param>
        /// <param name="collision">Information about the collision that occurred.</param>
        public bool OnCollision<T>(T other, CollisionInfo collision);
    }
}
