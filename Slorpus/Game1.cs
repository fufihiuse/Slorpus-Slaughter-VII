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
        // no more underscore, I know what I'm doing >:)
        private SpriteBatch shaderBatch;
        private RenderTarget2D finalTarget;

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

        Effect CRTFilter;
        int DEBUGTIMER = 0;

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
        // things that should be recieving input events all the time
        List<IKeyPress> constantKeyPressList;
        List<IMouseClick> constantMouseClickList;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.GraphicsProfile = GraphicsProfile.HiDef;
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
            constantKeyPressList = new List<IKeyPress>();
            constantMouseClickList = new List<IMouseClick>();

            screen = new Screen(_graphics, Window);
            constantKeyPressList.Add(screen);
            
            soundEffects = new SoundEffects();
            uiManager = new UIManager();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            CRTFilter = Content.Load<Effect>("shaders/crt");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            shaderBatch = new ShaderBatch(GraphicsDevice);
            finalTarget = new RenderTarget2D(
                GraphicsDevice,
                Screen.Size.X, Screen.Size.Y,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24
                );

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
            foreach (IDraw d in drawables)
            {
                layers.Add(d);
            }
            
            // misc additions to the lists
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
            
            if (Keyboard.GetState() != prevKB)
            {
                OnKeyPress(prevKB, constantKeyPressList);
            }
            if (Mouse.GetState() != prevMS)
            {
                OnMouseClick(prevMS, constantMouseClickList);
            }
            
            // update previous keyboard state
            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();
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
                OnKeyPress(prevKB, keyPressList);
            }
            if (Mouse.GetState() != prevMS)
            {
                OnMouseClick(prevMS, mouseClickList);
            }
            
            base.Update(gameTime);

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
        private void OnMouseClick(MouseState ms, List<IMouseClick> targets)
        {
            foreach(IMouseClick mc in targets)
            {
                mc.OnMouseClick(ms);
            }
        }
        
        /// <summary>
        /// Called whenever keyboard input changes.
        /// </summary>
        /// <param name="kb">PREVIOUS state of the keyboard.</param>
        private void OnKeyPress(KeyboardState kb, List<IKeyPress> targets)
        { 
            foreach(IKeyPress kp in targets)
            {
                kp.OnKeyPress(kb);
            }
        }

        private void PreDraw()
        {
            GraphicsDevice.Clear(Color.Black);

            // draw to small render target
            GraphicsDevice.SetRenderTarget(finalTarget);

            _spriteBatch.Begin();
        }

        private void PostDraw(GameTime gameTime)
        {
            _spriteBatch.End();

            // null will cause it to draw to the screen
            GraphicsDevice.SetRenderTarget(null);

            // set up shader effect(s)
            Matrix view = Matrix.Identity;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Screen.TrueSize.X, Screen.TrueSize.Y, 0, 0, 1);

            CRTFilter.Parameters["view_projection"].SetValue(view * projection);
            DEBUGTIMER++;
            CRTFilter.Parameters["refreshline_pos"].SetValue(((float)DEBUGTIMER % 60)/60);

            // _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, effect: fullScreenShader);
            _spriteBatch.Begin(effect: CRTFilter);

            // draw the small render target to the whole screen
            _spriteBatch.Draw(
                finalTarget,
                Screen.Target,
                Color.White
                );

            _spriteBatch.End();
        }

        protected override void Draw(GameTime gameTime)
        {
            PreDraw();

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
            }
            else
            {
                uiManager.Draw(_spriteBatch);
            }

            base.Draw(gameTime);

            PostDraw(gameTime);
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
                    Constants.PLAYER_BULLET_SIZE,
                    Constants.PLAYER_BULLET_SIZE
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
