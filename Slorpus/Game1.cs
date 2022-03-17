﻿using System;
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

        // input
        MouseState prevMS;
        KeyboardState prevKB;

        // managers
        Level level;
        EnemyManager enemyManager;
        BulletManager bulletManager;
        PhysicsManager physicsManager;

        // lists
        // these (usually) should not be modified directly, edit them with the managers
        List<IPhysics> physicsList;
        List<Enemy> enemyList;
        EnemyBullet[] bulletList;
        List<Wall> wallList;
        
        // more lists, these are for special objects that subscribe to certain events
        List<IUpdate> updateList;
        List<IDraw> drawList;
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
            physicsList = new List<IPhysics>();
            enemyList = new List<Enemy>();
            wallList = new List<Wall>();
            bulletList = new EnemyBullet[100];

            prevMS = Mouse.GetState();
            prevKB = Keyboard.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            squareTexture = Content.Load<Texture2D>("square");
 
            // instantiate all the manager classes on the empty, just initialized lists
            level = new Level(wallList, squareTexture, squareTexture, squareTexture);
            LevelParser levelParser = new LevelParser();
            List<GenericEntity> levelList = level.LoadFromFile("..\\..\\..\\levels\\example.sslvl"); //Loads example level and returns entityList
            bulletManager = new BulletManager(bulletList, squareTexture);
            enemyManager = new EnemyManager(enemyList, squareTexture, bulletManager);
            physicsManager = new PhysicsManager(physicsList, wallList, enemyManager, bulletManager);
            
            // parse data read from level
            levelParser.GetEnemies(enemyList, levelList, squareTexture, squareTexture);
            levelParser.GetWalls(wallList, levelList);
            levelParser.GetPhysicsObjects(physicsList, levelList, physicsManager, squareTexture, squareTexture);

            // miscellaneous, "special" items which dont have a manager
            updateList = levelParser.Updatables;
            drawList = levelParser.Drawables;
            mouseClickList = levelParser.MouseClickables;
            keyPressList = levelParser.KeyPressables;
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

            enemyManager.UpdateEnemies(gameTime);
            physicsManager.MovePhysics(gameTime);
            // TODO: get rid of the stupid bullet size argument
            physicsManager.CollideAndMoveBullets(gameTime, new Point(Constants.BULLET_SIZE, Constants.BULLET_SIZE));
            
            base.Update(gameTime);

            // update previous keyboard state
            prevKB = Keyboard.GetState();
            prevMS = Mouse.GetState();
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
            bulletManager.DrawBullets(_spriteBatch, new Point(Constants.BULLET_SIZE,Constants.BULLET_SIZE));
            enemyManager.DrawEnemies(_spriteBatch);

            base.Draw(gameTime);
            _spriteBatch.End();
        }
    }
}
