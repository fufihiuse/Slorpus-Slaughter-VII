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
        Player DEBUG;
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

            level = new Level(Constants.WALL_SIZE, squareTexture);
            level.LoadFromFile("..\\..\\..\\levels\\example.sslvl"); //Loads example level, should be changed

            DEBUG = new Player(
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
            DEBUG.UpdatePlayerPosition();
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
