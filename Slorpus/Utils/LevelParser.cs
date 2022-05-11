using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using Slorpus.Objects;
using Slorpus.Statics;
using Slorpus.Utils;
using Slorpus.Interfaces;
using Slorpus.Interfaces.Base;

namespace Slorpus.Utils
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
        
        // for passing to objects to load their textures
        ContentManager content;

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

        public LevelParser(ContentManager content)
        {
            this.content = content;
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
        public void GetWalls(List<Wall> walls, List<Wall> floors, List<GenericEntity> entityList)
        {
            foreach (GenericEntity ge in entityList)
            {
                switch (ge.EntityType) {
                    case '0':
                        Wall f = new Wall(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.WALL_SIZE,
                                    Constants.WALL_SIZE
                                    )
                                ),
                            Autotiler.GetWallTileSimple(Autotiler.GetTileIndex(ge.OriginPosition.X, ge.OriginPosition.Y, '0'))
                            );

                        // add a new wall to the wall list
                        floors.Add(f);
                        break;
                    case 'W':
                        Wall w = new Wall(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.WALL_SIZE,
                                    Constants.WALL_SIZE
                                    )
                                ),
                            Autotiler.GetWallTileSimple(Autotiler.GetTileIndex(ge.OriginPosition.X, ge.OriginPosition.Y))
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
                            Autotiler.GetWallTileSimple(Autotiler.GetTileIndex(ge.OriginPosition.X, ge.OriginPosition.Y)),
                            true, //is collidable
                            true //is a mirror
                            );
                        SortItem(m);
                        // add a new mirror to the wall list
                        walls.Add(m);
                        break;
                    case 'B':
                        Wall b = new Wall(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.WALL_SIZE,
                                    Constants.WALL_SIZE
                                    )
                                ),
                            Autotiler.GetWallTileSimple(Autotiler.GetTileIndex(ge.OriginPosition.X, ge.OriginPosition.Y)),
                            false, //is collidable
                            false //is a mirror
                            );
                        SortItem(b);
                        // add a new mirror to the wall list
                        walls.Add(b);
                        break;
                }
                if (ge.EntityType == 'P' || ge.EntityType == 'E' || ge.EntityType == 'H')
                {
                    Wall f = new Wall(
                        new Rectangle(
                            ge.Position,
                            new Point(
                                Constants.WALL_SIZE,
                                Constants.WALL_SIZE
                                )
                            ),
                        Autotiler.GetWallTileSimple(Autotiler.GetTileIndex(ge.OriginPosition.X, ge.OriginPosition.Y, '0'))
                        );

                    // add a new wall to the wall list
                    floors.Add(f);
                }
            }
        }
        
        /// <summary>
        /// Fills out the physicsObjects list with any generic physics objects
        /// </summary>
        /// <param name="physicsObjects"></param>
        /// <param name="entityList"></param>
        /// <param name="bulletCreationFunc"></param>
        /// <param name="cameraCreationFunc"></param>
        public void GetPhysicsObjects(
            List<IPhysics> physicsObjects,
            List<GenericEntity> entityList,
            Action<Point, Vector2> bulletCreationFunc)
        {
            foreach (GenericEntity ge in entityList)
            {
                switch (ge.EntityType)
                {
                    case 'P':
                        Player player = new Player(
                            new Rectangle(
                                ge.Position,
                                new Point(Constants.PLAYER.SIZE, Constants.PLAYER.SIZE)
                                ),
                            content,
                            bulletCreationFunc
                            );
                        SortItem(player);
                        physicsObjects.Add(player);
                        break;
                    case 'E':
                        Enemy e = new Enemy(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.ENEMY_SIZE,
                                    Constants.ENEMY_SIZE)
                                ),
                            content,
                            ShootingPattern.Ensconcing);
                        SortItem(e);
                        physicsObjects.Add(e);
                        break;

                    case 'H':
                        Enemy h = new Enemy(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.ENEMY_SIZE,
                                    Constants.ENEMY_SIZE)
                                ),
                            content,
                            ShootingPattern.HomingAttack);
                        SortItem(h);
                        physicsObjects.Add(h);
                        break;
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
