using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * This class is used to parse the output of Level.LoadFromFile().
     */
    class LevelParser
    {
        // if an entity implements IUpdate, it should be added to this list on load
        // we retrive it after initializing all the other lists
        List<IUpdate> updateables;
        List<IDraw> drawables;
        List<IMouseClick> mouseClickables;
        List<IKeyPress> keyPressables;
        
        
        // property just to warn if empty
        public List<IUpdate> Updatables
        {
            get {
                if (updateables.Count <= 0)
                {
                    Debugger.Log(0, "Warning", "Updateables list empty. Have you called the other entity Get methods?");
                }
                return updateables;
            }
        }
        public List<IDraw> Drawables
        {
            get {
                if (drawables.Count <= 0)
                {
                    Debugger.Log(0, "Warning", "Drawables list empty. Have you called the other entity Get methods?");
                }
                return drawables;
            }
        }
        public List<IMouseClick> MouseClickables
        {
            get {
                if (drawables.Count <= 0)
                {
                    Debugger.Log(0, "Warning", "MouseClickables list empty. Have you called the other entity Get methods?");
                }
                return mouseClickables;
            }
        }
        public List<IKeyPress> KeyPressables
        {
            get
            {
                if (keyPressables.Count <= 0)
                {
                    Debugger.Log(0, "Warning", "MouseClickables list empty. Have you called the other entity Get methods?");
                }
                return keyPressables;
            }
        }

        public LevelParser()
        {
            updateables = new List<IUpdate>();
            drawables = new List<IDraw>();
            mouseClickables = new List<IMouseClick>();
            keyPressables = new List<IKeyPress>();
        }

        /// <summary>
        /// Returns a list of walls, made based on the information parsed from entityList.
        /// </summary>
        /// <param name="walls">The wall list class who needs to have the newly loaded walls added to it.</param>
        /// <param name="entityList">Information about all the game objects that need to be created.</param>
        /// <returns>A list of all the walls in the level.</returns>
        public void GetWalls(List<Wall> walls, List<GenericEntity> entityList)
        {
            foreach (GenericEntity ge in entityList)
            {
                switch (ge.EntityType) {
                    case 'W':
                        Wall w = new Wall(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.WALL_SIZE,
                                    Constants.WALL_SIZE
                                    )
                                )
                            );

                        // add a new wall to the wall list
                        SortItem(w);
                        walls.Add(w);
                        break;
                    case 'M':
                        Wall m = new Wall(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.WALL_SIZE,
                                    Constants.WALL_SIZE
                                    )
                                ),
                            true, //is collidable
                            true //is a mirror
                            );
                        SortItem(m);
                        // add a new mirror to the wall list
                        walls.Add(m);
                        break;
                }
            }
        }

        // TODO: split mirrors into a different list so we dont have to check if things
        // are a mirror every frame
        // static List<Wall> GetMirrors(List<GenericEntity> entityList)
        
        /// <summary>
        /// Creates a set of enemies based on GenericEntities.
        /// </summary>
        /// <param name="entityList">Entities and their positions.</param>
        /// <param name="homeEnemyAsset">Asset for enemies with the homing attack.</param>
        /// <param name="enscEnemyAsset">Assets for enemies with the ensconcing attack.</param>
        /// <returns>A list of all enemies in the current level.</returns>
        public void GetEnemies(List<Enemy> enemyList, List<GenericEntity> entityList, Texture2D homeEnemyAsset, Texture2D enscEnemyAsset)
        { 
            foreach (GenericEntity ge in entityList)
            {
                switch (ge.EntityType) {
                    case 'E':
                        Enemy e = new Enemy(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.ENEMY_SIZE,
                                    Constants.ENEMY_SIZE)
                                ),
                            Vector2.Zero,
                            enscEnemyAsset,
                            ShootingPattern.Ensconcing);
                        SortItem(e);
                        enemyList.Add(e);
                        break;

                    case 'H':
                        Enemy h = new Enemy(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.ENEMY_SIZE,
                                    Constants.ENEMY_SIZE)
                                ),
                            Vector2.Zero,
                            homeEnemyAsset,
                            ShootingPattern.HomingAttack);
                        SortItem(h);
                        enemyList.Add(h);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Currently returns a list containing only the player.
        /// </summary>
        /// <param name="entityList">List of entities in the level</param>
        /// <param name="physicsManager">Physics Manager.</param>
        /// <param name="playerAsset">The player's Texture2D</param>
        /// <param name="playerBulletAsset">The Texture2D used by the bullet the player fires.</param>
        /// <returns>A list containing the player and any other physics objects loaded in from the level.</returns>
        public void GetPhysicsObjects(List<IPhysics> physicsObjects, List<GenericEntity> entityList, PhysicsManager physicsManager, Texture2D playerAsset, Texture2D playerBulletAsset)
        {
            foreach (GenericEntity ge in entityList)
            {
                if (ge.EntityType == 'P')
                {
                    Player player = new Player(
                        new Rectangle(
                            ge.Position,
                            new Point(Constants.PLAYER_SIZE, Constants.PLAYER_SIZE)
                            ),
                        Vector2.Zero,
                        physicsManager,
                        playerAsset,
                        playerBulletAsset
                        );

                    SortItem(player);
                    physicsObjects.Add(player);
                }
            }
        }

        // helper method that tries to add an object to all its matching event subscription lists
        private void SortItem(Object t)
        {
            try
            {
                IDraw temp = (IDraw)t;
                drawables.Add(temp);
            } catch { }
            try
            {
                IUpdate temp = (IUpdate)t;
                updateables.Add(temp);
            } catch { }
            try
            {
                IMouseClick temp = (IMouseClick)t;
                mouseClickables.Add(temp);
            } catch { }
            try
            {
                IKeyPress temp = (IKeyPress)t;
                keyPressables.Add(temp);
            } catch { }
        }
    }
}
