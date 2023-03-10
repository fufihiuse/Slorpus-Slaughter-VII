using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Slorpus.Utils
{
    public static class CollisionMath
    {
        public static CollisionInfo Between(Rectangle b1, Rectangle b2)
        {
            // dont use the builtin Intersects method because we need additional information
            // about the overlap like depth and normal
            // NOTE: using Top as the minimum side of the collision, assumes Top < Bottom (inverted coordinates)
            Vector2 min = new Vector2(Math.Max(b1.Left, b2.Left), Math.Max(b1.Top, b2.Top));
            Vector2 max = new Vector2(Math.Min(b1.Right, b2.Right), Math.Min(b1.Bottom, b2.Bottom));
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
                normal.X *= ((b1.Left > b2.Left) ? 2 : 0) - 1;
                normal.Y *= ((b1.Top > b2.Top) ? 2 : 0) - 1;
                normal.Normalize();
            }

            return new CollisionInfo(collided, depth, normal);
        }
    }
    public struct CollisionInfo
    {
        private bool collided;
        private Vector2 depth;
        private Vector2 normal;
        public bool Collided { get { return collided; } }
        public Vector2 Depth { get { return depth; } }
        public Vector2 Normal { get { return normal; } }

        public CollisionInfo(bool collided, Vector2 depth, Vector2 normal)
        {
            this.collided = collided;
            this.depth = depth;
            this.normal = normal;
        }
    }
}
