using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * used to reference the current camera
     * global variable ;)
     */
    static class CameraControl
    {
        public static Camera Camera;
    }
    /*
     * Simple struct that describes an instance of screenshake
     * so that multiple shakes can happen at once
     * ie. one long small shake with big shakes happening in the middle
     * each of these is stored in a list in camera
     */
    struct ShakeRequest
    {
        private bool isConstant;
        private int initialTimer;
        public int timer;
        public int maxShake;
        
        public int ShakeRange { get
            {
                return (isConstant) ? maxShake : GetShakeRange();
            }
        }

        public ShakeRequest(int lifetime, int shake_strength, bool IsConstant)
        {
            initialTimer = lifetime;
            timer = lifetime;
            isConstant = IsConstant;
            maxShake = shake_strength;
        }

        private int GetShakeRange()
        {
            return (int)((((float)timer) / ((float)initialTimer)) * maxShake);
        }
    }
    /*
     * This class contains and offset and some options for modifying it
     * as well as a follow target that it can linearly interpolate towards
     */
    class Camera : IPosition, IUpdate
    {
        Rectangle pos;
        IPosition followTarget;
        float lerpSpeed;
        Func<int[]> getClamp;
        Func<Point> getScreenSize;

        Point shakeOffset;
        List<ShakeRequest> shakeQueue;

        Random gen;

        public Rectangle Position { get { return pos; } set { pos = value; } }

        public Point Offset { get {
                return pos.Location + shakeOffset;
            }
        }

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

            shakeQueue = new List<ShakeRequest>();
            gen = new Random();
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
            // reset last frame's shake
            shakeOffset = Point.Zero;

            // shake the screen
            List<ShakeRequest> removals = new List<ShakeRequest>();
            for (int i = 0; i < shakeQueue.Count; i++)
            {
                int range = shakeQueue[i].maxShake;
                shakeOffset.X += gen.Next(range) - range/2;
                shakeOffset.Y += gen.Next(range) - range/2;
                ShakeRequest placeholder = shakeQueue[i];
                placeholder.timer -= 1;
                shakeQueue[i] = placeholder;

                if (placeholder.timer <= 0)
                {
                    removals.Add(placeholder);
                }
            }

            // remove finished shakes
            foreach (ShakeRequest remove in removals)
            {
                shakeQueue.Remove(remove);
            }
        }

        public void Shake(int length, int strength, bool isConstant=false)
        {
            shakeQueue.Add(new ShakeRequest(length, strength, isConstant));
        }
        
        /// <summary>
        /// Locks the camera to not go past certain points (ie, the edges of the level)
        /// </summary>
        public void Clamp()
        {
            int[] clamps = getClamp();
            // format:
            // [left border, right border, top border, bottom border]
            Point size = getScreenSize();
            pos.X = Math.Max(clamps[0], pos.X);
            pos.X = Math.Min(clamps[1] - size.X, pos.X);
            pos.Y = Math.Max(clamps[2], pos.Y);
            pos.Y = Math.Min(clamps[3] - size.Y, pos.Y);
        }

        public void Select()
        {
            CameraControl.Camera = this;
        }
    }
}
