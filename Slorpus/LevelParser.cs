using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    /*
     * This class is used to parse the output of Level.LoadFromFile().
     *
     */
    static class LevelParser
    {
        /// <summary>
        /// Returns a list of walls, made based on the information parsed from entityList.
        /// </summary>
        /// <param name="entityList">Information about all the game objects that need to be created.</param>
        /// <returns>A list of all the walls in the level.</returns>
        public static List<Wall> GetWalls(List<GenericEntity> entityList)
        {
            List<Wall> final = new List<Wall>();
            foreach (GenericEntity ge in entityList)
            {
                switch (ge.EntityType) {
                    case 'W':
                        // add a new wall to the wall list
                        final.Add(new Wall(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.WALL_SIZE,
                                    Constants.WALL_SIZE
                                    )
                                )
                            )
                        );
                        break;
                    case 'M':
                        // add a new mirror to the wall list
                        final.Add(new Wall(
                            new Rectangle(
                                ge.Position,
                                new Point(
                                    Constants.WALL_SIZE,
                                    Constants.WALL_SIZE
                                    )
                                ),
                            true, //is collidable
                            true //is a mirror
                            )
                        );
                        break;
                }
            }

            // return the constructed set of walls present in the entity list
            return final;
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
        public static List<Enemy> GetEnemies(List<GenericEntity> entityList, Texture2D homeEnemyAsset, Texture2D enscEnemyAsset)
        { 
            List<Enemy> final = new List<Enemy>();
            foreach (GenericEntity ge in entityList)
            {
                switch (ge.EntityType) {
                    case 'E':
                        final.Add(
                            new Enemy(
                                new Rectangle(
                                    ge.Position,
                                    new Point(
                                        Constants.ENEMY_SIZE,
                                        Constants.ENEMY_SIZE)
                                    ),
                                Vector2.Zero,
                                enscEnemyAsset,
                                ShootingPattern.Ensconcing));
                        break;

                    case 'H':
                        final.Add(
                            new Enemy(
                                new Rectangle(
                                    ge.Position,
                                    new Point(
                                        Constants.ENEMY_SIZE,
                                        Constants.ENEMY_SIZE)
                                    ),
                                Vector2.Zero,
                                homeEnemyAsset,
                                ShootingPattern.HomingAttack));
                        break;
                }
            }
            return final;
        }
        
        /// <summary>
        /// Currently returns a list containing only the player.
        /// </summary>
        /// <param name="entityList">List of entities in the level</param>
        /// <param name="physicsManager">Physics Manager.</param>
        /// <param name="playerAsset">The player's Texture2D</param>
        /// <param name="playerBulletAsset">The Texture2D used by the bullet the player fires.</param>
        /// <returns>A list containing the player and any other physics objects loaded in from the level.</returns>
        public static List<IPhysics> GetPhysicsObjects(List<GenericEntity> entityList, PhysicsManager physicsManager, Texture2D playerAsset, Texture2D playerBulletAsset)
        {
            List<IPhysics> final = new List<IPhysics>();
            foreach (GenericEntity ge in entityList)
            {
                if (ge.EntityType == 'P')
                {
                    final.Add(new Player(
                        new Rectangle(
                            ge.Position,
                            new Point(Constants.PLAYER_SIZE, Constants.PLAYER_SIZE)
                            ),
                        Vector2.Zero,
                        physicsManager,
                        playerAsset,
                        playerBulletAsset
                        )
                    );
                }
            }

            return final;
        }
    }
}
