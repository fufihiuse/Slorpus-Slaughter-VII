using Microsoft.Xna.Framework;

namespace Slorpus.Objects
{
    public struct Wall
    {
        //fields
        private Rectangle position;
        private bool isBulletCollider;
        private bool isMirror;

        private int WallSprite;

        public Rectangle Position { get { return position; } }
        public bool IsMirror { get { return isMirror; } }
        public bool IsBulletCollider { get { return isBulletCollider; } }

        //constuctor
        public Wall(Rectangle position, bool bullet_collider = true, bool isMirror = false)
        {
            this.position = position;
            isBulletCollider = bullet_collider;
            this.isMirror = isMirror;
            WallSprite = 0;
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
