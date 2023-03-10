using System;

using Microsoft.Xna.Framework;
using Slorpus.Utils;

namespace Slorpus.Objects
{
    public struct Wall
    {
        //fields
        private Rectangle position;
        private Rectangle subTex;
        private bool isBulletCollider;
        private bool isMirror;

        public Rectangle Position { get { return position; } }
        public Rectangle SubTex { get { return subTex; } }
        public bool IsMirror { get { return isMirror; } }
        public bool IsBulletCollider { get { return isBulletCollider; } }

        //constuctor
        public Wall(Rectangle position, Rectangle subTex, bool bullet_collider = true, bool isMirror = false)
        {
            this.position = position;
            isBulletCollider = bullet_collider;
            this.isMirror = isMirror;
            this.subTex = subTex;
        }

        /// <summary>
        /// returns if the hitbox has collided with the wall
        /// </summary>
        /// <param name="hitbox"></param>
        /// <returns></returns>
        public CollisionInfo Collision(Rectangle hitbox)
        {
            // dont use the builtin Intersects method because we need additional information
            // about the overlap like depth and normal
            // NOTE: using Top as the minimum side of the collision, assumes Top < Bottom (inverted coordinates)
            Vector2 min = new Vector2(Math.Max(hitbox.Left, position.Left), Math.Max(hitbox.Top, position.Top));
            Vector2 max = new Vector2(Math.Min(hitbox.Right, position.Right), Math.Min(hitbox.Bottom, position.Bottom));
            bool collided = min.X < max.X && min.Y < max.Y;

            Vector2 depth = new Vector2(0.0f);
            Vector2 normal = new Vector2(0.0f);

            return new CollisionInfo(collided, depth, normal);
        }
    }
}
