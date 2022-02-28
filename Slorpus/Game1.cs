using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D squareTexture;

        // manager(s)
        Level level;
        List<Enemy> enemyList;

        // debug object
        PhysicsObject DEBUG;
        Player player;
        List<IPhysics> physicsList;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            physicsList = new List<IPhysics>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            squareTexture = Content.Load<Texture2D>("square");

            //Using example textures
            level = new Level(Constants.WALL_SIZE, squareTexture, squareTexture, squareTexture, squareTexture);
            level.LoadFromFile("..\\..\\..\\levels\\maze.sslvl", out player, out enemyList); //Loads example level, should be changed

            DEBUG = new PhysicsObject(
                new Rectangle(
                    // position
                    new Point(200, 200),
                    // size
                    new Point(16, 16)),
                new Vector2(0, 0));

            physicsList.Add(DEBUG);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState kb = Keyboard.GetState();

            int xin = 0;
            int yin = 0;
            float speed = 0.5f;

            if (kb.IsKeyDown(Keys.W))
                yin -= 1;
            if (kb.IsKeyDown(Keys.S))
                yin += 1;
            if (kb.IsKeyDown(Keys.A))
                xin -= 1;
            if (kb.IsKeyDown(Keys.D))
                xin += 1;

            DEBUG.Velocity = new Vector2((DEBUG.Velocity.X + (xin * speed)) * 0.9f, (DEBUG.Velocity.Y + (yin * speed)) * 0.9f);

            PhysicsManager.MovePhysics(physicsList, level.WallList);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            level.Draw(_spriteBatch);
            _spriteBatch.Draw(squareTexture, DEBUG.Position, Color.White);
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
