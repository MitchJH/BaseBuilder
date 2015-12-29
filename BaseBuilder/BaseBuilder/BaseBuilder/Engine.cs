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
        Color backColour = Color.FromNonPremultiplied(120, 120, 120, 255);
        SpriteBatch spriteBatch;
        World WORLD;

        // ############################################# //
        // ######## EVERYTHING HERE NEEDS TO GO ######## //
        // ############################################# //
        Button button1;
        Button button2;
        Button button3;
        Button button4;
        Button button5;
        Rectangle bottomBar;
        Texture2D sand;
        
        public void Event_Test(GUIControl sender)
        {
            Random rand = new Random();

            for (int i = 0; i < 10; i++)
            {
                CrewMember newCrew = new CrewMember("test" + i, 20 + i, rand.Next(10, 800), rand.Next(10, 500), "crew");
                WORLD.CrewMembers.Add(newCrew);
            }
            Audio.Play("low_double_beep");
        }
        public void Do_Beep(GUIControl sender)
        {
            SoundEffectInstance sei = Audio.Get("click").CreateInstance();
            //sei.Volume = (0.2f) / Settings.MasterVolume;
            sei.Play();
        }

        // ##### THIS WILL GO INTO A UI CLASS LATER #### //
        bool active_selection = false;
        // ############################################# //
        // ############################################# //

        public Engine(string[] args)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Load the settings file
            Settings.LoadFromFile();
            // Parse any command line arguments
            Settings.ParseCommandLineArguments(args);

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
            // Load the localization file
            Localization.LoadLocalization();

            // Create the camera
            Camera.Create(GraphicsDevice.Viewport);

            // Create a new world object
            WORLD = new World();

            //This should be loaded in from Save File. Hardcoded for now.
            WORLD.CrewMembers.Add(new CrewMember("James", 23, 90, 60, "crew"));
            WORLD.CrewMembers.Add(new CrewMember("John", 25, 250, 160, "crew"));
            WORLD.CrewMembers.Add(new CrewMember("Joe", 33, 650, 300, "crew"));
            WORLD.CrewMembers.Add(new CrewMember("Jim", 27, 480, 100, "crew"));
            WORLD.CrewMembers.Add(new CrewMember("Jack", 21, 790, 333, "crew"));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the base textures
            Sprites.MISSING_TEXTURE = Content.Load<Texture2D>("Textures/missing");
            Sprites.PIXEL = Content.Load<Texture2D>("Textures/pixel");

            // Load all fonts
            Fonts.LoadFontBank("Content/Data/fontbank.txt", Content);

            // Load all game sprites
            Sprites.LoadSpriteBank("Content/Data/spritebank.txt", Content);

            // Load all sound effects
            Audio.LoadSoundBank("Content/Data/soundbank.txt", Content);

            // Enable the FPS counter
            FrameRateCounter.Enable();
            //Enable Crew Stats
            DebugCrewStats.Enable();
            // Enable version display
            Version.Enable();


            // #### TESTING BELOW ####
            sand = Content.Load<Texture2D>("Textures/sand");
            Texture2D tex = Content.Load<Texture2D>("Textures/button");
            Rectangle screen = GraphicsDevice.Viewport.Bounds;
            int buttonSize = 60;
            int buttomRoom = 20;

            button1 = new Button("testButton1", "\n\n\n\n\n\nCrew", new Rectangle(20, (screen.Height - buttonSize) - buttomRoom, buttonSize, buttonSize), tex, Fonts.Get("ButtonText"), Color.White);
            button1.onClick += new EHandler(Event_Test);
            button1.onMouseEnter += new EHandler(Do_Beep);

            button2 = new Button("testButton2", "\n\n\n\n\n\nBase", new Rectangle(100, (screen.Height - buttonSize) - buttomRoom, buttonSize, buttonSize), tex, Fonts.Get("ButtonText"), Color.White);
            button2.onClick += new EHandler(Event_Test);
            button2.onMouseEnter += new EHandler(Do_Beep);

            button3 = new Button("testButton3", "\n\n\n\n\n\nBuild", new Rectangle(180, (screen.Height - buttonSize) - buttomRoom, buttonSize, buttonSize), tex, Fonts.Get("ButtonText"), Color.White);
            button3.onClick += new EHandler(Event_Test);
            button3.onMouseEnter += new EHandler(Do_Beep);

            button4 = new Button("testButton4", "\n\n\n\n\n\nTasks", new Rectangle(260, (screen.Height - buttonSize) - buttomRoom, buttonSize, buttonSize), tex, Fonts.Get("ButtonText"), Color.White);
            button4.onClick += new EHandler(Event_Test);
            button4.onMouseEnter += new EHandler(Do_Beep);

            button5 = new Button("testButton5", "\n\n\n\n\n\nMissions", new Rectangle(340, (screen.Height - buttonSize) - buttomRoom, buttonSize, buttonSize), tex, Fonts.Get("ButtonText"), Color.White);
            button5.onClick += new EHandler(Event_Test);
            button5.onMouseEnter += new EHandler(Do_Beep);

            int barSize = ((screen.Height - buttonSize) - buttomRoom) + (buttonSize / 2);
            bottomBar = new Rectangle(0, barSize, screen.Width, barSize);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            Controls.Update();
            Camera.Update(gameTime);
            WORLD.Clock.Update(gameTime, ClockSpeed.RealTime);

            if (Controls.Mouse.IsInCameraView()) // Don't do anything with the mouse if it's not in our cameras viewport.
            {
                Vector2 mousePosition = new Vector2(Controls.Mouse.X, Controls.Mouse.Y);
                mousePosition = Vector2.Transform(mousePosition, Camera.InverseTransform);

                int x = (int)(mousePosition.X / Constants.TILE_SIZE);
                int y = (int)(mousePosition.Y / Constants.TILE_SIZE);

                this.Window.Title = "DEBUG - " +
                    "(Mouse: " + (int)mousePosition.X + ":" + (int)mousePosition.Y + ") " +
                    "(Tile: " + x + ":" + y + ") " +
                    "(Camera Position:{X:" + Camera.Position.X.ToString("N2") + " " + Camera.Position.Y.ToString("N2") + "} - Zoom:" + Camera.Zoom.ToString("N3") + ") " +
                    "(" + WORLD.Clock.DebugText + ")";
                
                //Update crew in real time.
                foreach (CrewMember c in WORLD.CrewMembers)
                {
                    c.Update(gameTime);
                }

                if (Controls.Mouse.LeftButton == ButtonState.Pressed && Controls.MouseOld.LeftButton == ButtonState.Released)
                {
                    foreach (CrewMember c in WORLD.CrewMembers)
                    {
                        //If the mouseposition is within the textures bounds of a crew member...
                        //This could be done radius based instead of texture bounds based to make it simpler, but might not work then for long rectangular objects such as solar panels or something.
                        if (mousePosition.X > c.Position.X - (Sprites.MISSING_TEXTURE.Bounds.Width / 2) && mousePosition.X < c.Position.X + (Sprites.MISSING_TEXTURE.Bounds.Width / 2))
                        {
                            if (mousePosition.Y > c.Position.Y - (Sprites.MISSING_TEXTURE.Bounds.Height / 2) && mousePosition.Y < c.Position.Y + (Sprites.MISSING_TEXTURE.Bounds.Height / 2))
                            {
                                //deselect any crew that are already selected.
                                foreach (CrewMember cm in WORLD.CrewMembers)
                                {
                                    if (cm.Selected)
                                    {
                                        cm.Selected = false;
                                    }
                                }
                                //TODO: Some more formal UI class will need to handle when things are selected, not the object itself.
                                c.Selected = true;
                                active_selection = true;

                                Audio.Play("high_beep");
                                Console.WriteLine(c.Name + " " + " has been selected");

                                break;
                            }
                        }
                    }
                }
                else if (Controls.Mouse.RightButton == ButtonState.Pressed && Controls.Keyboard.IsKeyDown(Keys.LeftControl))
                {
                    if ((x < 0 || y < 0 || x >= Constants.MAP_WIDTH || y >= Constants.MAP_HEIGHT) == false)
                    {
                        WORLD.Tiles[x, y].Type = TileType.Cliff;
                    }
                }
                else if (Controls.Mouse.RightButton == ButtonState.Pressed && Controls.MouseOld.RightButton == ButtonState.Released)
                {
                    if (active_selection)
                    {
                        foreach (CrewMember cm in WORLD.CrewMembers)
                        {
                            if (cm.Selected)
                            {
                                Point start_location = new Point((int)cm.Position.X / Constants.TILE_SIZE, (int)cm.Position.Y / Constants.TILE_SIZE);
                                Point destination = new Point(x, y);
                                Console.WriteLine("Generating Path from point (" + start_location.X + "," + start_location.Y + ") to point (" + destination.X + "," + destination.Y + ")");
                                cm.DeterminePath(WORLD.FindPath(start_location, destination));
                            }
                        }
                    }
                }
            }

            // Check for exit.
            if(Controls.Keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // ##### TESTING #####
            button1.Update(Controls.Mouse, Controls.Keyboard);
            button2.Update(Controls.Mouse, Controls.Keyboard);
            button3.Update(Controls.Mouse, Controls.Keyboard);
            button4.Update(Controls.Mouse, Controls.Keyboard);
            button5.Update(Controls.Mouse, Controls.Keyboard);
            // ###################

            FrameRateCounter.Update(gameTime);

            DebugCrewStats.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backColour);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.Transform);

            //spriteBatch.Draw(sand, sand.Bounds, Color.White);
            spriteBatch.Draw(sand, new Rectangle(0,0, Constants.MAP_WIDTH * Constants.TILE_SIZE, Constants.MAP_HEIGHT * Constants.TILE_SIZE), Color.White);
            bool showGrid = true;

            if (showGrid)
            {
                for (int x = 0; x < Constants.MAP_WIDTH; x++)
                {
                    for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                    {
                        Rectangle tileRectangle = new Rectangle(
                            x * Constants.TILE_SIZE, y * Constants.TILE_SIZE,
                            Constants.TILE_SIZE, Constants.TILE_SIZE);

                        //spriteBatch.Draw(Sprites.Get(WORLD.Tiles[x, y].Texture, 0), tileRectangle, Color.White);

                        if (WORLD.Tiles[x, y].Type == TileType.Empty)
                        {
                            spriteBatch.DrawRectangle(tileRectangle, Color.Black * 0.3f, 1 / Camera.Zoom);
                        }
                        else
                        {
                            spriteBatch.FillRectangle(tileRectangle, Color.Black);
                        }
                    }
                }
            }

            //Draw every crew members sprite.
            foreach (CrewMember crew_member in WORLD.CrewMembers)
            {
                //Drawing the texture at the position minus the width and height to center it. This will be done in the objects class in future.
                Rectangle re = new Rectangle((int)crew_member.Position.X - (Sprites.Get(crew_member.Sprite).Width / 2), (int)crew_member.Position.Y - (Sprites.Get(crew_member.Sprite).Height / 2), 64, 64);
                spriteBatch.Draw(Sprites.Get(crew_member.Sprite), re, Color.White);

                //If a crew member is selected then draw a circle around them.
                if (crew_member.Selected)
                {
                    spriteBatch.DrawCircle(crew_member.Position, 32, 20, Color.LightGreen);
                    
                    //DEBUG: Display their needs.
                    

                    //If a crew has a path then display it when they are selected.
                    if (crew_member.Path.Count > 0)
                    {
                        for (int i = 0; i < crew_member.Path.Count; i++)
                        {
                            Rectangle tileRectangle = new Rectangle(
                                crew_member.Path.ElementAt(i).Position.X * Constants.TILE_SIZE, crew_member.Path.ElementAt(i).Position.Y * Constants.TILE_SIZE,
                                Constants.TILE_SIZE, Constants.TILE_SIZE);

                            //spriteBatch.FillRectangle(tileRectangle, Color.DarkBlue);

                            if (i + 1 < crew_member.Path.Count)
                            {
                                Tile thisTile = new Tile(crew_member.Path.ElementAt(i).Position.X, crew_member.Path.ElementAt(i).Position.Y);
                                Tile nextTile = new Tile(crew_member.Path.ElementAt(i + 1).Position.X, crew_member.Path.ElementAt(i + 1).Position.Y);

                                spriteBatch.DrawLine(thisTile.Center,nextTile.Center,Color.White, 1 / Camera.Zoom);
                            }
                        }
                    }

                    /* This is probably just debug for now, but it's the code for drawing in the Red and Green Square for pathfinding.
                     * 
                     */
                    if (crew_member.StartTile != Point.Zero)
                    {
                        Rectangle tileRectangle = new Rectangle(
                            crew_member.StartTile.X * Constants.TILE_SIZE, crew_member.StartTile.Y * Constants.TILE_SIZE,
                            Constants.TILE_SIZE, Constants.TILE_SIZE);

                        spriteBatch.FillRectangle(tileRectangle, Color.Green);
                    }
                    if (crew_member.EndTile != Point.Zero)
                    {
                        Rectangle tileRectangle = new Rectangle(
                            crew_member.EndTile.X * Constants.TILE_SIZE, crew_member.EndTile.Y * Constants.TILE_SIZE,
                            Constants.TILE_SIZE, Constants.TILE_SIZE);

                        spriteBatch.FillRectangle(tileRectangle, Color.Red);
                    }

                }
            }

            spriteBatch.End();

            // ##### TESTING #####
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(Sprites.PIXEL, bottomBar, Color.Black * 0.75f);
            button1.Draw(spriteBatch);
            button2.Draw(spriteBatch);
            button3.Draw(spriteBatch);
            button4.Draw(spriteBatch);
            button5.Draw(spriteBatch);
            spriteBatch.End();
            // ##################

            // ##### FOR DEBUG #####
            foreach (CrewMember crew in WORLD.CrewMembers)
            {
                if(crew.Selected)
                {
                    DebugCrewStats.Draw(spriteBatch, crew);
                }
            }
            // #####################

            FrameRateCounter.Draw(spriteBatch);
            Version.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
