using System;
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

            LoadContent(content);
        }

        void IUpdate.Update(GameTime gameTime)
        {
            // per-frame logic
            UpdatePlayerPosition();
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
            if (kb.IsKeyDown(Keys.W) || kb.IsKeyDown(Keys.A) || kb.IsKeyDown(Keys.S) || kb.IsKeyDown(Keys.D))
            {
                SoundEffects.PlayEffect(5);
                for (int i = 0; i < 60; i++)
                {

                }
                SoundEffects.PlayEffect(6);
                for (int i = 0; i < 60; i++)
                {

                }
            }
        }


        /// <summary>
        /// Called by main whenever mouse state changes.
        /// </summary>
        /// <param name="ms">PREVIOUS state of the mouse.</param>
        void IMouseClick.OnMouseClick(MouseState previous)
        {
            MouseState ms = Mouse.GetState();
            if (bullets > 0 && ms.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
            {
                Point pos = new Point(
                    Position.Center.X - (Constants.BULLET_SIZE/2),
                    Position.Center.Y - (Constants.BULLET_SIZE/2)
                    );

                // get distance from player to mouse
                Vector2 vel = new Vector2(
                    ((int)ms.X / Constants.WALL_SIZE * Constants.WALL_SIZE + Camera.Position.X) - Position.X,
                    ((int)ms.Y / Constants.WALL_SIZE * Constants.WALL_SIZE + Camera.Position.Y) - Position.Y
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
        /// Updates the player position by detecting the keys pressed by the player
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
}
