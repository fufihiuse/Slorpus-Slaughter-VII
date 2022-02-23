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

        // debug object
        PhysicsObject DEBUG;
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            squareTexture = Content.Load<Texture2D>("square");

            level = new Level(19, squareTexture, squareTexture);
            level.LoadFromFile("..\\..\\..\\levels\\example.sslvl"); //Loads example level, should be changed

            DEBUG = new PhysicsObject(new Point(200, 200), new Vector2(0, 0), Constants.WALL_SIZE, Constants.WALL_SIZE);
            physicsList.Add(DEBUG);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // accelerate debug object
            DEBUG.Velocity = new Vector2(DEBUG.Velocity.X, DEBUG.Velocity.Y + 0.1f);

            PhysicsManager.MovePhysics(physicsList, level.Walls);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            level.Draw(_spriteBatch);
            _spriteBatch.Draw(squareTexture, new Rectangle(DEBUG.Position, DEBUG.Size), Color.White);
            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
