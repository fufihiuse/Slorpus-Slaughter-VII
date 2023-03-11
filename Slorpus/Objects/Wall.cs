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

        public Rectangle Position { get { return position; } }
        public Rectangle SubTex { get { return subTex; } }
        public bool IsMirror { get { return isMirror; } }
        public ushort Mask { get { return mask; } }

        //constuctor
        public Wall(Rectangle position, Rectangle subTex, bool bullet_collider = true, bool isMirror = false)
        {
            this.position = position;
            this.isMirror = isMirror;
            this.subTex = subTex;

            mask = (bullet_collider) ? Constants.WALL_COLLISION_MASK : Constants.BOW_COLLISION_MASK;
        }

        /// <summary>
        /// Get whether or not a rectangle is colliding with this wall, and the depth and normal of the collision.
        /// </summary>
        /// <param name="rect">The rectangle to check for collision with.</param>
        /// <returns>A CollisionInfo struct with all the information about the collision.</returns>
        public CollisionInfo Collision(Rectangle rect)
        {
            return CollisionMath.Between(rect, position);
        }
    }
}
