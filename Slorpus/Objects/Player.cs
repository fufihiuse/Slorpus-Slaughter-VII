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
        // bullet instantiation function
        Action<Point, Vector2> createBullet;

        // player state
        int bullets;
        int health;

        //animation state
        bool walking;
        
        // physics information
        private float mass = Constants.PLAYER_MASS;
        public override ushort Mask { get { return Constants.PLAYER_COLLISION_MASK; } }
        public override float Mass { get { return mass; } }        
        public static new Rectangle Position { get { return current.pos; } }
        

        // the step sound effects that need to be player
        Queue<Step> steps;
        
        private static Player current;
        
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

            //start up animation vars
            walking = false;

            LoadContent(content);
        }

        void IUpdate.Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            GetAndApplyInput(kb);
            
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

            // apply friction every frame, if moving
            if (vel.Length() != 0)
            {
                Vector2 friction = Vector2.Negate(Vector2.Normalize(vel)
                    * Constants.PLAYER_FRICTION_COEFFICIENT * Constants.GRAVITY * mass);

                // clamp the friction to be no larger than the velocity
                friction.X = (Math.Clamp(Math.Abs(friction.X), 0, Math.Abs(vel.X))) * Math.Sign(friction.X);
                friction.Y = (Math.Clamp(Math.Abs(friction.Y), 0, Math.Abs(vel.Y))) * Math.Sign(friction.Y);

                impulses += friction;
            }
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            Rectangle target = Position;
            target.Location -= Camera.Offset;
            sb.Draw(Game1.SquareTexture, target, Color.White);
        }
        
        // unused, but need to be implemented to make interfaces happy
        public void LoadContent(ContentManager content){}
        void IKeyPress.OnKeyPress(KeyboardState kb) {}


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

                SoundEffects.PlayEffectVolume("bullet", 0.8f, 0, 0); // Plays firing sound effect
            }
        }

        /// <summary>
        /// Gets the input from the keyboard and applies it to the player's state; the walking state
        /// as well as the physics impulses are updated to move the player.
        /// </summary>
        /// <param name="kb">The current state of the keyboard.</param>
        public void GetAndApplyInput(KeyboardState kb)
        {
            float xin = 0;
            float yin = 0;

            xin += kb.IsKeyDown(Keys.A) ? -1 : 0;
            xin += kb.IsKeyDown(Keys.D) ? 1 : 0;
            yin += kb.IsKeyDown(Keys.S) ? 1 : 0;
            yin += kb.IsKeyDown(Keys.W) ? -1 : 0;

            Vector2 input = new Vector2(xin, yin);

            if (input.LengthSquared() > 0) {
                input = Vector2.Normalize(input);
                // whether or not we are walking = whether or not we are giving input
                // (use by animations and sound playing)
                walking = true;
            }

            impulses += input * Constants.PLAYER_MOVE_IMPULSE;
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
            play = () => { SoundEffects.PlayEffectVolume(soundEffect, 0.15f, 0, 0); };
            timer = length;

            // wow this code sucks man
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
