using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Slorpus
{
    class Player : PhysicsObject, IUpdate, IDraw, IMouseClick, IKeyPress
    {
        Action<Point, Vector2> createBullet;
        // number of bullets the player currently has
        int bullets;
        // player texture
        Texture2D asset;

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        public Player(Rectangle pos, Vector2 vel, Action<Point, Vector2> bulletCreationFunc, Texture2D playerAsset): base(pos, vel)
        {
            this.createBullet = bulletCreationFunc;
            this.asset = playerAsset;
            bullets = 1;
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
            if (bullets > 0 && ms.LeftButton == ButtonState.Pressed && previous.LeftButton == ButtonState.Released)
            {
                Point pos = new Point(Position.X, Position.Y);

                // get distance from player to mouse
                Vector2 vel = new Vector2(
                    (ms.X + Camera.Position.X)-Position.X,
                    (ms.Y + Camera.Position.Y)-Position.Y
                    );
                // normalize it
                vel = Vector2.Normalize(vel);
                //multiply by speed
                vel = Vector2.Multiply(vel, Constants.PLAYER_BULLET_SPEED);

                createBullet(pos, vel);
                // test screenshake
                Camera.Shake(10, 5);
                //bullets--;

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
    }
}
