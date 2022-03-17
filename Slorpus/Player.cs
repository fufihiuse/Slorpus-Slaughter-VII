using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    class Player : PhysicsObject, IUpdate, IDraw
    {
        // reference the game's physics manager
        PhysicsManager physicsManager;
        // number of bullets the player currently has
        int bullets = 1;

        Texture2D bulletAsset;
        Texture2D asset;

        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="vel"></param>
        public Player(Rectangle pos, Vector2 vel, PhysicsManager physicsManager, Texture2D playerAsset, Texture2D bulletAsset): base(pos, vel)
        {
            this.physicsManager = physicsManager;
            this.asset = playerAsset;
            this.bulletAsset = bulletAsset;
        }

        void IUpdate.Update(GameTime gameTime)
        {
            // per-frame logic
            UpdatePlayerPosition();
        }

        void IDraw.Draw(SpriteBatch sb)
        {
            sb.Draw(asset, this.Position, Color.White);
        }
        
        /// <summary>
        /// Called by main whenever mouse button state changes.
        /// </summary>
        /// <param name="ms">CURRENT state of the mouse.</param>
        public void OnMouseClick(MouseState ms)
        {
            if (bullets > 0 && ms.LeftButton == ButtonState.Pressed)
            {
                // use our reference to the physics manager to instantiate the player bullet
                Rectangle bulletRect = new Rectangle(
                    new Point(this.Position.X, this.Position.Y),
                    new Point(Constants.PLAYER_BULLET_SIZE, Constants.PLAYER_BULLET_SIZE)
                    );

                // get distance from player to mouse
                Vector2 vel = new Vector2(
                    ms.X-this.Position.X,
                    ms.Y-this.Position.Y
                    );
                // normalize it
                vel = Vector2.Normalize(vel);
                //multiply by speed
                vel = Vector2.Multiply(vel, Constants.PLAYER_BULLET_SPEED);
                
                physicsManager.AddPhysicsObject(new PlayerProjectile(bulletRect, vel));
                bullets--;
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
