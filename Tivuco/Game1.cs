using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Tivuco
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static Configurations configurations;

        //Mouse states used to track Mouse button press
        //public static MouseState currentMouseState;
        //public static MouseState previousMouseState;

        // Represents the player
        //Player player;
        public static List<Player> players;

        Texture2D background;
        Rectangle backgroundRectangle;

        public static List<Tile> tiles = new List<Tile>();

        public static int screenWidth;
        public static int screenHeight;

        Inputs keyboardOne;
        Inputs keyboardTwo;
        Inputs gamePadOne;
        Inputs gamePadTwo;
        Inputs gamePadThree;
        Inputs gamePadFour;

        public static bool keyboardOneTaken;
        public static bool keyboardTwoTaken;
        public static bool gamePadOneTaken;
        public static bool gamePadTwoTaken;
        public static bool gamePadThreeTaken;
        public static bool gamePadFourTaken;

        int maxNumberOfPlayers;
        public static int numberOfPlayersSelected;
        public static int deadPlayers;

        bool readyToLoadMatch;

        List<Selection> selections;
        Vector2[] startingPositions;

        public static Result result;

        //int elapsedTime;

        public static SpriteFont font18;
        public static SpriteFont font24;
        public static SpriteFont font36;

        public static Screen screen;

        public enum Screen { Menu, Selection, Match, MatchStatistics, Result }

        Button playButton;
        Button optionsButton;
        Button quitButton;

        public MenuButtons menuButtons;

        public enum MenuButtons { Play, Options, Quit }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            configurations = new Configurations(false, false, 1f);

            graphics.IsFullScreen = configurations.fullScreen;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            players = new List<Player>();
            //elapsedTime = 0;
            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            backgroundRectangle = new Rectangle(0, 0, screenWidth, screenHeight);

            keyboardOneTaken = false;
            keyboardTwoTaken = false;
            gamePadOneTaken = false;
            gamePadTwoTaken = false;
            gamePadThreeTaken = false;
            gamePadFourTaken = false;
            readyToLoadMatch = false;

            keyboardOne = new KeyboardInputs(KeyboardInputs.KeyboardIndex.One);
            keyboardTwo = new KeyboardInputs(KeyboardInputs.KeyboardIndex.Two);
            gamePadOne = new GamePadInputs(PlayerIndex.One);
            gamePadTwo = new GamePadInputs(PlayerIndex.Two);
            gamePadThree = new GamePadInputs(PlayerIndex.Three);
            gamePadFour = new GamePadInputs(PlayerIndex.Four);

            selections = new List<Selection>();
            startingPositions = new Vector2[4];

            maxNumberOfPlayers = 0;
            numberOfPlayersSelected = 0;
            deadPlayers = 0;

            screen = Screen.Menu;
            menuButtons = MenuButtons.Play;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SoundsLibrary.LoadSoundLibrary(Content);

            LoadFonts();

            LoadMenu();

            LoadSelectionScreen();

            SoundsLibrary.PlaySongs(SoundsLibrary.menuSong);
        }

        public void LoadFonts()
        {
            font18 = Content.Load<SpriteFont>("Fonts/ThinPixel18");
            font24 = Content.Load<SpriteFont>("Fonts/ThinPixel24");
            font36 = Content.Load<SpriteFont>("Fonts/ThinPixel36");
        }

        public void LoadMenu()
        {
            Texture2D button = Content.Load<Texture2D>("Graphics\\button");

            int maxNumberOfButtons = Enum.GetNames(typeof(MenuButtons)).Length;

            int buttonWidth = 146;
            int buttonHeight = 50;

            int startingY = (screenHeight / 2) - (((buttonHeight + 20) * (maxNumberOfButtons - 1)) / 2);

            for (int i = 0; i < maxNumberOfButtons; i++)
            {
                switch ((MenuButtons)i)
                {
                    case MenuButtons.Play:
                        playButton = new Button(button, buttonWidth, buttonHeight, (screenWidth / 2), startingY,
                                                    font36, "Play", true);
                        break;

                    case MenuButtons.Options:
                        optionsButton = new Button(button, buttonWidth, buttonHeight, (screenWidth / 2), startingY,
                                                    font36, "Options", false);
                        break;

                    case MenuButtons.Quit:
                        quitButton = new Button(button, buttonWidth, buttonHeight, (screenWidth / 2), startingY,
                                                    font36, "Quit", false);
                        break;
                }

                startingY += 50 + 20;
            }

            background = Content.Load<Texture2D>("Graphics/boy_underneath_a_tree");
        }

        public void LoadSelectionScreen()
        {
            Texture2D selectionBox = Content.Load<Texture2D>("Graphics\\character_selection_box");
            Texture2D selectionCharacters = Content.Load<Texture2D>("Graphics\\character_selection");

            int maxNumberOfBoxes = 2;

            if (GamePadIsConnected(PlayerIndex.One))
            {
                maxNumberOfBoxes++;

                if (GamePadIsConnected(PlayerIndex.Two))
                {
                    maxNumberOfBoxes++;
                }
            }

            int startingX = (screenWidth / 2) - (((150 + 20) * maxNumberOfBoxes) / 2);

            startingPositions[0] = new Vector2(30, 310);
            startingPositions[1] = new Vector2(Game1.screenWidth - 30, 310);
            startingPositions[2] = new Vector2(30, 110);
            startingPositions[3] = new Vector2(Game1.screenWidth - 30, 110);

            for (int i = 0; i < maxNumberOfBoxes; i++)
            {
                Rectangle destination = new Rectangle(startingX, (screenHeight / 2) - (260 / 2), 150, 260);

                Selection selectionScreen;

                if (IsOdd(i))
                {
                    selectionScreen = new Selection(selectionBox, selectionCharacters, 150, 157, destination, startingPositions[i], Player.Side.Left, "Player " + (i + 1));
                }
                else
                {
                    selectionScreen = new Selection(selectionBox, selectionCharacters, 150, 157, destination, startingPositions[i], Player.Side.Right, "Player " + (i + 1));
                }

                startingX += 150 + 20;

                selections.Add(selectionScreen);
            }
        }

        //protected override void BeginRun(SoundEffect backgroundSound)
        //{
        //    // I created an instance here but you should keep track of this variable
        //    // in order to stop it when you want.
        //    var backSong = backgroundSound.CreateInstance();
        //    backSong.IsLooped = true;
        //    backSong.Play();
        //    backSong.Volume = 0.2f;

        //    base.BeginRun();
        //}

        public static bool IsOdd(int value)
        {
            return value % 2 != 0;
        }

        public bool GamePadIsConnected(PlayerIndex index)
        {
            GamePadState gamePad = GamePad.GetState(index);

            if (gamePad.IsConnected)
            {
                return true;
            }

            return false;
        }

        public void LoadMatch()
        {
            background = Content.Load<Texture2D>("Graphics/amarelinho_noite");

            LoadResult();
            LoadPlayers();
            LoadRoundsStatistics();
            LoadTiles();
        }

        public void LoadRoundsStatistics()
        {
            Texture2D roundBox = Content.Load<Texture2D>("Graphics\\round_box");
            Texture2D roundCharacters = Content.Load<Texture2D>("Graphics\\round_characters");
            Texture2D roundCount = Content.Load<Texture2D>("Graphics\\round_counts");

            int startingY = (screenHeight / 2) - (((roundBox.Height + 20) * numberOfPlayersSelected) / 2);

            for (int i = 0; i < selections.Count; i++)
            {
                if (selections[i].selectionKind != Selection.SelectionKind.InputSelection)
                {
                    Vector2 destination = new Vector2((Game1.screenWidth / 2) - (roundBox.Width / 2), startingY);

                    result.AddRound(roundBox, roundCount, roundCharacters, 35, 65, destination, selections[i].playerID, selections[i].character);

                    startingY += roundBox.Height + 20;
                }
            }
        }

        public void LoadResult()
        {
            Texture2D victory = Content.Load<Texture2D>("Graphics/victory");
            result = new Result(victory, 700, 2);
        }

        public void LoadPlayers()
        {
            foreach (Selection s in selections)
            {
                if (s.selectionKind != Selection.SelectionKind.InputSelection)
                    s.PlayerLoading(Content);

                s.showText = true;
                s.showTextElapsedTime = 0;
            }
        }

        //public Player LoadPlayer(Vector2 position, string type, int width, int height, Inputs input)
        //{
        //    Player player;

        //    Animation playerAnimation = new Animation();
        //    Texture2D playerTexture = Content.Load<Texture2D>("Graphics\\" + type);
        //    // Frame dimensions: 39x - 45y
        //    playerAnimation.Initialize(playerTexture, position, width, height, 0, 0, 100, Color.White, 1f, true);

        //    Texture2D measureBar = Content.Load<Texture2D>("Graphics\\measure_bar");
        //    Texture2D aimTexture = Content.Load<Texture2D>("Graphics\\aim");
        //    Aim aim = new Aim(aimTexture, 30, 9, 50);

        //    switch (type)
        //    {
        //        case "dwarf":
        //            Animation boozeAnimation = new Animation();
        //            Texture2D boozeTexture = Content.Load<Texture2D>("Graphics\\booze");
        //            boozeAnimation.Initialize(boozeTexture, position, 9, 9, 0, 4, 100, Color.White, 1f, true);

        //            Animation lighterAnimation = new Animation();
        //            Texture2D lighterTexture = Content.Load<Texture2D>("Graphics\\lighter");
        //            lighterAnimation.Initialize(lighterTexture, position, 4, 7, 0, 4, 100, Color.White, 1f, true);

        //            Animation fireSpitAnimation = new Animation();
        //            Texture2D fireSpitTexture = Content.Load<Texture2D>("Graphics\\lighter_on_fire");
        //            fireSpitAnimation.Initialize(fireSpitTexture, position, 46, 32, 0, 4, 100, Color.White, 1f, true);

        //            FireSpitting fireSpitting = new FireSpitting(boozeAnimation, lighterAnimation, fireSpitAnimation, 0.5f, 100);

        //            Texture2D boozeSpinTexture = Content.Load<Texture2D>("Graphics\\spinning_booze");

        //            //SpinningBooze boozeSpinning = new SpinningBooze(boozeSpinAnimation, 0.3f, 25, 5);

        //            player = new Dwarf(playerAnimation, aim, measureBar, 1, 100, 100, 0, 5, 5, new Vector2(6, 33),
        //                                Player.Side.Right, input, fireSpitting, boozeSpinTexture);
        //            break;

        //        case "dove":
        //            Animation sandwichAnimation = new Animation();
        //            Texture2D sandwichTexture = Content.Load<Texture2D>("Graphics\\sandwich");
        //            sandwichAnimation.Initialize(sandwichTexture, position, 9, 9, 0, 5, 400, Color.White, 1f, true);

        //            Animation stinkAnimation = new Animation();
        //            Texture2D stinkTexture = Content.Load<Texture2D>("Graphics\\stink");
        //            stinkAnimation.Initialize(stinkTexture, position, 45, 45, 0, 6, 100, Color.White, 1f, true);

        //            Texture2D pooTexture = Content.Load<Texture2D>("Graphics\\poo");

        //            Texture2D forkTexture = Content.Load<Texture2D>("Graphics\\fork");
        //            Animation forkAnimation = new Animation();
        //            forkAnimation.Initialize(forkTexture, new Vector2(0, 0), 18, 18, 0, 7, 50, Color.White, 1f, true);

        //            Projectile fork = new Projectile(forkAnimation, 0, 35, 10);

        //            player = new Dove(playerAnimation, aim, measureBar, 1, 100, 100, 0.5f, 5, 5, new Vector2(3, 39),
        //                                Player.Side.Left, input, sandwichAnimation, pooTexture, 4, stinkAnimation, fork);
        //            break;

        //        default:
        //            player = null;
        //            break;
        //    }

        //    return player;
        //}

        public void LoadTiles()
        {
            Texture2D tileTexture = Content.Load<Texture2D>("Graphics\\tile");

            tiles.Add(new Tile(tileTexture, 0, 0, 15, screenWidth));
            tiles.Add(new Tile(tileTexture, 15, 0, screenWidth - 15, 15));
            tiles.Add(new Tile(tileTexture, screenWidth - 15, 15, 15, screenWidth - 15));
            tiles.Add(new Tile(tileTexture, 15, screenHeight - 15, screenWidth - 15 - 15, 15));

            tiles.Add(new Tile(tileTexture, 15, 140, 68, 53));
            tiles.Add(new Tile(tileTexture, screenWidth - 15 - 68, 140, 68, 53));
            tiles.Add(new Tile(tileTexture, 15, screenHeight - 15 - 80 - 43, 118, 43));
            tiles.Add(new Tile(tileTexture, screenWidth - 15 - 118, screenHeight - 15 - 80 - 43, 118, 43));

            tiles.Add(new Tile(tileTexture, 15 + 147, 15 + 87, 93, 14));
            tiles.Add(new Tile(tileTexture, screenWidth - 15 - 147 - 93, 15 + 87, 93, 14));
            tiles.Add(new Tile(tileTexture, 15 + 147, 15 + 87 + 143 + 14, 93, 14));
            tiles.Add(new Tile(tileTexture, screenWidth - 15 - 147 - 93, 15 + 87 + 143 + 14, 93, 14));

            tiles.Add(new Tile(tileTexture, 15 + 195, screenHeight - 15 - 45, 380, 45));
            tiles.Add(new Tile(tileTexture, 15 + 180 + 90, screenHeight - 15 - 15 - 89, 230, 89));
            tiles.Add(new Tile(tileTexture, 15 + 305, 177 + 15, 160, 17));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || GamePad.GetState(PlayerIndex.Two).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            switch (screen)
            {
                case Screen.Menu:
                    UpdateMenu();
                    break;

                case Screen.Selection:
                    UpdateSelection(gameTime);
                    break;

                case Screen.Match:
                    UpdateMatch(gameTime);
                    break;

                case Screen.MatchStatistics:
                    UpdateMatchStatistics(gameTime);
                    break;

                case Screen.Result:
                    UpdateResult();
                    break;
            }

            base.Update(gameTime);
        }

        public void UpdateResult()
        {
            bool resetMatrix = false;

            foreach (Player p in players)
            {
                p.inputs.GetInputs();

                if (p.inputs.PauseGame())
                {
                    resetMatrix = true;
                }
            }

            if (resetMatrix)
            {
                ClearMatch();
            }
        }

        public void ClearMatch()
        {
            background = Content.Load<Texture2D>("Graphics/boy_underneath_a_tree");

            players.Clear();
            tiles.Clear();
            result = null;
            maxNumberOfPlayers = 0;
            numberOfPlayersSelected = 0;
            deadPlayers = 0;
            ResetInputsTaken();

            selections.Clear();
            LoadSelectionScreen();

            screen = Screen.Menu;

            SoundsLibrary.PlaySongs(SoundsLibrary.menuSong);
        }

        public void UpdateMenu()
        {
            keyboardOne.GetInputs();
            gamePadOne.GetInputs();

            if (gamePadOne.SelectY() != 0 || keyboardOne.SelectY() != 0)
            {
                int currentButton = (int)menuButtons;

                if (gamePadOne.SelectY() != 0)
                {
                    currentButton += gamePadOne.SelectY();
                }
                else if (keyboardOne.SelectY() != 0)
                {
                    currentButton += keyboardOne.SelectY();
                }

                if (currentButton < 0)
                {
                    currentButton = Enum.GetNames(typeof(MenuButtons)).Length - 1;
                }
                else if (currentButton >= Enum.GetNames(typeof(MenuButtons)).Length)
                {
                    currentButton = 0;
                }

                switch (menuButtons)
                {
                    case MenuButtons.Play:
                        playButton.UnselectButton();
                        break;

                    case MenuButtons.Options:
                        optionsButton.UnselectButton();
                        break;

                    case MenuButtons.Quit:
                        quitButton.UnselectButton();
                        break;
                }

                menuButtons = (MenuButtons)currentButton;

                switch (menuButtons)
                {
                    case MenuButtons.Play:
                        playButton.SelectButton();
                        break;

                    case MenuButtons.Options:
                        optionsButton.SelectButton();
                        break;

                    case MenuButtons.Quit:
                        quitButton.SelectButton();
                        break;
                }
            }

            if (gamePadOne.PauseGame() || keyboardOne.PauseGame() ||
                gamePadOne.PauseGame() || keyboardOne.PauseGame())
            {
                switch (menuButtons)
                {
                    case MenuButtons.Play:
                        screen = Screen.Selection;
                        break;

                    case MenuButtons.Options:
                        
                        break;

                    case MenuButtons.Quit:
                        Exit();
                        break;
                }
            }
        }

        public int CheckAnyMoveYInput()
        {
            return 0;
        }

        public void UpdateSelection(GameTime gameTime)
        {
            if (readyToLoadMatch)
            {
                LoadMatch();
                readyToLoadMatch = false;
                screen = Screen.Match;

                SoundsLibrary.PlaySongs(SoundsLibrary.matchLittleYellowSong);
            }
            else
            {
                GetAllInputs();

                foreach (Selection s in selections)
                {
                    s.Update(gameTime);

                    if (s.selectionKind == Selection.SelectionKind.InputSelection)
                    {
                        CheckForSelectedInput(s);
                    }
                }

                if (numberOfPlayersSelected == maxNumberOfPlayers && maxNumberOfPlayers > 1)
                {
                    readyToLoadMatch = true;
                }
            }
        }

        public void GetAllInputs()
        {
            keyboardOne.GetInputs();
            keyboardTwo.GetInputs();
            gamePadOne.GetInputs();
            gamePadTwo.GetInputs();
            gamePadThree.GetInputs();
            gamePadFour.GetInputs();
        }

        public void ResetInputsTaken()
        {
            keyboardOneTaken = false;
            keyboardTwoTaken = false;
            gamePadOneTaken = false;
            gamePadTwoTaken = false;
            gamePadThreeTaken = false;
            gamePadFourTaken = false;
        }

        public void CheckForSelectedInput(Selection s)
        {
            if (keyboardOne.PauseGame())
            {
                if (!keyboardOneTaken)
                {
                    s.input = keyboardOne;
                    s.selectionKind = Selection.SelectionKind.CharacterSelection;
                    keyboardOneTaken = true;
                    maxNumberOfPlayers++;
                    return;
                }
            }

            if (keyboardTwo.PauseGame())
            {
                if (!keyboardTwoTaken)
                {
                    s.input = keyboardTwo;
                    s.selectionKind = Selection.SelectionKind.CharacterSelection;
                    keyboardTwoTaken = true;
                    maxNumberOfPlayers++;
                    return;
                }
            }

            if (gamePadOne.PauseGame())
            {
                if (!gamePadOneTaken)
                {
                    s.input = gamePadOne;
                    s.selectionKind = Selection.SelectionKind.CharacterSelection;
                    gamePadOneTaken = true;
                    maxNumberOfPlayers++;
                    return;
                }
            }

            if (gamePadTwo.PauseGame())
            {
                if (!gamePadTwoTaken)
                {
                    s.input = gamePadTwo;
                    s.selectionKind = Selection.SelectionKind.CharacterSelection;
                    gamePadTwoTaken = true;
                    maxNumberOfPlayers++;
                    return;
                }
            }

            if (gamePadThree.PauseGame())
            {
                if (!gamePadThreeTaken)
                {
                    s.input = gamePadThree;
                    s.selectionKind = Selection.SelectionKind.CharacterSelection;
                    gamePadThreeTaken = true;
                    maxNumberOfPlayers++;
                    return;
                }
            }

            if (gamePadFour.PauseGame())
            {
                if (!gamePadFourTaken)
                {
                    s.input = gamePadFour;
                    s.selectionKind = Selection.SelectionKind.CharacterSelection;
                    gamePadFourTaken = true;
                    maxNumberOfPlayers++;
                    return;
                }
            }
        }

        public void UpdateMatch(GameTime gameTime)
        {
            foreach (Player p in players)
            {
                p.Update(gameTime);

                //if (p.inputs.GetType().Name == "KeyboardAndMouseInputs" && p.aiming)
                //{
                //    AdjustAimingMousePosition(p);
                //}
            }

            if (deadPlayers == maxNumberOfPlayers - 1)
            {
                foreach (Player pl in players)
                {
                    if (!pl.playerIsDead)
                    {
                        result.SetRoundWinner(pl.id);
                        break;
                    }
                }

                screen = Screen.MatchStatistics;
            }
        }

        public void UpdateMatchStatistics(GameTime gameTime)
        {
            bool reloadMatch = false;

            foreach (Player p in players)
            {
                p.inputs.GetInputs();

                if (p.inputs.PauseGame())
                {
                    if (!Result.matchEnded)
                    {
                        reloadMatch = true;
                    }
                    else
                    {
                        screen = Screen.Result;

                        SoundsLibrary.PlaySongs(SoundsLibrary.victoryScreenSong);
                    }
                }
            }

            if (reloadMatch)
            {
                foreach (Player pl in players)
                {
                    pl.ResetPlayer();
                }

                deadPlayers = 0;
                screen = Screen.Match;
            }

            result.Update(gameTime);
        }

        public void AdjustAimingMousePosition(Player p)
        {
            try
            {
                Mouse.SetPosition(p.aim.mouseAimArea.X - (p.aim.mouseAimArea.Width / 2),
                                            p.aim.mouseAimArea.Y - (p.aim.mouseAimArea.Height / 2));

                #region Commented Code
                //if (Game1.currentMouseState.Position.Y > aim.mouseAimArea.Y + aim.mouseAimArea.Height)
                //{
                //    Mouse.SetPosition(Game1.currentMouseState.Position.X,
                //                        aim.mouseAimArea.Y + aim.mouseAimArea.Height - 1);

                //    // 2
                //    Console.WriteLine("Mudado em 2:\nNew: " + Mouse.GetState().Position.ToString() +
                //                    "\nCurrent: " + Game1.currentMouseState.Position.ToString() +
                //                        "\nComparisson: " + (aim.mouseAimArea.Y + aim.mouseAimArea.Height).ToString());
                //}

                //if (Game1.currentMouseState.Position.Y < aim.mouseAimArea.Y)
                //{
                //    Mouse.SetPosition(Game1.currentMouseState.Position.X, aim.mouseAimArea.Y - 1);

                //    // 3
                //    Console.WriteLine("Mudado em 3:\nNew: " + Mouse.GetState().Position.ToString() +
                //                    "\nCurrent: " + Game1.currentMouseState.Position.ToString() +
                //                        "\nComparisson: " + (aim.mouseAimArea.Y + aim.mouseAimArea.Height).ToString());
                //}

                //if (Game1.currentMouseState.Position.X > aim.mouseAimArea.X + aim.mouseAimArea.Width)
                //{
                //    Mouse.SetPosition(aim.mouseAimArea.X + aim.mouseAimArea.Width - 1, Game1.currentMouseState.Position.Y);

                //    // 4
                //    Console.WriteLine("Mudado em 4:\nNew: " + Mouse.GetState().Position.ToString() +
                //                    "\nCurrent: " + Game1.currentMouseState.Position.ToString() +
                //                        "\nComparisson: " + (aim.mouseAimArea.Y + aim.mouseAimArea.Height).ToString());
                //}

                //if (Game1.currentMouseState.Position.X < aim.mouseAimArea.X)
                //{
                //    Mouse.SetPosition(aim.mouseAimArea.X - 1, Game1.currentMouseState.Position.Y);

                //    // 5
                //    Console.WriteLine("Mudado em 5:\nNew: " + Mouse.GetState().Position.ToString() +
                //                    "\nCurrent: " + Game1.currentMouseState.Position.ToString() +
                //                        "\nComparisson: " + (aim.mouseAimArea.Y + aim.mouseAimArea.Height).ToString());
                //}
                #endregion
            }
            catch
            {
                //if (ex.GetHashCode() != 33583636)
                //{

                //}
            }
        }

        #region Commented Method
        //public void UpdateGameplay(GameTime gameTime)
        //{
        //    CheckPlayerOneInputs(player);

        //    // Make sure that the player does not go out of bounds
        //    player.position.X = MathHelper.Clamp(player.position.X, 0, GraphicsDevice.Viewport.Width - (player.width));
        //    if (player.position.Y > GraphicsDevice.Viewport.Height)
        //        player.active = false;
        //    //player.position.Y = MathHelper.Clamp(player.position.Y, 0, GraphicsDevice.Viewport.Height - (player.height));

        //    player.Update(gameTime);

        //    foreach (Tile t in tiles)
        //    {
        //        if (t.rectangle.Intersects(player.down))
        //        {
        //            if (player.onGround == null)
        //            {
        //                player.position.Y = t.rectangle.Y - player.height;
        //                player.AdjustCollisionRectangles();
        //                player.onGround = t;
        //                player.jumpForce = 0;
        //            }
        //        }
        //        else if (player.onGround == t)
        //        {
        //            player.onGround = null;
        //            player.jumpForce = 0;
        //        }

        //        if (t.rectangle.Intersects(player.up))
        //        {
        //            player.position.Y = t.rectangle.Y + t.rectangle.Height;
        //            player.AdjustCollisionRectangles();
        //            player.jumpForce = 0;
        //            player.hp -= 5;
        //            if (player.hp <= 0)
        //            {
        //                player.active = false;
        //            }
        //        }
        //        else if (t.rectangle.Intersects(player.left) && player.side == Player.Side.Left)
        //        {
        //            player.position.X = t.rectangle.X + t.rectangle.Width;
        //            player.AdjustCollisionRectangles();
        //        }
        //        else if (t.rectangle.Intersects(player.right) && player.side == Player.Side.Right)
        //        {
        //            player.position.X = t.rectangle.X - player.width;
        //            player.AdjustCollisionRectangles();
        //        }
        //    }

        //    player.FinishUpdating(gameTime);
        //}

        //public void CheckPlayerOneInputs()
        //{
        //    if (GamePad.GetState(player.playerIndex).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
        //    {
        //        player.position = new Vector2(80, 310);
        //        player.hp = 100;
        //        player.sp = 100;
        //        player.onGround = null;
        //        player.active = true;
        //    }

        //    // Use the Keyboard / Dpad
        //    if (player.currentKeyboardState.IsKeyDown(Keys.Right) || player.currentGamePadState.DPad.Right == ButtonState.Pressed || player.currentGamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
        //    {
        //        if (player.currentGamePadState.IsButtonDown(Buttons.LeftThumbstickRight))
        //        {
        //            player.Walk(player.currentGamePadState.ThumbSticks.Left.X);
        //        }
        //        else
        //        {
        //            player.Walk(1);
        //        }
        //    }
        //    else if (player.currentKeyboardState.IsKeyDown(Keys.Left) || player.currentGamePadState.DPad.Left == ButtonState.Pressed || player.currentGamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
        //    {
        //        if (player.currentGamePadState.IsButtonDown(Buttons.LeftThumbstickLeft))
        //        {
        //            player.Walk(player.currentGamePadState.ThumbSticks.Left.X);
        //        }
        //        else
        //        {
        //            player.Walk(-1);
        //        }
        //    }
        //    else
        //    {
        //        if (player.onGround != null)
        //        {
        //            player.ChangeAction(Player.Action.Idle);
        //        }
        //    }

        //    if (player.currentKeyboardState.IsKeyDown(Keys.Up) || player.currentKeyboardState.IsKeyDown(Keys.Space) || player.currentGamePadState.Buttons.A == ButtonState.Pressed)
        //    {
        //        if (player.onGround != null && player.sp >= 30)
        //        {
        //            player.sp -= 30;
        //            if (player.sp < 0)
        //            {
        //                player.sp = 0;
        //            }
        //            player.onGround = null;
        //            player.jumpForce = 8f;
        //            player.ChangeAction(Player.Action.Jumping);
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (screen)
            {
                case Screen.Menu:
                    DrawMenu();
                    break;

                case Screen.Selection:
                    DrawSelection();
                    break;

                case Screen.Match:
                    DrawMatch();
                    break;

                case Screen.MatchStatistics:
                case Screen.Result:
                    DrawMatchStatistics();
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawMenu()
        {
            spriteBatch.Draw(background, backgroundRectangle, Color.White);

            playButton.Draw(spriteBatch);
            optionsButton.Draw(spriteBatch);
            quitButton.Draw(spriteBatch);
        }

        public void DrawSelection()
        {
            foreach (Selection s in selections)
            {
                s.Draw(spriteBatch);
            }
        }

        public void DrawMatchStatistics()
        {
            result.Draw(spriteBatch);
        }

        public void DrawMatch()
        {
            spriteBatch.Draw(background, backgroundRectangle, Color.White);

            foreach (Tile t in tiles)
            {
                t.Draw(spriteBatch);
            }

            foreach (Player p in players)
            {
                p.Draw(spriteBatch);
            }

            foreach (Player p in players)
            {
                p.DrawSkills(spriteBatch);
            }
        }
    }
}
