using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Slorpus.Utils
{
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
