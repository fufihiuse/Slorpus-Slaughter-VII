using Microsoft.Xna.Framework;

namespace Slorpus.Interfaces
{
    public interface IPointPhysics
    {
        public Point Position { get; }
        public Vector2 Velocity { get; set; }

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
    }
}
