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
        
        // important objects
        Camera camera;
        Screen screen;
        Action<IDestroyable> remove_object;

        // input
        MouseState prevMS;
        KeyboardState prevKB;

        // managers
        Level level;
        LevelParser levelParser;
        EnemyManager enemyManager;
        BulletManager bulletManager;
        PhysicsManager physicsManager;

        // lists
        // these (usually) should not be modified directly, edit them with the managers
        List<IPhysics> physicsList;
        List<Enemy> enemyList;
        EnemyBullet[] bulletList;
        List<Wall> wallList;
        SoundEffects soundEffects;
        Queue<IDestroyable> destroy_queue;
        
        // more lists, these are for special objects that subscribe to certain events
        List<IUpdate> updateList;
        List<IDraw> drawList;
        List<IMouseClick> mouseClickList;
        List<IKeyPress> keyPressList;

        //i need this
        Rectangle bRect;

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
            destroy_queue = new Queue<IDestroyable>();
            levelParser = new LevelParser();

            // anonymous function that is used to destroy any IDestroyable object
            remove_object = (IDestroyable destroy_target) => {
                destroy_queue.Enqueue(destroy_target);
            };

            prevMS = Mouse.GetState();
            prevKB = Keyboard.GetState();

            screen = new Screen(
                new Point(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight
                )
            );
            soundEffects = new SoundEffects();
            screen.Use();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            squareTexture = Content.Load<Texture2D>("square");

            LoadLevel("2"); 
        }

        public void LoadLevel(string levelname)
        {
            // instantiate all the manager classes on the empty, just initialized lists
            level = new Level(wallList, squareTexture, squareTexture, squareTexture);
            //LevelParser levelParser = new LevelParser();
            List<GenericEntity> levelList = level.LoadFromFile($"..\\..\\..\\levels\\{levelname}.sslvl"); //Loads example level and returns entityList
            bulletManager = new BulletManager(bulletList, squareTexture);
            enemyManager = new EnemyManager(enemyList, squareTexture, bulletManager);
            physicsManager = new PhysicsManager(physicsList, wallList, bulletManager);
            
            // bullet creation function
            Action<Point, Vector2> createbullet = (Point loc, Vector2 vel) => CreateBullet(loc, vel);
            // camera creation function
            Action<IPosition> createCamera = (IPosition player) => CreateCamera(player);
            levelParser.GetPhysicsObjects(physicsList, levelList, createbullet, createCamera, squareTexture, squareTexture);
            
            // parse data read from level
            levelParser.GetEnemies(enemyList, levelList, squareTexture, squareTexture, remove_object);
            levelParser.GetWalls(wallList, levelList);

            // miscellaneous, "special" items which dont have a manager
            updateList = levelParser.Updatables;
            drawList = levelParser.Drawables;
            mouseClickList = levelParser.MouseClickables;
            keyPressList = levelParser.KeyPressables;
            SoundEffects.AddSounds(Content);

            // add enemies to physicsobject list
            foreach (Enemy e in enemyList)
            {
                physicsList.Add(e);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (IUpdate u in updateList)
            {
                u.Update(gameTime);
            }

            // check for changes in input
            if (Keyboard.GetState() != prevKB)
            {
                OnKeyPress(prevKB);
            }
            if (Mouse.GetState() != prevMS)
            {
                OnMouseClick(prevMS);
            }
            

            enemyManager.UpdateEnemies(gameTime, levelParser._Player, bRect);
            physicsManager.MovePhysics(gameTime);
            // TODO: get rid of the stupid bullet size argument
            physicsManager.CollideAndMoveBullets(gameTime, new Point(Constants.BULLET_SIZE, Constants.BULLET_SIZE));
            camera.Update(gameTime);
            
            base.Update(gameTime);

            // update previous keyboard state
            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();

            // clean up objects that need to be destroyed
            while (destroy_queue.Count > 0)
            {
                IDestroyable destroy_target = destroy_queue.Dequeue();
                // try to remove this object from all the lists
                // with the exception of walls and enemy bullets!!!
                try
                {
                    IUpdate update_version = (IUpdate)destroy_target;
                    updateList.Remove(update_version);
                }
                catch (InvalidCastException) { /* do nothing */ }
                try
                {
                    IDraw draw_target = (IDraw)destroy_target;
                    drawList.Remove(draw_target);
                }
                catch (InvalidCastException) { /* do nothing */ }
                try
                {
                    IPhysics physics_target = (IPhysics)destroy_target;
                    physicsList.Remove(physics_target);
                }
                catch (InvalidCastException) { /* do nothing */ }
                try
                {
                    Enemy enemy_target = (Enemy)destroy_target;
                    enemyList.Remove(enemy_target);
                }
                catch (InvalidCastException) { /* do nothing */ }
            }
        }
        
        /// <summary>
        /// Called whenever mouse input state changes.
        /// </summary>
        /// <param name="ms">PREVIOUS state of the mouse.</param>
        public void OnMouseClick(MouseState ms)
        {
            foreach(IMouseClick mc in mouseClickList)
            {
                mc.OnMouseClick(ms);
            }
        }
        
        /// <summary>
        /// Called whenever keyboard input changes.
        /// </summary>
        /// <param name="kb">PREVIOUS state of the keyboard.</param>
        public void OnKeyPress(KeyboardState kb)
        { 
            foreach(IKeyPress kp in keyPressList)
            {
                kp.OnKeyPress(kb);
            }
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
            bulletManager.DrawBullets(_spriteBatch,
                new Point(
                    Constants.BULLET_SIZE,
                    Constants.BULLET_SIZE
                    )
                );
            enemyManager.DrawEnemies(_spriteBatch);

            base.Draw(gameTime);
            _spriteBatch.End();
        }

        /// <summary>
        /// Proof of concept method that creates the player bullet. Delegated to the player.
        /// </summary>
        /// <param name="location">Starting location of the bullet.</param>
        /// <param name="velocity">Starting velocity of the bullet.</param>
        public void CreateBullet(Point location, Vector2 velocity)
        {
            bRect = new Rectangle(location,
                new Point(
                    Constants.PLAYER_BULLET_SIZE,
                    Constants.PLAYER_BULLET_SIZE
                    )
                );

            PlayerProjectile bullet = new PlayerProjectile(bRect, velocity, squareTexture, remove_object);
            updateList.Add(bullet);
            drawList.Add(bullet);
            physicsList.Add(bullet);
        }

        private void CreateCamera(IPosition followTarget)
        {
            camera = new Camera(followTarget, Constants.CAMERA_SPEED);
        }
    }
}
