using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public enum CameraMovement
    {
        // does not go past edges of level
        // level is aligned to bottom right if smaller than screen
        Normal,
        // level does not track anything
        Centered,
        // no clamping
        Free
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
    class Camera: IUpdate
    {
        static private Rectangle pos;
        static private Point shakeOffset;
        static private List<ShakeRequest> shakeQueue;
        private CameraMovement moveConstraint;
        // method that gets called when moving
        private Action moveBehavior;
        
        Func<Rectangle> followTarget;
        float lerpSpeed;
        Random gen;

        static public Rectangle Position { get { return pos; } }

        static public Point Offset { get {
                return pos.Location + shakeOffset;
            }
        }
        public CameraMovement MovementConstraint
        {
            get { return moveConstraint; }
            set
            {
                switch (value)
                {
                    case CameraMovement.Normal:
                        // move and then clamp to level size
                        moveBehavior = () => {
                            LerpFollow();
                            Clamp();
                        };
                        moveConstraint = CameraMovement.Normal;
                        break;
                    case CameraMovement.Free:
                        // do nothing on move except follow target
                        moveBehavior = () => { LerpFollow(); };
                        moveConstraint = CameraMovement.Free;
                        break;
                    case CameraMovement.Centered:
                        // centers the camera in the level
                        pos.X = (Level.Size.X * Constants.WALL_SIZE / 2) - Screen.Size.X/2;
                        pos.Y = (Level.Size.Y * Constants.WALL_SIZE / 2) - Screen.Size.Y/2;
                        
                        // do nothing on move
                        moveBehavior = () => { };
                        moveConstraint = CameraMovement.Centered;
                        break;
                }
            }
        }

        public Camera(Func<Rectangle> followTarget, float lerpSpeed, bool NoOverrideShake=false)
        {
            this.followTarget = followTarget;
            this.lerpSpeed = lerpSpeed;
            // default camera mode
            MovementConstraint = CameraMovement.Normal;
            // if level is small, do centered instead
            if (Level.Size.X * Constants.WALL_SIZE < Screen.Size.X || Level.Size.Y *Constants.WALL_SIZE < Screen.Size.Y)
            {
                MovementConstraint = CameraMovement.Centered;
            }
            
            if (!NoOverrideShake)
                shakeQueue = new List<ShakeRequest>();
            
            gen = new Random();
        }

        /// <summary>
        /// Interpolates the camera position towards its current target.
        /// </summary>
        /// <param name="lerpSpeedOverride">Overrides the camera's interpolation speed, a value between 0 and 1. (optional)</param>
        public void LerpFollow(float lerpSpeedOverride=-1)
        {
            float useSpeed = lerpSpeedOverride;
            if (useSpeed > 1 || useSpeed < 0)
            {
                useSpeed = lerpSpeed;
            }

            Point screenOffset = Screen.Size;

            pos.X = (int)MathHelper.Lerp(pos.X, followTarget().X - (screenOffset.X/2), useSpeed);
            pos.Y = (int)MathHelper.Lerp(pos.Y, followTarget().Y - (screenOffset.Y/2), useSpeed);
        }

        public void Update(GameTime gameTime)
        {
            moveBehavior();
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

        public static void Shake(int length, int strength, bool isConstant=false)
        {
            shakeQueue.Add(new ShakeRequest(length, strength, isConstant));
        }
        
        /// <summary>
        /// Locks the camera to not go past certain points (ie, the edges of the level)
        /// </summary>
        public void Clamp()
        {
            Point size = Screen.Size;
            pos.X = Math.Max(0, pos.X);
            pos.X = Math.Min(Level.Size.X * Constants.WALL_SIZE - size.X, pos.X);
            pos.Y = Math.Max(0, pos.Y);
            pos.Y = Math.Min(Level.Size.Y * Constants.WALL_SIZE - size.Y, pos.Y);
        }
    }
}
