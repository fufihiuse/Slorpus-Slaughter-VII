using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Slorpus
{
    //enums
    public enum GameStatej
    {
        Menu,
        Game,
        Settings,
        Pause,
        GameOver
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
        GameState prevGameState;
        MouseState prevMouseState;

        //backgrounds
        Texture2D menuBackground;
        Texture2D settingsBackground;
        Texture2D pauseBackground;
        Texture2D gameOverBackground;

        //button lists
        List<Button> menuButtons;
        List<Button> gameButtons;
        List<Button> gameOverButtons;
        List<Button> pauseButtons;
        List<Button> settingsButtons;

        //buttons
        //menu
        Button menuStart;
        Button menuSettings;
        Button menuExit;
        //settings
        Button godMode;
        Button back;
        //pause
        Button resume;
        Button pauseSettings;
        Button pauseExit;
        //gameover
        Button retry;
        Button gameOverMenu;

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
            menuBackground = content.Load<Texture2D>("menuBackground");
            settingsBackground = content.Load<Texture2D>("settingsBackground");
            pauseBackground = content.Load<Texture2D>("pauseBackground");
            gameOverBackground = content.Load<Texture2D>("gameOverBackground");


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
            //menu
            menuStart = new Button(new Rectangle(300, 225, 200, 50),
                standard, hover, active);
            menuSettings = new Button(new Rectangle(300, 295, 200, 50), 
                standard, hover, active);
            menuExit = new Button(new Rectangle(300, 365, 200, 50), 
                standard, hover, active);
            //settings
            godMode = new Button(new Rectangle(300, 295, 200, 50),
                standard, hover, active);
            back = new Button(new Rectangle(300, 365, 200, 50),
                standard, hover, active);
            //pause
            resume = new Button(new Rectangle(300, 225, 200, 50),
                standard, hover, active);
            pauseSettings = new Button(new Rectangle(300, 295, 200, 50),
                standard, hover, active);
            pauseExit = new Button(new Rectangle(300, 365, 200, 50),
                standard, hover, active);
            //gameover
            retry = new Button(new Rectangle(300, 295, 200, 50),
                standard, hover, active);
            gameOverMenu = new Button(new Rectangle(300, 365, 200, 50),
                standard, hover, active);

            //add buttons to lists
            //menu
            menuButtons.Add(menuStart);
            menuButtons.Add(menuSettings);
            menuButtons.Add(menuExit);
            //settings
            settingsButtons.Add(godMode);
            settingsButtons.Add(back);
            //pause
            pauseButtons.Add(resume);
            pauseButtons.Add(pauseSettings);
            pauseButtons.Add(pauseExit);
            //gameover
            gameOverButtons.Add(retry);
            gameOverButtons.Add(gameOverMenu);
        }
        /// <summary>
        /// updates the ui gamestate depending on which button is pressed
        /// </summary>
        public void Update(MouseState ms, KeyboardState ks)
        {
            if(prevMouseState.LeftButton != ButtonState.Pressed)
            {
                switch (currentGameState)
                {
                    case GameState.Menu:
                        //update buttons
                        if (menuStart.Update(ms))
                        {
                            currentGameState = GameState.Game;
                        }
                        if (menuSettings.Update(ms))
                        {
                            currentGameState = GameState.Settings;
                            prevGameState = GameState.Menu;
                        }
                        if (menuExit.Update(ms))
                        {
                            Environment.Exit(0);
                        }
                        break;

                    case GameState.Game:
                        if (ks.IsKeyDown(Keys.P))
                        {
                            currentGameState = GameState.Pause;
                        }
                        break;

                    case GameState.Settings:
                        if (godMode.Update(ms))
                        {

                        }
                        if (back.Update(ms))
                        {
                            currentGameState = prevGameState;
                        }
                        break;

                    case GameState.Pause:
                        if (resume.Update(ms))
                        {
                            currentGameState = GameState.Game;
                        }
                        if (pauseSettings.Update(ms))
                        {
                            currentGameState = GameState.Settings;
                            prevGameState = GameState.Menu;
                        }
                        if (pauseExit.Update(ms))
                        {
                            currentGameState = GameState.Menu;
                        }
                        break;

                    case GameState.GameOver:
                        if (retry.Update(ms))
                        {
                            currentGameState = GameState.Game;
                        }
                        if (gameOverMenu.Update(ms))
                        {
                            currentGameState = GameState.Menu;
                        }
                        break;
                }
            }
            prevMouseState = ms;
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
                        menuBackground, 
                        new Rectangle(0, 0, 800, 480), 
                        Color.White
                        );

                    //draw all menuButtons
                    foreach(Button button in menuButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Game:
                    //draw all gameButtons
                    foreach (Button button in gameButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Settings:
                    //draw background
                    sb.Draw(
                        settingsBackground,
                        new Rectangle(0, 0, 800, 480),
                        Color.White
                        );

                    //draw all settingsButtons
                    foreach (Button button in settingsButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.Pause:
                    //draw background
                    sb.Draw(
                        pauseBackground,
                        new Rectangle(0, 0, 800, 480),
                        Color.White
                        );

                    //draw all pauseButtons
                    foreach (Button button in pauseButtons)
                    {
                        button.Draw(sb);
                    }
                    break;

                case GameState.GameOver:
                    //draw background
                    sb.Draw(
                        gameOverBackground,
                        new Rectangle(0, 0, 800, 480),
                        Color.White
                        );

                    //draw all gameOverButtons
                    foreach (Button button in gameOverButtons)
                    {
                        button.Draw(sb);
                    }
                    break;
            }
        }
    }
}
