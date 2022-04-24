using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public bool Collision(Rectangle hitbox)
        {
            return position.Intersects(hitbox);
        }
    }
}
