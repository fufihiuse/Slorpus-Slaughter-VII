using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_Textbox;

using Slorpus.Objects;
using Slorpus.Statics;

namespace Slorpus.Managers
{
    //enums
    public enum GameState
    {
        Menu,
        Game,
        Settings,
        CustomLevel,
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
        // big bad public static
        public static bool IsGodModeOn { get { return isGodModeOn; } }
        private static bool isGodModeOn = false;

        //fields  
        GameState currentGameState;
        GameState prevGameState;
        GraphicsDevice GraphicsDevice;
        Game1 Game1;

        //Bool to control checkbox draw
        private bool loadedCustom = false;

        //backgrounds
        Texture2D menuBackground;
        Texture2D settingsBackground;
        Texture2D pauseBackground;
        Texture2D gameOverBackground;
        Texture2D customLevelBackground;

        //Check Texture
        Texture2D check;

        //TextBox
        TextBox textBox;
        string inputtedText;

        //button lists
        List<Button> menuButtons;
        List<Button> gameButtons;
        List<Button> gameOverButtons;
        List<Button> pauseButtons;
        List<Button> settingsButtons;
        List<Button> customLevelButtons;

        //buttons
        //menu
        Button menuStart;
        Button menuSettings;
        Button menuExit;
        //settings
        Button godMode;
        Button back;
        Button customLvl;
        //pause
        Button resume;
        Button pauseSettings;
        Button pauseExit;
        //gameover
        Button retry;
        Button gameOverMenu;
        //custom level loader
        Button loadLevel;

        //properties
        public GameState CurrentGameState
        {
            get
            {
                return currentGameState;
            }
        }

        //constructor
        public UIManager(GraphicsDevice GraphicsDevice, Game1 Game1)
        {
            this.Game1 = Game1;
            this.GraphicsDevice = GraphicsDevice;
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
            customLevelBackground = content.Load<Texture2D>("customLevelBackground");

            //Text Input
            textBox = new TextBox(
                new Rectangle(300, 295, 200, 50),
                11,
                String.Empty,
                GraphicsDevice,
                Game1.NotoSans,
                Color.Black,
                Color.Red,
                60);
            textBox.Renderer.Color = new Color(0.63529411764f, 0.09411764705f, 0.09411764705f);
            textBox.Active = false;
            KeyboardInput.Initialize(Game1, 100f, 60);
            //textBox.Area = new Rectangle(300, 295, 200, 50);
            check = content.Load<Texture2D>("check");


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
            customLevelButtons = new List<Button>();

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
            customLvl = new Button(new Rectangle(300, 225, 200, 50),
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
            //custom level loader
            loadLevel = new Button(new Rectangle(521, 295, 50, 50),
                standard, hover, active);

            //add buttons to lists
            //menu
            menuButtons.Add(menuStart);
            menuButtons.Add(menuSettings);
            menuButtons.Add(menuExit);
            //settings
            settingsButtons.Add(godMode);
            settingsButtons.Add(customLvl);
            settingsButtons.Add(back);
            //pause
            pauseButtons.Add(resume);
            pauseButtons.Add(pauseSettings);
            pauseButtons.Add(pauseExit);
            //gameover
            gameOverButtons.Add(retry);
            gameOverButtons.Add(gameOverMenu);
            //custom level loader
            customLevelButtons.Add(loadLevel);
            customLevelButtons.Add(back);
            
        }
        /// <summary>
        /// updates the ui gamestate depending on which button is pressed
        /// </summary>
        public void Update(MouseState ms, KeyboardState ks)
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
                    if (ks.IsKeyDown(Keys.Escape))
                    {
                        currentGameState = GameState.Pause;
                    }
                    break;

                case GameState.Settings:
                    if (godMode.Update(ms))
                    {
                        isGodModeOn = !isGodModeOn;
                    }
                    if (customLvl.Update(ms))
                    {
                        currentGameState = GameState.CustomLevel;
                        prevGameState = GameState.Settings;
                        textBox.Active = true;
                    }
                    if (back.Update(ms))
                    {
                        currentGameState = prevGameState;
                    }
                    break;

                case GameState.CustomLevel:
                    KeyboardInput.Update();
                    textBox.Update();
                    if (godMode.Update(ms))
                    {
                        isGodModeOn = !isGodModeOn;
                    }
                    if (loadLevel.Update(ms))
                    {
                        inputtedText = textBox.Text.String;
                        //Attempt to load custom level
                        try 
                        { 
                            LevelInfo.LoadCustomLevel(inputtedText); //TODO: add custom input, look for pre-written library please oh god https://github.com/UnterrainerInformatik/Monogame-Textbox
                            LevelInfo.ReloadLevel();
                            loadedCustom = true;
                        }
                        catch (Exception) { Console.WriteLine("FAIL!"); loadedCustom = false; } //TODO: change to draw on screen
                        textBox.Clear();
                    }
                    if (back.Update(ms))
                    {
                        currentGameState = prevGameState;
                        prevGameState = GameState.Menu;
                        textBox.Active = false;
                        loadedCustom = false;
                        textBox.Clear();
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
                        LevelInfo.ReloadLevel();
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
                    if (isGodModeOn)
                    {
                        sb.DrawString(Game1.TestingFont, "GODMODE ENABLED", new Vector2(5, 0), Color.Red);
                    }
                    break;

                case GameState.CustomLevel:
                    //draw background
                    sb.Draw(
                        customLevelBackground,
                        new Rectangle(0, 0, 800, 480),
                        Color.White
                        );

                    //draw all settingsButtons
                    foreach (Button button in customLevelButtons)
                    {
                        button.Draw(sb);
                    }

                    //Draw text box
                    textBox.Draw(sb);

                    //Draw check box
                    sb.Draw(
                        check,
                        new Rectangle(522, 295, 48, 48),
                        Color.White
                        );

                    if (loadedCustom) 
                    {
                        sb.DrawString(Game1.TestingFont, "Level loaded successfully!", new Vector2(5, 0), Color.Red);
                    }
                    else
                    {
                        sb.DrawString(Game1.TestingFont, "No custom level loaded", new Vector2(5, 0), Color.Red);
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
