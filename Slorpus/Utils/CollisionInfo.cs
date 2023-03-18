using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Slorpus.Utils
{
    public static class CollisionMath
    {
        public static CollisionInfo Between(Rectangle b1, Rectangle b2,
            Vector2 subPixelOffset1=new Vector2(), Vector2 subPixelOffset2=new Vector2())
        {
            // dont use the builtin Intersects method because we need additional information
            // about the overlap like depth and normal
            // NOTE: using Top as the minimum side of the collision, assumes Top < Bottom (inverted coordinates)
            float b1Left = b1.Left + subPixelOffset1.X;
            float b1Right = b1.Right + subPixelOffset1.X;
            float b2Left = b2.Left + subPixelOffset2.X;
            float b2Right = b2.Right + subPixelOffset2.X;
            float b1Top = b1.Top + subPixelOffset1.Y;
            float b1Bottom = b1.Bottom + subPixelOffset1.Y;
            float b2Top = b2.Top + subPixelOffset2.Y;
            float b2Bottom = b2.Bottom + subPixelOffset2.Y;
            Vector2 min = new Vector2(Math.Max(b1Left, b2Left), Math.Max(b1Top, b2Top));
            Vector2 max = new Vector2(Math.Min(b1Right, b2Right), Math.Min(b1Bottom, b2Bottom));
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
                normal.X *= ((b1Left > b2Left) ? 2 : 0) - 1;
                normal.Y *= ((b1Top > b2Top) ? 2 : 0) - 1;
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
