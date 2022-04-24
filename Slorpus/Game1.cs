using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Slorpus.Utils;
using Slorpus.Managers;
using Slorpus.Objects;
using Slorpus.Statics;
using Slorpus.Interfaces;
using Slorpus.Interfaces.Base;

namespace Slorpus
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // debug assets for use everywhere
        private static Texture2D squareTexture;
        public static Texture2D SquareTexture { get { return squareTexture; } }
        private static SpriteFont testingFont;
        public static SpriteFont TestingFont { get { return testingFont; } }

        // important misc objects
        Camera camera;
        Screen screen;
        LevelInfo _levelInfo;
        Dereferencer _dereferencer;
        Layers layers;

        // input
        MouseState prevMS;
        KeyboardState prevKB;

        // managers
        Level level;
        BulletManager bulletManager;
        PhysicsManager physicsManager;
        UIManager uiManager;

        // lists
        // these (usually) should not be modified directly, edit them with the managers
        List<IPhysics> physicsList;
        EnemyBullet[] bulletList;
        List<Wall> wallList;
        SoundEffects soundEffects;
        Queue<IDestroyable> destroy_queue;
        
        // more lists, these are for special objects that subscribe to certain events
        List<IUpdate> updateList;
        List<IMouseClick> mouseClickList;
        List<IKeyPress> keyPressList;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            destroy_queue = new Queue<IDestroyable>();
            _levelInfo = new LevelInfo(this);
            _dereferencer = new Dereferencer(destroy_queue);

            // anonymous function that is used to destroy any IDestroyable object

            prevMS = Mouse.GetState();
            prevKB = Keyboard.GetState();

            screen = new Screen(
                new Point(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight
                )
            );
            screen.Use();
            
            soundEffects = new SoundEffects();
            uiManager = new UIManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            squareTexture = Content.Load<Texture2D>("square");
            testingFont = Content.Load<SpriteFont>("Arial12");
            
            // load UI
            uiManager.LoadUI(Content);
            
            // load sound effects
            SoundEffects.AddSounds(Content);
            
            // load first level
            LoadLevel(Constants.LEVELS[0]); 
        }

        public void LoadLevel(string levelname)
        {
            ResetLists();
            layers = new Layers();
            
            // read the level out of a file
            level = new Level(wallList, Content);
            List<GenericEntity> levelList = level.LoadFromFile($"..\\..\\..\\levels\\{levelname}.sslvl");
            
            // create managers and utils
            bulletManager = new BulletManager(bulletList, squareTexture);
            physicsManager = new PhysicsManager(physicsList, wallList, bulletManager);
            LevelParser levelParser = new LevelParser(Content);
            
            // bullet creation function
            Action<Point, Vector2> createbullet = (Point loc, Vector2 vel) => CreateBullet(loc, vel);
            
            // parse data read from level (player requires the bullet creation func)
            levelParser.GetWalls(wallList, levelList);
            levelParser.GetPhysicsObjects(physicsList, levelList, createbullet);
            
            // instantiate camera
            if (Player.Position != null)
            {
                // function to retrieve the camera's target coordinates
                Func<Rectangle> getFollowTarget = () => { return Player.Position; };
                // create camera
                camera = new Camera(getFollowTarget, Constants.CAMERA_SPEED);
            }
            else
            {
                throw new Exception("A player is needed to instantiate the player camera, " +
                    "but no player was created on this level.");
            }
            
            // miscellaneous, "special" items which dont have a manager
            updateList = levelParser.Updatables;
            mouseClickList = levelParser.MouseClickables;
            keyPressList = levelParser.KeyPressables;
            
            // handle different draw layers
            List<IDraw> drawables = levelParser.Drawables;
            // sort all the drawables into their respective layers
            _levelInfo.initialEnemyCount = 0;
            foreach (IDraw d in drawables)
            {
                layers.Add(d);
                // also get enemy count since we're iterating anyway
                if (d is Enemy)
                {
                    _levelInfo.initialEnemyCount += 1;
                }
            }

            // also add the bullets and level to be drawn
            layers.Add(bulletManager);
            layers.Add(level);

            // add camera and physics to be updated
            updateList.Add(camera);
            updateList.Add(physicsManager);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState kb = Keyboard.GetState();
            MouseState ms = Mouse.GetState();
            if (prevMS.LeftButton != ButtonState.Pressed || uiManager.CurrentGameState == GameState.Game)
            {
                //only update the game if the gamestate is game
                if (uiManager.CurrentGameState == GameState.Game)
                {
                    GameUpdate(gameTime);
                }
                uiManager.Update(ms, kb);
            }
            prevMS = ms;
        }

        protected void GameUpdate(GameTime gameTime)
        {

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
            
            
            base.Update(gameTime);

            // update previous keyboard state
            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();

            // clean up objects that need to be destroyed
            Cleanup();
        }
        
        /// <summary>
        /// Removes references to all objects in the Game1 destroy queue.
        /// </summary>
        private void Cleanup()
        {
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
                    layers.Remove(draw_target);
                }
                catch (InvalidCastException) { /* do nothing */ }
                try
                {
                    IPhysics physics_target = (IPhysics)destroy_target;
                    physicsList.Remove(physics_target);
                    if (physics_target is PlayerProjectile)
                    {
                        if (!UIManager.IsGodModeOn && Enemy.Count > 0)
                        {
                            // FAIL STATE / LOSE CONDITION
                            LevelInfo.ReloadLevel();
                        }
                    }
                    else if (physics_target is Enemy)
                    {
                        if (Enemy.Count <= 0)
                        {
                            LevelInfo.LoadNextLevel();
                        }
                    }
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

            //draw ui or game
            if(uiManager.CurrentGameState == GameState.Game)
            {
                // draw player and objects
                foreach (List<IDraw> drawList in layers)
                {
                    foreach (IDraw d in drawList)
                    {
                        d.Draw(_spriteBatch);
                    }
                }
                _spriteBatch.DrawString(testingFont, $"{Enemy.Count}", new Vector2(50, 50), Color.White);
                _spriteBatch.DrawString(testingFont, $"{LevelInfo.InitialEnemyCount}", new Vector2(50, 60), Color.White);
            }
            else
            {
                uiManager.Draw(_spriteBatch);
                /*
                _spriteBatch.DrawString(
                    testingFont, 
                    "Width: " + _graphics.PreferredBackBufferWidth + " Height" + _graphics.PreferredBackBufferHeight, 
                    new Vector2(0,0), 
                    Color.Black
                    );*/
            }

            base.Draw(gameTime);
            _spriteBatch.End();
        }

        /// <summary>
        /// Proof of concept method that creates the player bullet. Delegated to the player.
        /// </summary>
        /// <param name="location">Starting location of the bullet.</param>
        /// <param name="velocity">Starting velocity of the bullet.</param>
        private void CreateBullet(Point location, Vector2 velocity)
        {
            Rectangle pRect = new Rectangle(location,
                new Point(
                    Constants.PLAYER.BULLET_SIZE,
                    Constants.PLAYER.BULLET_SIZE
                    )
                );

            PlayerProjectile projectile = new PlayerProjectile(pRect, velocity, Content);
            updateList.Add(projectile);
            layers.Add(projectile);
            physicsList.Add(projectile);
        }
        
        /// <summary>
        /// Completely reset the contents of the physics list, wall list, and bullet list.
        /// </summary>
        private void ResetLists()
        {
            // reset the number of enemies by actually destroying each one
            if (physicsList != null)
            {
                foreach (IPhysics p in physicsList)
                {
                    try
                    {
                        Enemy temp = (Enemy)p;
                        temp.Destroy();
                    }
                    catch (InvalidCastException) { }
                }
            }
            physicsList = new List<IPhysics>();
            wallList = new List<Wall>();
            bulletList = new EnemyBullet[100];
        }
    }
}
