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

        // managers
        Level level;
        EnemyManager enemyManager;
        BulletManager bulletManager;
        PhysicsManager physicsManager;

        // debug object
        PhysicsObject DEBUG;

        // lists
        // these (usually) should not be modified directly, edit them with the managers
        List<IPhysics> physicsList;
        List<Enemy> enemyList;
        EnemyBullet[] bulletList;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            physicsList = new List<IPhysics>();
            enemyList = new List<Enemy>();
            bulletList = new EnemyBullet[100];

            // TODO: properly reallocate space instead of just having a static large array
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            squareTexture = Content.Load<Texture2D>("square");

            level = new Level(Constants.WALL_SIZE, squareTexture);
            level.LoadFromFile("..\\..\\..\\levels\\example.sslvl"); //Loads example level, should be changed

            DEBUG = new PhysicsObject(
                new Rectangle(
                    // position
                    new Point(200, 200),
                    // size
                    new Point(16, 16)),
                new Vector2(0, 0));

            physicsList.Add(DEBUG);

            bulletManager = new BulletManager(bulletList, squareTexture);
            enemyManager = new EnemyManager(enemyList, squareTexture, bulletManager);
            physicsManager = new PhysicsManager(physicsList, level.WallList, enemyManager, bulletManager);
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

            enemyManager.UpdateEnemies(gameTime);
            physicsManager.MovePhysics(gameTime);
            physicsManager.CollideAndMoveBullets(gameTime, new Point(Constants.BULLET_SIZE, Constants.BULLET_SIZE));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            
            // draw the walls
            level.Draw(_spriteBatch);

            // draw player and objects
            _spriteBatch.Draw(squareTexture, DEBUG.Position, Color.White);
            
            // draw bullets and enemies
            bulletManager.DrawBullets(_spriteBatch);
            enemyManager.DrawEnemies(_spriteBatch);

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
