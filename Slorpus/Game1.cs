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
        private KeyboardState kb;

        // managers
        Level level;
        EnemyManager enemyManager;
        BulletManager bulletManager;
        PhysicsManager physicsManager;

        // lists
        // these (usually) should not be modified directly, edit them with the managers
        List<IPhysics> physicsList;
        List<Enemy> enemyList;
        List<GenericEntity> entityList;
        EnemyBullet[] bulletList;
        List<Wall> wallList;

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
            wallList = new List<Wall>();
            bulletList = new EnemyBullet[100];

            // TODO: properly reallocate space instead of just having a static large array
            kb = new KeyboardState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            squareTexture = Content.Load<Texture2D>("square");
 
            // instantiate all the manager classes on the empty, just initialized lists
            level = new Level(wallList, squareTexture, squareTexture, squareTexture);
            entityList = level.LoadFromFile("..\\..\\..\\levels\\example.sslvl", entityList); //Loads example level and returns entityList
            bulletManager = new BulletManager(bulletList, squareTexture);
            enemyManager = new EnemyManager(enemyList, squareTexture, bulletManager);
            physicsManager = new PhysicsManager(physicsList, wallList, enemyManager, bulletManager);
            
            // parse data read from level
            enemyList = LevelParser.GetEnemies(entityList, squareTexture, squareTexture);
            wallList = LevelParser.GetWalls(entityList);
            physicsList = LevelParser.GetPhysicsObjects(entityList, physicsManager, squareTexture, squareTexture);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState kb = Keyboard.GetState();

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
            
            // draw bullets and enemies
            bulletManager.DrawBullets(_spriteBatch, new Point(5,5));
            enemyManager.DrawEnemies(_spriteBatch);

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
