using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

using Slorpus.Managers;
using Slorpus.Interfaces;
using Slorpus.Interfaces.Base;
using Slorpus.Statics;

namespace Slorpus.Objects
{
    class Player : PhysicsObject, IUpdate, IDraw, IMouseClick, IKeyPress, ILoad, IDestroyable
    {
        Action<Point, Vector2> createBullet;
        // number of bullets the player currently has
        int bullets;
        int health;

        //animation fields
        bool left;
        bool walking;
        KeyboardState pkb;
        List<Texture2D> walkingAnimation;
        List<Texture2D> idleAnimation;
        int currentFrame;
        double timer;
        double frameLength;
        
        // publicly expose players position
        private static Player current;
        public static new Rectangle Position { get { return current.pos; } }
        
        public int Health
        {
            get { return health; }
            set
            {
                if(health - value <= 0)
                {
                    health = 0;
                }
            }
        }

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        public Player(Rectangle pos, ContentManager content, Action<Point, Vector2> bulletCreationFunc): base(pos, Vector2.Zero)
        {
            this.createBullet = bulletCreationFunc;
            bullets = 1;

            // select most recently instatiated player as the current player
            current = this;

            //start up animation vars
            left = false;
            walking = false;
            pkb = Keyboard.GetState();
            currentFrame = 0;
            timer = 0;
            frameLength = 0.1;
            walkingAnimation = new List<Texture2D>();
            idleAnimation = new List<Texture2D>();

            LoadContent(content);
        }

        void IUpdate.Update(GameTime gameTime)
        {
            // per-frame logic
            UpdatePlayerPosition();
            UpdateFrame(gameTime);
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            if (left)
            {
                sb.Draw(
                playerAnimationFrame(),
                new Vector2(target.X, target.Y),
                new Rectangle(0, 0, idleAnimation[0].Width, idleAnimation[0].Height),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.FlipHorizontally,
                0.0f
                );
            }
            else
            {
                sb.Draw(
                playerAnimationFrame(),
                new Vector2(target.X, target.Y),
                new Rectangle(0, 0, idleAnimation[0].Width, idleAnimation[0].Height),
                Color.White,
                0.0f,
                Vector2.Zero,
                1.0f,
                SpriteEffects.None,
                0.0f
                );
            }
        }

        public void LoadContent(ContentManager content)
        {
            idleAnimation.Add(content.Load<Texture2D>("player/lizard_f_idle_anim_f0"));
            idleAnimation.Add(content.Load<Texture2D>("player/lizard_f_idle_anim_f1"));
            idleAnimation.Add(content.Load<Texture2D>("player/lizard_f_idle_anim_f2"));
            idleAnimation.Add(content.Load<Texture2D>("player/lizard_f_idle_anim_f3"));
            walkingAnimation.Add(content.Load<Texture2D>("player/lizard_f_run_anim_f0"));
            walkingAnimation.Add(content.Load<Texture2D>("player/lizard_f_run_anim_f1"));
            walkingAnimation.Add(content.Load<Texture2D>("player/lizard_f_run_anim_f2"));
            walkingAnimation.Add(content.Load<Texture2D>("player/lizard_f_run_anim_f3"));
        }

        void IKeyPress.OnKeyPress(KeyboardState kb)
        {
            // key pressed logic
        }


        /// <summary>
        /// Called by main whenever mouse state changes.
        /// </summary>
        /// <param name="ms">PREVIOUS state of the mouse.</param>
        void IMouseClick.OnMouseClick(MouseState previous)
        {
            MouseState ms = Mouse.GetState();
            Point msLoc = Screen.GetMousePosition(ms);
            if (bullets > 0 && ms.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
            {
                Point pos = new Point(
                    Position.Center.X - (Constants.BULLET_SIZE/2),
                    Position.Center.Y - (Constants.BULLET_SIZE/2)
                    );

                // get distance from player to mouse
                Vector2 vel = new Vector2(
                    ((int)msLoc.X / Constants.WALL_SIZE * Constants.WALL_SIZE + Camera.Position.X) - Position.X,
                    ((int)msLoc.Y / Constants.WALL_SIZE * Constants.WALL_SIZE + Camera.Position.Y) - Position.Y
                    );
                // normalize it
                vel = Vector2.Normalize(vel);
                //multiply by speed
                vel = Vector2.Multiply(vel, Constants.PLAYER_BULLET_SPEED);

                createBullet(pos, vel);

                // test screenshake
                Camera.Shake(10, 5);

                if (!UIManager.IsGodModeOn) {
                    bullets--;
                }

                SoundEffects.PlayEffect(0); // Plays firing sound effect
            }
        }

        /// <summary>
        /// Updates the player position by detecting the keys pressed by the player and the animation state
        /// </summary>
        /// <param name="kb"></param>
        public void UpdatePlayerPosition()
        {
            KeyboardState kb = Keyboard.GetState();
            float speed = 0.5f;
            float xin = 0;
            float yin = 0;
            float xTemp = 0;
            float yTemp = 0;

            //update direction
            if(Screen.GetMousePosition().X > current.pos.X)
            {
                left = false;
            }
            else
            {
                left = true;
            }

            //bool for walking animation
            walking = false;

            if (kb.IsKeyDown(Keys.W))
            { 
                yTemp = -1;
                walking = true;
            }

            if (kb.IsKeyDown(Keys.S))
            { 
                yTemp = 1;
                walking = true;
            }

            if (kb.IsKeyDown(Keys.A))
            { 
                xTemp = -1;
                walking = true;
            }

            if (kb.IsKeyDown(Keys.D))
            { 
                xTemp = 1;
                walking = true;
            }

            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.D))
            {
                xTemp = 0.707f;
                yTemp = -0.707f;
            }
            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.A))
            {
                xTemp = -0.707f;
                yTemp = -0.707f;
            }
            if (kb.IsKeyDown(Keys.A) && kb.IsKeyDown(Keys.S))
            {
                xTemp = -0.707f;
                yTemp = 0.707f;
            }
            if (kb.IsKeyDown(Keys.S) && kb.IsKeyDown(Keys.D))
            {
                xTemp = 0.707f;
                yTemp = 0.707f;
            }
            if (kb.IsKeyDown(Keys.A) && kb.IsKeyDown(Keys.D))
            {
                xTemp = 0f;
            }
            if (kb.IsKeyDown(Keys.W) && kb.IsKeyDown(Keys.S))
            {
                yTemp = 0f;
            }
            xin += xTemp;
            yin += yTemp;

            Velocity = new Vector2((Velocity.X + (xin * speed)) * 0.9f, (Velocity.Y + (yin * speed)) * 0.9f);
            pkb = kb;
        }

        public void Destroy()
        {
            // clean up static variable
            if (current == this)
            {
                current = null;
            }
            Dereferencer.Destroy(this);
        }

        /// <summary>
        /// returns the frame the player should be on
        /// </summary>
        /// <returns></returns>
        private Texture2D playerAnimationFrame()
        {
            if (walking)
            {
                return walkingAnimation[currentFrame];
            }
            return idleAnimation[0];
        }

        private void UpdateFrame(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.TotalSeconds;
            if(timer >= frameLength)
            {
                currentFrame++;
                if (currentFrame >= 3)
                {
                    currentFrame = 0;
                }
                timer -= frameLength;
            }
        }
    }
}
