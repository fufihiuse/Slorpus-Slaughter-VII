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

        // input
        MouseState prevMS;
        KeyboardState prevKB;

        // managers
        Level level;
        EnemyManager enemyManager;
        BulletManager bulletManager;
        PhysicsManager physicsManager;

        // debug object
        PhysicsObject DEBUG;
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
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            squareTexture = Content.Load<Texture2D>("square");

            LoadLevel("aynrand"); 
        }

            DEBUG = new PhysicsObject(
                new Rectangle(
                    // position
                    new Point(200, 200),
                    // size
                    new Point(16, 16)),
                new Vector2(0, 0));

            // parse data read from level
            levelParser.GetEnemies(enemyList, levelList, squareTexture, squareTexture);
            levelParser.GetWalls(wallList, levelList);
            
            // bullet creation function
            Action<Point, Vector2> createbullet = (Point loc, Vector2 vel) => CreateBullet(loc, vel);
            // camera creation function
            Action<IPosition> createCamera = (IPosition player) => CreateCamera(player);
            levelParser.GetPhysicsObjects(physicsList, levelList, createbullet, createCamera, squareTexture, squareTexture);

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
            Rectangle bRect = new Rectangle(location,
                new Point(
                    Constants.PLAYER_BULLET_SIZE,
                    Constants.PLAYER_BULLET_SIZE
                    )
                );
            PlayerProjectile bullet = new PlayerProjectile(bRect, velocity, squareTexture);
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
