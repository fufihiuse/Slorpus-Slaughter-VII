using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    //enums
    public enum GameState
    {
        Menu,
        Game,
        GameOver,
        Pause,
        Settings
    }
    public enum ButtonCondition
    {
        Standard,
        Hover,
        Active
    }
    public class UIManager
    {
        //fields
        GameState currentGameState;
        Texture2D background;

        //buttons
        List<Button> menuButtons;
        List<Button> gameButtons;
        List<Button> gameOverButtons;
        List<Button> pauseButtons;
        List<Button> settingsButtons;

        //properties
        public GameState CurrentGameState
        {
            get
            {
                return currentGameState;
            }
        }

        //constructor
        public UIManager()
        {
            currentGameState = GameState.Menu;
        }

        //methods
        /// <summary>
        /// loads all the textures for the buttons and then passes them into  the coresponding buttons including background
        /// </summary>
        public void LoadUI()
        {
            //background

            //buttons
            menuButtons = new List<Button>();
            gameButtons = new List<Button>();
            gameOverButtons = new List<Button>();
            pauseButtons = new List<Button>();
            settingsButtons = new List<Button>();
        }
        /// <summary>
        /// updates the ui gamestate depending on which button is pressed
        /// </summary>
        public void Update(MouseState ms)
        {
            switch (currentGameState)
            {
                case GameState.Menu:
                    break;

                case GameState.Game:
                    break;

                case GameState.GameOver:
                    break;

                case GameState.Pause:
                    break;

                case GameState.Settings:
                    break;
            }
        }
        /// <summary>
        /// draws all UI
        /// </summary>
        public void Draw(SpriteBatch sb)
        {
            switch (currentGameState)
            {
                case GameState.Menu:
                    //draw background

                    //draw all menuButtons

                    break;

                case GameState.Game:
                    //draw background

                    //draw all gameButtons

                    break;

                case GameState.GameOver:
                    //draw background

                    //draw all gameOverButtons

                    break;

                case GameState.Pause:
                    //draw background

                    //draw all pauseButtons

                    break;

                case GameState.Settings:
                    //draw background

                    //draw all settingsButtons

                    break;
            }
        }
    }
}
