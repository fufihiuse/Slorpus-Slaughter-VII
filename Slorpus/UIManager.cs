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
    /// <summary>
    /// Controls UI
    /// </summary>
    public class UIManager
    {
        //fields  
        GameState currentGameState;
        Texture2D background;

        //button lists
        List<Button> menuButtons;
        List<Button> gameButtons;
        List<Button> gameOverButtons;
        List<Button> pauseButtons;
        List<Button> settingsButtons;

        //buttons
        Button gameStart;

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
        public void LoadUI(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            //background
            background = content.Load<Texture2D>("enemy");

            //testing button textures
            Texture2D standard = content.Load<Texture2D>("buttonPlaceholder");
            Texture2D active = content.Load<Texture2D>("buttonPlaceholderActive");
            Texture2D hover = content.Load<Texture2D>("buttonPlaceholderHover");

            //button lists
            menuButtons = new List<Button>();
            gameButtons = new List<Button>();
            gameOverButtons = new List<Button>();
            pauseButtons = new List<Button>();
            settingsButtons = new List<Button>();

            //make buttons
            gameStart = new Button(new Rectangle(20, 20, 20, 20),
                standard, hover, active);

            //add buttons to lists
            menuButtons.Add(gameStart);
        }
        /// <summary>
        /// updates the ui gamestate depending on which button is pressed
        /// </summary>
        public void Update(MouseState ms)
        {
            switch (currentGameState)
            {
                case GameState.Menu:
                    //update buttons
                    if (gameStart.Update(ms))
                    {
                        currentGameState = GameState.Game;
                    }
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
                    sb.Draw(
                        background, 
                        new Rectangle(0, 0, 600, 600), 
                        Color.White
                        );

                    //draw all menuButtons
                    foreach(Button button in menuButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Game:
                    //draw background

                    //draw all gameButtons
                    foreach (Button button in gameButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.GameOver:
                    //draw background

                    //draw all gameOverButtons
                    foreach (Button button in gameOverButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Pause:
                    //draw background

                    //draw all pauseButtons
                    foreach (Button button in pauseButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Settings:
                    //draw background

                    //draw all settingsButtons
                    foreach (Button button in settingsButtons)
                    {
                        button.Draw(sb);
                    }
                    break;
            }
        }
    }
}
