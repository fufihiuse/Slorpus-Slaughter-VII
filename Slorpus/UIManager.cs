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
        /// loads all the textures for the buttons and then passes them into  the coresponding buttons
        /// </summary>
        public void LoadButtons()
        {

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
        public void Draw()
        {
            switch (currentGameState)
            {
                case GameState.Menu:
                    break;

                case GameState.GameOver:
                    break;

                case GameState.Pause:
                    break;

                case GameState.Settings:
                    break;
            }
        }
    }
}
