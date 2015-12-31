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
        EntityCollider _entity_collider;

        // Menus
        MainMenu mainMenu;

        // ############################################# //
        // ######## EVERYTHING HERE NEEDS TO GO ######## //
        // ############################################# //
        PhysicsEntity physics_thing;

        Button button1;
        Button button2;
        Button button3;
        Button button4;
        Button button5;
        Rectangle topBar;
        Rectangle thickBar;
        Rectangle bottomBar;
        Texture2D sand;
        Label clock;
        public void Event_Test(GUIControl sender)
        {
            Random rand = new Random();

            for (int i = 0; i < 10; i++)
            {
                CrewMember newCrew = new CrewMember("test" + i, 20 + i, rand.Next(10, 800), rand.Next(10, 500), "crew");
                World.CrewMembers.Add(newCrew);
            }
            Audio.PlaySoundEffect("low_double_beep");
        }
        public void Do_Beep(GUIControl sender)
        {
            Audio.PlaySoundEffect("click");
        }
        public void Bed_Test(GUIControl sender)
        {
            ObjectManager.GetType("bed1").Sprite = "bed2";
            Audio.PlaySoundEffect("low_double_beep");
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
            // Load the localization file
            Localization.LoadLocalization();

            // Create the camera
            Camera.Create(GraphicsDevice.Viewport);

            //This should be loaded in from Save File. Hardcoded for now.
            World.CrewMembers.Add(new CrewMember("James", 23, 90, 60, "crew"));
            World.CrewMembers.Add(new CrewMember("John", 25, 250, 160, "crew"));
            World.CrewMembers.Add(new CrewMember("Joe", 33, 650, 300, "crew"));
            World.CrewMembers.Add(new CrewMember("Jim", 27, 480, 100, "crew"));
            World.CrewMembers.Add(new CrewMember("Jack", 21, 790, 333, "crew"));

            physics_thing = new PhysicsEntity(new Vector2(100, 0));

            //Initialize the EntityCollider
            _entity_collider = new EntityCollider();

            //Add all objects that can collide to the entity collider. Do this somewhere else later, and in real time as objects get added/removed from the world.
            foreach(Entity e in World.CrewMembers)
            {
                _entity_collider.Add(e);
            }

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

            // Load all object types
            ObjectManager.LoadObjectBank("Content/Data/objectbank.txt", Content);

            //This should be loaded in from Save File. Hardcoded for now. Needs to be loaded after the ObjectBank is read.
            World.InternalObjects.Add(new GameObject("ID_1", "bed1", new Vector2(10, 10)));
            World.InternalObjects.Add(new GameObject("ID_2", "bed1", new Vector2(20, 10)));
            World.InternalObjects.Add(new GameObject("ID_3", "bed1", new Vector2(30, 10)));
            World.InternalObjects.Add(new GameObject("ID_4", "bed1", new Vector2(40, 10)));
            World.InternalObjects.Add(new GameObject("ID_5", "bed1", new Vector2(10, 20)));
            World.InternalObjects.Add(new GameObject("ID_6", "bed1", new Vector2(20, 20)));
            World.InternalObjects.Add(new GameObject("ID_7", "bed2", new Vector2(30, 20)));
            World.InternalObjects.Add(new GameObject("ID_8", "bed3", new Vector2(40, 20)));

            // Enable the FPS counter
            FrameRateCounter.Enable();
            //Enable Crew Stats
            DebugCrewStats.Enable();
            // Enable version display
            Version.Enable();

            // Create Menus
            mainMenu = new MainMenu(GraphicsDevice.Viewport.Bounds, Content);

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
            button5.onClick += new EHandler(Bed_Test);
            button5.onMouseEnter += new EHandler(Do_Beep);

            int barSize = ((screen.Height - buttonSize) - buttomRoom) + (buttonSize / 2);
            bottomBar = new Rectangle(0, barSize, screen.Width, barSize);
            topBar = new Rectangle(0, 0, screen.Width, 22);
            thickBar = new Rectangle(0, 22, screen.Width, 2);

            Vector2 clockPos = new Vector2(screen.Width / 2, 1);
            clock = new Label("clock", "00:00", clockPos, Fonts.Standard, Color.White, 5, 0);

            // Testing time by moving clock to when sun sets
            World.Clock.SetClock(0, 0, 15, 55, 0);
            // Speed clock up a bit
            World.Clock.SetSpeed(ClockSpeed.MinutesPerSecond);

            // Test music
            MediaPlayer.IsRepeating = true;
            Audio.PlayMusicTrack("background");
        }

        protected override void UnloadContent()
        {
            Settings.WriteToFile();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GameStateManager.State == GameState.Game)
            {
            Controls.Update();
            Camera.Update(gameTime);
            World.Clock.Update(gameTime);
            clock.Text = World.Clock.Time;

            _entity_collider.Update(gameTime);

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
                    "(" + World.Clock.DebugText + ")";
                
                //Update crew in real time.
                foreach (CrewMember c in World.CrewMembers)
                {
                    c.Update(gameTime);
                }

                if (Controls.Mouse.LeftButton == ButtonState.Pressed && Controls.MouseOld.LeftButton == ButtonState.Released)
                {
                    foreach (CrewMember c in World.CrewMembers)
                    {
                        //If the mouseposition is within the textures bounds of a crew member...
                        //This could be done radius based instead of texture bounds based to make it simpler, but might not work then for long rectangular objects such as solar panels or something.
                        if (mousePosition.X > c.Position.X - (Sprites.MISSING_TEXTURE.Bounds.Width / 2) && mousePosition.X < c.Position.X + (Sprites.MISSING_TEXTURE.Bounds.Width / 2))
                        {
                            if (mousePosition.Y > c.Position.Y - (Sprites.MISSING_TEXTURE.Bounds.Height / 2) && mousePosition.Y < c.Position.Y + (Sprites.MISSING_TEXTURE.Bounds.Height / 2))
                            {
                                //deselect any crew that are already selected.
                                foreach (CrewMember cm in World.CrewMembers)
                                {
                                    if (cm.Selected)
                                    {
                                        cm.Selected = false;
                                    }
                                }
                                //TODO: Some more formal UI class will need to handle when things are selected, not the object itself.
                                c.Selected = true;
                                active_selection = true;

                                Audio.PlaySoundEffect("high_beep");
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
                        World.Tiles[x, y].Type = TileType.Impassable;
                    }
                }
                else if (Controls.Mouse.RightButton == ButtonState.Pressed && Controls.MouseOld.RightButton == ButtonState.Released)
                {
                    if (active_selection)
                    {
                        foreach (CrewMember cm in World.CrewMembers)
                        {
                            if (cm.Selected)
                            {
                                Point start_location = new Point((int)cm.Position.X / Constants.TILE_SIZE, (int)cm.Position.Y / Constants.TILE_SIZE);
                                Point destination = new Point(x, y);
                                Console.WriteLine("Generating Path from point (" + start_location.X + "," + start_location.Y + ") to point (" + destination.X + "," + destination.Y + ")");
                                cm.DeterminePath(World.FindPath(start_location, destination));
                            }
                        }
                    }
                }
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

                // Check for exit.
                if (Controls.Keyboard.IsKeyDown(Keys.Escape))
                {
                    GameStateManager.State = GameState.Exit;
                }
            }
            else if (GameStateManager.State == GameState.MainMenu)
            {
                Controls.Update();
                mainMenu.Update();
            }

            if (GameStateManager.State == GameState.Exit)
            {
                Exit();
            }

            physics_thing.Update(gameTime);

            if (Controls.Keyboard.IsKeyDown(Keys.F))
            {
                physics_thing.ApplyForce(new Vector2(-1, 0));
            }
            if (Controls.Keyboard.IsKeyDown(Keys.T))
            {
                physics_thing.ApplyForce(new Vector2(0, -1));
            }
            if (Controls.Keyboard.IsKeyDown(Keys.H))
            {
                physics_thing.ApplyForce(new Vector2(1, 0));
            }
            if (Controls.Keyboard.IsKeyDown(Keys.G))
            {
                physics_thing.ApplyForce(new Vector2(0, 1));
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GameStateManager.State == GameState.Game)
            {
            GraphicsDevice.Clear(backColour);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.Transform);

                spriteBatch.Draw(sand, new Rectangle(0, 0, Constants.MAP_WIDTH * Constants.TILE_SIZE, Constants.MAP_HEIGHT * Constants.TILE_SIZE), World.Clock.AmbientLightFromTime);

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

                        if (World.Tiles[x, y].Type == TileType.Walkable)
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
            spriteBatch.DrawCircle(physics_thing.Position, 32, 20, Color.Red);

            //Draw every crew members sprite.
            foreach (CrewMember crew_member in World.CrewMembers)
            {
                //Drawing the texture at the position minus the width and height to center it. This will be done in the objects class in future.
                Rectangle re = new Rectangle((int)crew_member.Position.X - (Sprites.Get(crew_member.Sprite).Width / 2), (int)crew_member.Position.Y - (Sprites.Get(crew_member.Sprite).Height / 2), 64, 64);
                spriteBatch.Draw(Sprites.Get(crew_member.Sprite), re, World.Clock.AmbientLightFromTime);

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

                                    spriteBatch.DrawLine(thisTile.Center, nextTile.Center, Color.White, 1 / Camera.Zoom);
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

            foreach (GameObject io in World.InternalObjects)
            {
                Rectangle re2 = new Rectangle((int)io.Position.X, (int)io.Position.Y, Sprites.Get(io.ObjectType.Sprite).Width, Sprites.Get(io.ObjectType.Sprite).Height);
                spriteBatch.Draw(Sprites.Get(io.ObjectType.Sprite), re2, World.Clock.AmbientLightFromTime);
            }

            spriteBatch.End();

            // ##### TESTING #####
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(Sprites.PIXEL, topBar, Color.Black * 0.75f);
            spriteBatch.Draw(Sprites.PIXEL, bottomBar, Color.Black * 0.75f);
            spriteBatch.Draw(Sprites.PIXEL, thickBar, Color.Black);
            button1.Draw(spriteBatch);
            button2.Draw(spriteBatch);
            button3.Draw(spriteBatch);
            button4.Draw(spriteBatch);
            button5.Draw(spriteBatch);
            clock.Draw(spriteBatch);
            spriteBatch.End();
            // ##################

            // ##### FOR DEBUG #####
            foreach (CrewMember crew in World.CrewMembers)
            {
                    if (crew.Selected)
                {
                    DebugCrewStats.Draw(spriteBatch, crew);
                }
            }
            // #####################

            FrameRateCounter.Draw(spriteBatch);
            Version.Draw(spriteBatch);
            }
            else if (GameStateManager.State == GameState.MainMenu)
            {
                GraphicsDevice.Clear(Color.Black);
                mainMenu.Draw(spriteBatch);
                Version.Draw(spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
