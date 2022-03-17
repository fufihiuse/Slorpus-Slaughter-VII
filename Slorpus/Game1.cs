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

        // lists
        // these (usually) should not be modified directly, edit them with the managers
        List<IPhysics> physicsList;
        List<IUpdate> updateList;
        List<IDraw> drawList;
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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            squareTexture = Content.Load<Texture2D>("square");
 
            // instantiate all the manager classes on the empty, just initialized lists
            level = new Level(wallList, squareTexture, squareTexture, squareTexture);
            LevelParser levelParser = new LevelParser();
            entityList = level.LoadFromFile("..\\..\\..\\levels\\example.sslvl", entityList); //Loads example level and returns entityList
            bulletManager = new BulletManager(bulletList, squareTexture);
            enemyManager = new EnemyManager(enemyList, squareTexture, bulletManager);
            physicsManager = new PhysicsManager(physicsList, wallList, enemyManager, bulletManager);
            
            // parse data read from level
            enemyList = levelParser.GetEnemies(entityList, squareTexture, squareTexture);
            wallList = levelParser.GetWalls(entityList);
            physicsList = levelParser.GetPhysicsObjects(entityList, physicsManager, squareTexture, squareTexture);

            // miscellaneous, "special" items which dont have a manager
            updateList = levelParser.Updatables;
            drawList = levelParser.Drawables;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (IUpdate u in updateList)
            {
                u.Update(gameTime);
            }

            enemyManager.UpdateEnemies(gameTime);
            physicsManager.MovePhysics(gameTime);
            // TODO: get rid of the stupid bullet size argument
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
            foreach (IDraw d in drawList)
            {
                d.Draw(_spriteBatch);
            }
            
            // draw bullets and enemies
            bulletManager.DrawBullets(_spriteBatch, new Point(5,5));
            enemyManager.DrawEnemies(_spriteBatch);

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
