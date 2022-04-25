using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

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
        // player texture
        Texture2D asset;
        int health;

        Queue<Step> steps;
        
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

            steps = new Queue<Step>();
            // pre-queue the first step
            steps.Enqueue(new Step(Constants.PLAYER.STEP_SPEED, "walk1"));

            // select most recently instatiated player as the current player
            current = this;

            LoadContent(content);
        }

        void IUpdate.Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            UpdatePlayerPosition(kb);
            
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.S) ||
                kb.IsKeyDown(Keys.D) || kb.IsKeyDown(Keys.A))
            {
                Step current = steps.Peek();
                current.Decrement();
                if (current.Played)
                {
                    steps.Dequeue();
                    steps.Enqueue(
                        new Step(
                            Constants.PLAYER.STEP_SPEED,
                            current.NextSound)
                        );
                }
            }
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            sb.Draw(asset, target, Color.White);
        }

        public void LoadContent(ContentManager content)
        {
            asset = Game1.SquareTexture; // content.Load<Texture2D>("square");
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
                vel = Vector2.Multiply(vel, Constants.PLAYER.BULLET_SPEED);

                createBullet(pos, vel);

                // test screenshake
                Camera.Shake(10, 5);

                if (!UIManager.IsGodModeOn) {
                    bullets--;
                }

                SoundEffects.PlayEffect("bullet"); // Plays firing sound effect
            }
        }

        /// <summary>
        /// Updates the player position by detecting the keys pressed by the player
        /// </summary>
        /// <param name="kb"></param>
        public void UpdatePlayerPosition(KeyboardState kb)
        {
            float speed = 0.5f;
            float xin = 0;
            float yin = 0;
            float xTemp = 0;
            float yTemp = 0;

            if (kb.IsKeyDown(Keys.W))
            { yTemp = -1; }

            if (kb.IsKeyDown(Keys.S))
            { yTemp = 1; }

            if (kb.IsKeyDown(Keys.A))
            { xTemp = -1; }

            if (kb.IsKeyDown(Keys.D))
            { xTemp = 1; }

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
    }
    /// <summary>
    /// One entry in a queue of steps, used for playing a series of sounds.
    /// </summary>
    class Step
    {
        private string nextSound;
        private bool played = false;

        public string NextSound { get { return nextSound; } }
        public bool Played { get { return played; } }
        private int timer;
        Action play;

        public Step(int length, string soundEffect)
        {
            play = () => { SoundEffects.PlayEffect(soundEffect); };
            timer = length;
            
            switch (soundEffect)
            {
                case "walk1":
                    nextSound = "walk2";
                    break;
                case "walk2":
                    nextSound = "walk1";
                    break;
                default:
                    throw new Exception($"sound {soundEffect} not accounted for in this switch.");
            }
        }
        public void Decrement()
        {
            timer--;
            if (timer == 0)
            {
                play();
                played = true;
            }
        }
    }
}
