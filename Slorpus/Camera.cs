using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    class Camera : IPosition, IUpdate
    {
        Rectangle pos;
        IPosition followTarget;
        float lerpSpeed;
        Func<int[]> getClamp;
        Func<Point> getScreenSize;

        public Rectangle Position { get { return pos; } set { pos = value; } }

        public Point Offset { get { return pos.Location; } }

        public Camera(IPosition followTarget, float lerpSpeed, Func<int[]> getClamp, Func<Point> getScreenSize)
        {
            if (followTarget == this)
            {
                throw new Exception("Camera cannot follow itself.");
            }
            this.followTarget = followTarget;
            this.lerpSpeed = lerpSpeed;
            this.getClamp = getClamp;
            this.getScreenSize = getScreenSize;
        }

        public void LerpFollow(float lerpSpeedOverride=-1, bool clamp = true)
        {
            float useSpeed = lerpSpeedOverride;
            if (useSpeed > 1 || useSpeed < 0)
            {
                useSpeed = lerpSpeed;
            }

            Point screenOffset = getScreenSize();

            pos.X = (int)MathHelper.Lerp(pos.X, followTarget.Position.X - (screenOffset.X/2), useSpeed);
            pos.Y = (int)MathHelper.Lerp(pos.Y, followTarget.Position.Y - (screenOffset.Y/2), useSpeed);

            if (clamp)
            {
                Clamp();
            }
        }

        public void Update(GameTime gameTime)
        {
            LerpFollow();
        }

        public void Clamp()
        {
            int[] clamps = getClamp();
            // format:
            // [left border, right border, top border, bottom border]

            pos.X = Math.Max(clamps[0], pos.X);
            pos.X = Math.Min(clamps[1], pos.X);
            pos.Y = Math.Max(clamps[2], pos.Y);
            pos.Y = Math.Min(clamps[3], pos.Y);
        }
    }
}
