using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Coursework_180016989
{


    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // Different Scenes
        enum Menu
        {
            MainMenu = 0,
            Gameplay = 1,
            GameOver = 2,
            Victory = 3
        }

        // Game Difficulty
        enum Difficulty
        {
            Normal = 0,
            Hard = 1
        }

        // Finite State Machine for different menus
        FSM fsmMenus;

        // Command Manager for Input Events
        CommandManager inputCommandManager;

        // Collision Manager for Collision Events
        CollisionManager collisionManager;

        // Loader object to read XML and text files
        Loader loader;

        private Menu currentMenu;
        private Difficulty currentDifficulty;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Player player;

        Pickup healthPickup;
        Pickup magicPickup;

        Platform ground;

        Boss currentBoss;

        // List of different spells
        // for independent updates
        List<Spell> power_balls = new List<Spell>();
        List<Spell> fire_balls = new List<Spell>();
        List<Spell> ice_balls = new List<Spell>();

        List<Spell> currentSpell = new List<Spell>();

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        MouseState currentMouseState;
        MouseState previousMouseState;

        // Custom cursor texture
        Texture2D cursorTexture;

        // The main background
        Texture2D mainBackground;

        // Parallaxing background of clouds
        List<ParallaxingBackground> clouds = new List<ParallaxingBackground>();

        // HUD
        private SpriteFont mainHud;
        List<string> TutorialTexts;

        // Life and Magic Bars
        Texture2D barOutline;
        Texture2D enemyBarOutline;
        Texture2D healthBar;
        Texture2D manaBar;
        Texture2D enemyHealthBar;

        List<Texture2D> currentSpellIcons = new List<Texture2D>();
        private int currentSpellNum;

        // Time played
        private float playTime;
        private int minutesPlayed;
        private int secondsPlayed;

        // A small pause between boss switches
        // and menu scenes
        private float cutSceneTimer;
        private float cutSceneDuration;

        public Game1()
        {

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1080;

            graphics.IsFullScreen = true;

            graphics.GraphicsProfile = GraphicsProfile.HiDef;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            inputCommandManager = new CommandManager();

            collisionManager = new CollisionManager();

            fsmMenus = new FSM(this);

            loader = new Loader();

            // Different Game States
            MainMenuState mainMenuState = new MainMenuState();
            GameplayState gameplayState = new GameplayState();
            GameOverState gameOverState = new GameOverState();
            VictoryState victoryState = new VictoryState();

            // Adding appropriate transitions to each state
            mainMenuState.AddTransition(new Transition(gameplayState, () => currentMenu == Menu.Gameplay));

            gameplayState.AddTransition(new Transition(mainMenuState, () => currentMenu == Menu.MainMenu));
            gameplayState.AddTransition(new Transition(gameOverState, () => currentMenu == Menu.GameOver));
            gameplayState.AddTransition(new Transition(victoryState, () => currentMenu == Menu.Victory));

            gameOverState.AddTransition(new Transition(mainMenuState, () => currentMenu == Menu.MainMenu));
            gameOverState.AddTransition(new Transition(gameplayState, () => currentMenu == Menu.Gameplay));

            victoryState.AddTransition(new Transition(mainMenuState, () => currentMenu == Menu.MainMenu));
            victoryState.AddTransition(new Transition(gameplayState, () => currentMenu == Menu.Gameplay));

            // Add all the states to the FSM
            fsmMenus.AddState(mainMenuState);
            fsmMenus.AddState(gameplayState);
            fsmMenus.AddState(gameOverState);
            fsmMenus.AddState(victoryState);

            fsmMenus.Initialise("MainMenu");
            currentMenu = Menu.MainMenu;
            
            player = new Player(Services);

            healthPickup = new Pickup(Services);
            magicPickup = new Pickup(Services);

            ground = new Platform(Services);

            TutorialTexts = new List<string>();

            IsMouseVisible = false;

            InitialiseBindings();

            base.Initialize();

        }

        private void InitialiseBindings()
        {
            // All the command keys used in the game
            inputCommandManager.AddKeyboardBindings(Keys.A, player.MoveLeft);
            inputCommandManager.AddKeyboardBindings(Keys.D, player.MoveRight);
            inputCommandManager.AddKeyboardBindings(Keys.Space, player.Jump);
            inputCommandManager.AddKeyboardBindings(Keys.LeftShift, player.Dash);
            inputCommandManager.AddKeyboardBindings(Keys.D1, SetPowerSpell);
            inputCommandManager.AddKeyboardBindings(Keys.D2, SetFireSpell);
            inputCommandManager.AddKeyboardBindings(Keys.D3, SetIceSpell);

            inputCommandManager.AddMouseBindings(MouseButton.RIGHT, player.Dash);
            inputCommandManager.AddMouseBindings(MouseButton.LEFT, CastSpell);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the data from the XML file
            loader.LoadGame("info.xml");

            // Each cloud layer
            ParallaxingBackground bgLayer1 = new ParallaxingBackground(Services);
            ParallaxingBackground bgLayer2 = new ParallaxingBackground(Services);
            ParallaxingBackground bgLayer3 = new ParallaxingBackground(Services);
            ParallaxingBackground bgLayer4 = new ParallaxingBackground(Services);

            // The clouds are made to move at
            // different speeds and directions
            bgLayer1.Initialise("Graphics/World/clouds_1", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialise("Graphics/World/clouds_2", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, -2);
            bgLayer3.Initialise("Graphics/World/clouds_3", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 1);
            bgLayer4.Initialise("Graphics/World/clouds_4", GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 2);

            clouds.Add(bgLayer1);
            clouds.Add(bgLayer2);
            clouds.Add(bgLayer3);
            clouds.Add(bgLayer4);

            mainBackground = Content.Load<Texture2D>("Graphics/World/Mountain Background");

            // Player is set to fall down from the 
            // middle of the screen at the far left
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            // Placed at 75 units from the bottom
            Vector2 groundPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.Height - 75.0f);

            player.Initialise(playerPosition, GraphicsDevice);
            ground.Initialise(PlatformType.Rigid, groundPosition);
            healthPickup.Initalise(GraphicsDevice, PickupType.Health);
            magicPickup.Initalise(GraphicsDevice, PickupType.Magic);

            collisionManager.AddCollidable(player);

            // Name of boss loaded from XML file
            switch (SaveInfo.Instance.BossName)
            {
                // The health is set and weapons are added 
                // based on which boss is currently active
                case "BlackFog":

                    currentBoss = new BlackFog(Services, GraphicsDevice, SaveInfo.Instance.BossHealth);

                    collisionManager.AddCollidable(currentBoss);
                    collisionManager.AddCollidable((currentBoss as BlackFog).GetSword());
                    collisionManager.AddCollidable((currentBoss as BlackFog).GetLeftWave());
                    collisionManager.AddCollidable((currentBoss as BlackFog).GetRightWave());

                    break;

                case "SwampCry":

                    currentBoss = new SwampCry(Services, GraphicsDevice, SaveInfo.Instance.BossHealth);

                    collisionManager.AddCollidable(currentBoss);
                    collisionManager.AddCollidable((currentBoss as SwampCry).GetClub());
                    collisionManager.AddCollidable((currentBoss as SwampCry).GetSpell());

                    break;
            }

            collisionManager.AddCollidable(ground);
            collisionManager.AddCollidable(healthPickup);
            collisionManager.AddCollidable(magicPickup);

            for (int i = 0; i < 20; i++)
            {
                Spell p_spell = new Spell(Services);

                // The player function is subscribed to the spell function
                // for weapon accuracy calculation
                p_spell.WeaponAccuracy += player.CalculateWeaponAccuracy;

                p_spell.Initialise(playerPosition, SpellType.Power);
                power_balls.Add(p_spell);
                collisionManager.AddCollidable(p_spell);
            }
            for (int i = 0; i < 20; i++)
            {
                Spell f_spell = new Spell(Services);

                f_spell.WeaponAccuracy += player.CalculateWeaponAccuracy;

                f_spell.Initialise(playerPosition, SpellType.Fire);
                fire_balls.Add(f_spell);
                collisionManager.AddCollidable(f_spell);
            }
            for (int i = 0; i < 20; i++)
            {
                Spell i_spell = new Spell(Services);

                i_spell.WeaponAccuracy += player.CalculateWeaponAccuracy;

                i_spell.Initialise(playerPosition, SpellType.Ice);
                ice_balls.Add(i_spell);
                collisionManager.AddCollidable(i_spell);
            }

            currentSpell = power_balls;

            cursorTexture = Content.Load<Texture2D>("Graphics/UI/custom-cursor");

            mainHud = Content.Load<SpriteFont>("Fonts/gameFont");

            barOutline = Content.Load<Texture2D>("Graphics/UI/bar_outline");
            enemyBarOutline = Content.Load<Texture2D>("Graphics/UI/enemy_bar_outline");
            healthBar = Content.Load<Texture2D>("Graphics/UI/health_bar");
            enemyHealthBar = Content.Load<Texture2D>("Graphics/UI/enemy_health_bar");
            manaBar = Content.Load<Texture2D>("Graphics/UI/mana_bar");

            currentSpellIcons.Add(Content.Load<Texture2D>("Graphics/UI/Power-Logo"));
            currentSpellIcons.Add(Content.Load<Texture2D>("Graphics/UI/Fire-Logo"));
            currentSpellIcons.Add(Content.Load<Texture2D>("Graphics/UI/Ice-Logo"));

            // Loading time played from XML file
            minutesPlayed = SaveInfo.Instance.MinutesPlayed;
            secondsPlayed = SaveInfo.Instance.SecondsPlayed;

            cutSceneDuration = 3.0f;
            cutSceneTimer = 0.0f;

            // Loading in tutorial instructions from a text file
            Stream s = TitleContainer.OpenStream("Content/Scripts/Tutorials.txt");
            loader.ReadLinesFromText(s, ref TutorialTexts);

        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unloading in the reverse order they were initialised

            for (int i = 0; i < power_balls.Count; i++)
                power_balls[i].UnloadContent();
            for (int i = 0; i < fire_balls.Count; i++)
                fire_balls[i].UnloadContent();
            for (int i = 0; i < ice_balls.Count; i++)
                ice_balls[i].UnloadContent();

            currentBoss.UnloadContent();

            for (int i = 0; i < clouds.Count; i++)
                clouds[i].UnloadContent();

            ground.UnloadContent();

            magicPickup.UnloadContent();
            healthPickup.UnloadContent();

            player.UnloadContent();

            Content.Unload();
        }

        // Reset the entire game
        private void GameReset()
        {
            player.Reset();

            // This is only used to remove the collidables
            // from the collision manager when the player wishes to restart
            if (currentBoss is SwampCry)
            {
                int index = collisionManager.GetIndexOfCollidable(currentBoss);

                collisionManager.RemoveCollidable(currentBoss);
                collisionManager.RemoveCollidable((currentBoss as SwampCry).GetClub());
                collisionManager.RemoveCollidable((currentBoss as SwampCry).GetSpell());

                currentBoss.UnloadContent();
                currentBoss = null;
                currentBoss = new BlackFog(Services, GraphicsDevice);

                collisionManager.AddCollidable((currentBoss as BlackFog).GetSword());
                collisionManager.AddCollidable((currentBoss as BlackFog).GetLeftWave());
                collisionManager.AddCollidable((currentBoss as BlackFog).GetRightWave());

                collisionManager.InsertCollidable(index, currentBoss as BlackFog);

            }
            else
                currentBoss.Reset();

            // Reset all the spells
            for (int i = 0; i < power_balls.Count; i++)
                power_balls[i].Reset();

            for (int i = 0; i < fire_balls.Count; i++)
                fire_balls[i].Reset();

            for (int i = 0; i < ice_balls.Count; i++)
                ice_balls[i].Reset();

            // Reset all the pickups
            magicPickup.Reset();
            healthPickup.Reset();

            // Reset the playtime
            // and the cutscene pause timer
            playTime = 0.0f;
            minutesPlayed = 0;
            secondsPlayed = 0;

            cutSceneTimer = 0.0f;

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // The game is saved before closing the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                // Make sure we reset all the values
                // before saving if we're not in gameplay
                if (currentMenu == Menu.GameOver || currentMenu == Menu.Victory)
                    GameReset();

                SaveInfo.Instance.PlayerHealth = player.Health;
                SaveInfo.Instance.PlayerMagic = player.Magic;
                SaveInfo.Instance.SpellsFired = player.SpellsFired;
                SaveInfo.Instance.SpellsHit = player.SpellsHit;
                SaveInfo.Instance.MinutesPlayed = minutesPlayed;
                SaveInfo.Instance.SecondsPlayed = secondsPlayed;
                SaveInfo.Instance.BossHealth = currentBoss.GetHealth();
                SaveInfo.Instance.BossName = currentBoss.GetType().Name;

                loader.SaveGame("info.xml", SaveInfo.Instance);

                UnloadContent();

                Exit();
            }

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Get the states for all input hardware
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;

            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            // Update the menu we're currently in
            fsmMenus.Update(elapsed);

            base.Update(gameTime);

        }

        // Enter the game in the appropriate difficulty
        public void MainMenuUpdate()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                currentMenu = Menu.Gameplay;
                currentDifficulty = Difficulty.Normal;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Enter))
            {
                currentMenu = Menu.Gameplay;
                currentDifficulty = Difficulty.Hard;
            }
        }

        // This would either be
        // Game Over Screen
        // OR
        // Victory Screen
        private void EndgameUpdate()
        {
            if (currentKeyboardState.IsKeyDown(Keys.Q) &&
                previousKeyboardState.IsKeyUp(Keys.Q))
            {
                GameReset();

                currentMenu = Menu.MainMenu;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.Space) &&
                previousKeyboardState.IsKeyUp(Keys.Space))
            {
                GameReset();

                currentMenu = Menu.Gameplay;
                currentDifficulty = Difficulty.Normal;

            }
            else if (currentKeyboardState.IsKeyDown(Keys.Enter) &&
                previousKeyboardState.IsKeyUp(Keys.Enter))
            {
                GameReset();

                currentMenu = Menu.Gameplay;
                currentDifficulty = Difficulty.Hard;
            }

        }

        // Both functions do the same
        // but are kept separate for
        // any future changes
        public void GameOverUpdate() { EndgameUpdate(); }
        public void VictoryUpdate() { EndgameUpdate(); }

        // The main game update
        public void GameUpdate(float elapsed)
        {
            // Update the clouds
            for (int i = 0; i < clouds.Count; i++)
                clouds[i].Update();

            // Update the input events
            inputCommandManager.Update();

            // Update the player
            player.Update(elapsed, currentKeyboardState, GraphicsDevice);

            // Update the current boss
            currentBoss.Update(elapsed, GraphicsDevice, player.GetPosition());

            // The seconds are
            // made to increase by
            // the delta time
            if (playTime >= 1.0f)
            {
                secondsPlayed++;

                // Once the seconds hit 60 or above
                // The minutes are increased and the
                // seconds are reset
                if (secondsPlayed >= 60) { minutesPlayed++; secondsPlayed = 0; }

                playTime = 0.0f;
            }
            else
                playTime += elapsed;
            

            // Update the health pick ups when player's health isn't
            // equal to the max health and if the difficulty
            // is set to Normal
            if (player.Health != player.MaxHealth &&
                            currentDifficulty == Difficulty.Normal)
                healthPickup.Update(elapsed, GraphicsDevice);

            // Update the magic pick up when player's
            // magic is not equal to his max magic
            if (player.Magic != player.MaxMagic)
                magicPickup.Update(elapsed, GraphicsDevice);

            // To make sure only one of them is alive at a time
            if (currentBoss.GetHealth() <= 0 && player.Health > 0)
            {
                if (cutSceneTimer >= cutSceneDuration)
                {
                    // After the pause,
                    // Load in the next boss by adding its weapons
                    // to the collision manager
                    if (currentBoss is BlackFog)
                    {

                        int index = collisionManager.GetIndexOfCollidable(currentBoss);

                        collisionManager.RemoveCollidable(currentBoss);
                        collisionManager.RemoveCollidable((currentBoss as BlackFog).GetSword());
                        collisionManager.RemoveCollidable((currentBoss as BlackFog).GetLeftWave());
                        collisionManager.RemoveCollidable((currentBoss as BlackFog).GetRightWave());

                        currentBoss.UnloadContent();
                        currentBoss = null;
                        currentBoss = new SwampCry(Services, GraphicsDevice);

                        collisionManager.InsertCollidable(index, currentBoss as SwampCry);
                        collisionManager.AddCollidable((currentBoss as SwampCry).GetClub());
                        collisionManager.AddCollidable((currentBoss as SwampCry).GetSpell());

                        cutSceneTimer = 0.0f;

                    }
                    // If it's the last boss, then move to the victory screen
                    else if (currentBoss is SwampCry)
                    {
                        currentMenu = Menu.Victory;
                    }
                }
                else
                {
                    cutSceneTimer += elapsed;
                }

            }
            else if (player.Health <= 0 && currentBoss.GetHealth() > 0)
            {
                // Move to Game Over screen after the timer
                if (cutSceneTimer >= cutSceneDuration)
                    currentMenu = Menu.GameOver;
                else
                    cutSceneTimer += elapsed;
            }

            // Each spell is made to update
            // separately because multiple spell types
            // could be moving at the same time
            for (int i = 0; i < power_balls.Count; i++)
                power_balls[i].Update(elapsed, currentMouseState, player.GetPosition(), GraphicsDevice);

            for (int i = 0; i < fire_balls.Count; i++)
                fire_balls[i].Update(elapsed, currentMouseState, player.GetPosition(), GraphicsDevice);

            for (int i = 0; i < ice_balls.Count; i++)
                ice_balls[i].Update(elapsed, currentMouseState, player.GetPosition(), GraphicsDevice);

            collisionManager.Update();

        }


        private void CastSpell(eButtonState buttonState, Vector2 amount)
        {
            // Cast a spell if the player is alive
            if (buttonState == eButtonState.PRESSED && player.Health > 0)
            {
                // Loop through the active spell
                for (int i = 0; i < currentSpell.Count; i++)
                {
                    // If it's not casted, then choose this one
                    if (currentSpell[i].SpellCasted == false)
                    {
                        // If player has any magic points left
                        if (player.Magic >= currentSpell[i].MPCost)
                        {
                            currentSpell[i].SpellCasted = true;

                            // Decrease the magic points by the cost of spell
                            player.Magic -= currentSpell[i].MPCost;

                        }

                        break;

                    }

                }
            }

        }

        // Equipping spells
        private void SetPowerSpell(eButtonState buttonState, Vector2 amount)
        {
            currentSpell = power_balls;
            currentSpellNum = 0;
        }

        private void SetFireSpell(eButtonState buttonState, Vector2 amount)
        {
            currentSpell = fire_balls;
            currentSpellNum = 1;
        }

        private void SetIceSpell(eButtonState buttonState, Vector2 amount)
        {
            currentSpell = ice_balls;
            currentSpellNum = 2;
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw the appropriate scene
            switch (currentMenu)
            {
                case Menu.MainMenu:
                    MainMenuDraw(gameTime);
                    break;
                case Menu.Gameplay:
                    GameplayDraw(gameTime);
                    break;
                case Menu.GameOver:
                    GameOverDraw(gameTime);
                    break;
                case Menu.Victory:
                    VictoryDraw(gameTime);
                    break;
            }

            base.Draw(gameTime);

        }

        private void MainMenuDraw(GameTime gameTime)
        {

            spriteBatch.Begin();

            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            // Draw the title of the game
            DrawShadowedString(mainHud, "BANISHED DEMONS", new Vector2(GraphicsDevice.Viewport.X + 120.0f,
                GraphicsDevice.Viewport.Y + 150.0f), Color.DarkRed, Vector2.Zero, 2.0f);

            // Draw all the tutorial messages
            for (int i = 0; i < TutorialTexts.Count; i++)
            {
                DrawShadowedString(mainHud, TutorialTexts[i], new Vector2((GraphicsDevice.Viewport.X + ((i + 1) * 80.0f)),
                (GraphicsDevice.Viewport.Height / 3f) + ((i + 1) * 60.0f)), Color.White, Vector2.Zero, 0.5f);
            }

            spriteBatch.End();
        }

        private void EndgameDraw(string endTitle1, string endTitle2, Color endColour)
        {
            spriteBatch.Begin();

            // To add zeroes before 1 digit numbers
            string zeroString_m;
            string zeroString_s;

            if (minutesPlayed < 10)
                zeroString_m = "0";
            else
                zeroString_m = "";

            if (secondsPlayed < 10)
                zeroString_s = "0";
            else
                zeroString_s = "";

            // Depends on which end game screen
            string message1 = "You " + endTitle2 + " for " + zeroString_m + minutesPlayed + ":" + zeroString_s + secondsPlayed;
            string message2 = "Weapon Accuracy: " + player.WeaponAccuracy + "%";
            string message3 = "Press Space to Retry in Normal Mode";
            string message4 = "Press Enter to Retry in Hardcore Mode";
            string message5 = "Press Q to Return to Main Menu";

            // Draw all the messages
            DrawShadowedString(mainHud, endTitle1, new Vector2((GraphicsDevice.Viewport.Width / 2) - 430.0f,
                (GraphicsDevice.Viewport.Height / 2) - 300.0f), endColour, Vector2.Zero, 2.0f);

            DrawShadowedString(mainHud, message1, new Vector2((GraphicsDevice.Viewport.Width / 2) - 430.0f,
                (GraphicsDevice.Viewport.Height / 2)), endColour, Vector2.Zero, 0.5f);

            DrawShadowedString(mainHud, message2, new Vector2((GraphicsDevice.Viewport.Width / 2) - 430.0f,
                (GraphicsDevice.Viewport.Height / 2) + 100.0f), endColour, Vector2.Zero, 0.5f);

            DrawShadowedString(mainHud, message3, new Vector2((GraphicsDevice.Viewport.Width / 2) - 430.0f,
                (GraphicsDevice.Viewport.Height / 2) + 200.0f), endColour, Vector2.Zero, 0.5f);

            DrawShadowedString(mainHud, message4, new Vector2((GraphicsDevice.Viewport.Width / 2) - 430.0f,
                (GraphicsDevice.Viewport.Height / 2) + 300.0f), endColour, Vector2.Zero, 0.5f);

            DrawShadowedString(mainHud, message5, new Vector2((GraphicsDevice.Viewport.Width / 2) - 430.0f,
                (GraphicsDevice.Viewport.Height / 2) + 400.0f), endColour, Vector2.Zero, 0.5f);

            spriteBatch.End();
        }

        private void GameOverDraw(GameTime gameTime) { EndgameDraw("DEFEATED", "survived", Color.Red); }

        private void VictoryDraw(GameTime gameTime) { EndgameDraw("TRIUMPHED", "fought", Color.SpringGreen); }

        private void GameplayDraw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            spriteBatch.Begin();

            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);

            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

            for (int i = 0; i < clouds.Count; i++)
                clouds[i].Draw(spriteBatch);

            ground.Draw(spriteBatch);

            player.Draw(gameTime, spriteBatch);

            magicPickup.Draw(spriteBatch);

            if (currentDifficulty == Difficulty.Normal)
                healthPickup.Draw(spriteBatch);

            for (int i = 0; i < power_balls.Count; i++)
                power_balls[i].Draw(gameTime, spriteBatch);

            for (int i = 0; i < fire_balls.Count; i++)
                fire_balls[i].Draw(gameTime, spriteBatch);

            for (int i = 0; i < ice_balls.Count; i++)
                ice_balls[i].Draw(gameTime, spriteBatch);

            currentBoss.Draw(gameTime, spriteBatch);

            DrawHUD();

            spriteBatch.Draw(currentSpellIcons[currentSpellNum], hudLocation + new Vector2(20.0f, 150.0f),
                new Rectangle(currentSpellIcons[currentSpellNum].Bounds.X, currentSpellIcons[currentSpellNum].Bounds.Y,
                currentSpellIcons[currentSpellNum].Width, currentSpellIcons[currentSpellNum].Height),
                Color.White, 0.0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0.0f);

            spriteBatch.Draw(cursorTexture, new Vector2(Mouse.GetState().X, Mouse.GetState().Y),
                new Rectangle(cursorTexture.Bounds.X, cursorTexture.Bounds.Y, cursorTexture.Width, cursorTexture.Height),
                Color.White, 0.0f, new Vector2(cursorTexture.Width / 2, cursorTexture.Height / 2), 2.0f, SpriteEffects.None, 0.0f);

            spriteBatch.End();
        }



        private void DrawHUD()
        {
            Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);

            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Drawing the outline for each bar
            spriteBatch.Draw(barOutline, hudLocation + new Vector2(10.0f, 0.0f),
                new Rectangle(barOutline.Bounds.X, barOutline.Bounds.Y,
                barOutline.Width, barOutline.Height),
                Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);

            spriteBatch.Draw(barOutline, hudLocation + new Vector2(10.0f, 60.0f),
                new Rectangle(barOutline.Bounds.X, barOutline.Bounds.Y,
                barOutline.Width, barOutline.Height),
                Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);

            spriteBatch.Draw(enemyBarOutline, hudLocation + new Vector2(700.0f, 60.0f),
                new Rectangle(enemyBarOutline.Bounds.X, enemyBarOutline.Bounds.Y,
                enemyBarOutline.Width, enemyBarOutline.Height),
                Color.White, 0.0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);

            DrawBar(healthBar, hudLocation + new Vector2(70.0f, 0.0f), player.Health, player.MaxHealth);
            DrawBar(manaBar, hudLocation + new Vector2(70.0f, 60.0f), player.Magic, player.MaxMagic);
            DrawBar(enemyHealthBar, hudLocation + new Vector2(760.0f, 60.0f),
                currentBoss.GetHealth(), currentBoss.GetMaxHealth());

        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color, Vector2 offset, float scale)
        {
            // Draw a black version of the same
            // string with some offset to portray
            // a shadow effect
            spriteBatch.DrawString(font, value, position + offset + new Vector2(3.0f, 3.0f), Color.Black, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(font, value, position + offset, color, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0.0f);
        }

        private void DrawBar(Texture2D texture, Vector2 position, float value, float maxValue)
        {
            // We get the percentage value
            // to a scale of 1
            float percent = value / maxValue;

            // We use this percentage onto
            // the texture width to get our new width
            float adjusted_width = percent * texture.Width;

            Rectangle source = new Rectangle(texture.Bounds.X, texture.Bounds.Y,
                (int)adjusted_width, texture.Height);


            spriteBatch.Draw(texture, position, source, Color.White, 0.0f,
                Vector2.Zero, 2.0f, SpriteEffects.None, 0.0f);

        }

    }
}