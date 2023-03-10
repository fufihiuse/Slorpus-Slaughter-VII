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

            this.mask = (bullet_collider) ? Constants.WALL_COLLISION_MASK : Constants.BOW_COLLISION_MASK;
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
            
            // also get depth and normal if necessary
            Vector2 depth = new Vector2(0.0f);
            Vector2 normal = new Vector2(0.0f);

            if (collided)
            {
                // calculate depth
                depth = max - min;
                
                // make normal components signs of depth
                if (depth.Y != 0)
                {
                    // if depth.x > depth.y, then the collision is more horizontal and the normal's
                    // Y component should be the non-zero sign of depth.Y (normal pointing either up or down).
                    normal.Y = (depth.X >= depth.Y) ? (depth.Y / Math.Abs(depth.Y)) : 0;
                }
                if (depth.X != 0)
                {
                    // same theory: if the collision is more vertical, then we need to correct horizontally
                    // so the normal points as the sign of the depth
                    normal.X = (depth.X <= depth.Y) ? (depth.X / Math.Abs(depth.X)) : 0;
                }
                normal.X *= ((hitbox.Left > position.Left) ? 2 : 0) - 1;
                normal.Y *= ((hitbox.Top > position.Top) ? 2 : 0) - 1;
                normal.Normalize();
            }

            return new CollisionInfo(collided, depth, normal);
        }
    }
}
