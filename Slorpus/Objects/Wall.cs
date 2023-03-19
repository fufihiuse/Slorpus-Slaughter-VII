using System;

using Microsoft.Xna.Framework;
using Slorpus.Utils;
using Slorpus.Statics;

namespace Slorpus.Objects
{
    public struct Wall
    {
        //fields
        private Rectangle position;
        private Rectangle subTex;
        private bool isMirror;
        private ushort mask;
        private ushort bit;
        private Point levelCoordinate;

        public Rectangle Position { get { return position; } }
        public Rectangle SubTex { get { return subTex; } }
        public bool IsMirror { get { return isMirror; } }
        public ushort Mask { get { return mask; } }
        public ushort Bit { get { return bit; } }
        public Point LevelCoordinate { get { return levelCoordinate; } }

        int id;
        public int ID { get { return id; } }

        //constuctor
        public Wall(Rectangle position, Rectangle subTex, Point levelCoordinate, bool bullet_collider = true, bool isMirror = false)
        {
            this.position = position;
            this.isMirror = isMirror;
            this.subTex = subTex;
            this.levelCoordinate = levelCoordinate;

            id = UUID.get();
            
            // mask is actually unecessary because the wall has no collision handlers
            mask = (bullet_collider) ? Constants.WALL_COLLISION_MASK : Constants.BOW_COLLISION_MASK;
            bit = (bullet_collider) ? Constants.WALL_COLLISION_BIT : Constants.BOW_COLLISION_BIT;

            if (!bullet_collider && isMirror)
            {
                throw new Exception("Cannot initialize a wall which is both a mirror and a BOW.");
            }
        }

        /// <summary>
        /// Get whether or not a rectangle is colliding with this wall, and the depth and normal of the collision.
        /// </summary>
        /// <param name="rect">The rectangle to check for collision with.</param>
        /// <returns>A CollisionInfo struct with all the information about the collision.</returns>
        public CollisionInfo Collision(Rectangle rect, Vector2 subPixelOffset)
        {
            return CollisionMath.Between(rect, position, subPixelOffset);
        }
    }
}
