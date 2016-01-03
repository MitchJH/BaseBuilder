using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BaseBuilder
{
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Color backColour = Color.FromNonPremultiplied(100, 100, 100, 255);
        SpriteBatch spriteBatch;

        World world;
        MainMenu mainMenu;

        public Engine(string[] args)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Load the settings file
            Settings.LoadFromFile();
            // Parse any command line arguments
            Settings.ParseCommandLineArguments(args);

            // Set starting game state
            GameStateManager.State = Settings.StartingGameState;

            // Set mouse visibility and fixed timestep
            this.IsMouseVisible = Settings.IsMouseVisible;
            this.IsFixedTimeStep = Settings.IsFixedTimestep;

            // Set the resolution
            this.graphics.PreferredBackBufferWidth = Settings.X_resolution;
            this.graphics.PreferredBackBufferHeight = Settings.Y_resolution;

            // Adjust window mode
            if (Settings.WindowMode == WindowMode.Fullscreen)
            {
                this.graphics.IsFullScreen = true;
            }
            else
            {
                // Set the position of the window
                var form = System.Windows.Forms.Control.FromHandle(this.Window.Handle).FindForm();
                form.Location = new System.Drawing.Point(Settings.X_windowPos, Settings.Y_windowPos);

                // Set the size of the window
                form.Size = new System.Drawing.Size(Settings.Window_Width, Settings.Window_Height);

                if (Settings.WindowMode == WindowMode.Borderless)
                {
                    // Make the form borderless
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                }
            }
        }

        protected override void Initialize()
        {
            // Create the camera
            Camera.Create(GraphicsDevice.Viewport);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the base textures
            Sprites.MISSING_TEXTURE = Content.Load<Texture2D>("Textures/missing");
            Sprites.PIXEL = Content.Load<Texture2D>("Textures/pixel");

            // Load the localization file
            Localization.LoadLocalization();

            // Load all fonts
            Fonts.LoadFontBank("Content/Data/fontbank.txt", Content);

            // Load all game sprites
            Sprites.LoadSpriteBank("Content/Data/spritebank.txt", Content);

            // Load all sound effects
            Audio.LoadSoundBank("Content/Data/soundbank.txt", Content);

            // Load all object types
            ObjectManager.LoadObjectBank("Content/Data/objectbank.txt", Content);

            // Load all structure types
            StructureManager.LoadStructureBank("Content/Data/structurebank.txt", Content);
            
            // Enable the FPS counter
            FrameRateCounter.Enable();
            //Enable Crew Stats
            DebugCrewStats.Enable();
            // Enable version display
            Version.Enable();

            // Create Objects
            world = new World(GraphicsDevice.Viewport.Bounds, Content);
            mainMenu = new MainMenu(GraphicsDevice.Viewport.Bounds, Content);
        }

        protected override void UnloadContent()
        {
            Settings.WriteToFile();
        }

        protected override void Update(GameTime gameTime)
        {
            Controls.Update();

            if (GameStateManager.State == GameState.GameWorld)
            {
                world.Update(gameTime);

                // DEBUG INFORMATION
                this.Window.Title = "DEBUG - " +
                    "(Mouse: " + (int)Controls.GameMousePosition.X + ":" + (int)Controls.GameMousePosition.Y + ") " +
                    "(Tile: " + Controls.MouseTilePosition.X + ":" + Controls.MouseTilePosition.Y + ") " +
                    "(Camera Position:{X:" + Camera.Position.X.ToString("N2") + " " + Camera.Position.Y.ToString("N2") + "} - Zoom:" + Camera.Zoom.ToString("N3") + ") " +
                    "(" + World.Clock.DebugText + ")";
            }
            else if (GameStateManager.State == GameState.MainMenu)
            {
                mainMenu.Update();
            }

            if (GameStateManager.State == GameState.Exit)
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GameStateManager.State == GameState.GameWorld)
            {
                GraphicsDevice.Clear(backColour);
                world.Draw(spriteBatch);
            }
            else if (GameStateManager.State == GameState.MainMenu)
            {
                GraphicsDevice.Clear(Color.Black);
                mainMenu.Draw(spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
