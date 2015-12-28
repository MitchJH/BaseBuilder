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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Engine : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        World WORLD;

        // TODO: Move FPS/performances counters to own class
        float frameRate;

        // ############################################# //
        // ######## EVERYTHING HERE NEEDS TO GO ######## //
        // ############################################# //
        CrewMember crew = new CrewMember();
        List<CrewMember> crew_members;

        Point startTile = Point.Zero;
        Point endTile = Point.Zero;
        LinkedList<Tile> path = new LinkedList<Tile>();
        List<Vector2> curve = new List<Vector2>();
        // ############################################# //
        // ############################################# //

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            // Load the settings file
            Settings.Load();

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

            // Load the localization file
            Localization.LoadLocalization();

            // Create the camera
            Camera.Create(GraphicsDevice.Viewport);

            // Create a new world object
            WORLD = new World();

            //Initialzing crew members.
            crew_members = new List<CrewMember>();
            //This should be loaded in from Save File. Hardcoded for now.
            crew_members.Add(new CrewMember("James", 23, 0, 0, "crew"));
            crew_members.Add(new CrewMember("John", 25, 250, 160, "crew"));
            crew_members.Add(new CrewMember("Joe", 33, 650, 300, "crew"));
            crew_members.Add(new CrewMember("Jim", 27, 480, 100, "crew"));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the base textures
            Sprites.MISSING_TEXTURE = Content.Load<Texture2D>("Textures/missing");
            Sprites.PIXEL = Content.Load<Texture2D>("Textures/pixel");
            // Load all game sprites
            Sprites.LoadSpriteBank("Content/Data/spritebank.txt", Content);
            // Load all sound effects
            Audio.LoadSoundBank("Content/Data/soundbank.txt", Content);
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

                Facing f = crew.Facing;

                this.Window.Title = "DEBUG - " +
                    "(Mouse: " + (int)mousePosition.X + ":" + (int)mousePosition.Y + ") " +
                    "(Tile: " + x + ":" + y + ") " +
                    "(Camera: Position:" + Camera.Position + " - Zoom:" + Camera.Zoom + ") " +
                    "(FPS: " + frameRate.ToString("N0") + ") " +
                    "(" + WORLD.Clock.DebugText + ")";
                
                // Check for a left mouse click
                if (Controls.Mouse.LeftButton == ButtonState.Pressed && Controls.MouseOld.LeftButton == ButtonState.Released)
                {
                    foreach (CrewMember crew_member in crew_members)
                    {
                        //If the mouseposition is within the textures bounds of a crew member...
                        //This could be done radius based instead of texture bounds based to make it simpler, but might not work then for long rectangular objects such as solar panels or something.
                        if (mousePosition.X > crew_member.Position.X - (Sprites.Get(crew_member.Sprite).Bounds.Width / 2) && mousePosition.X < crew_member.Position.X + (Sprites.Get(crew_member.Sprite).Bounds.Width / 2))
                        {
                            if (mousePosition.Y > crew_member.Position.Y - (Sprites.Get(crew_member.Sprite).Bounds.Height / 2) && mousePosition.Y < crew_member.Position.Y + (Sprites.Get(crew_member.Sprite).Bounds.Height / 2))
                            {
                                //deselect any crew that are already selected.
                                foreach (CrewMember cm in crew_members)
                                {
                                    if (cm.Selected)
                                    {
                                        cm.Selected = false;
                                    }
                                }

                                //TODO: Some more formal UI class will need to handle when things are selected, not the object itself.
                                crew_member.Selected = true;
                                Audio.Play("high_beep");
                                Console.WriteLine(crew_member.Name + " " + " has been selected");
                                
                                break;
                            }
                        }
                    }

                    if (path.Count > 0)
                    {
                        path.Clear();
                        startTile = Point.Zero;
                        endTile = Point.Zero;
                    }
                    else
                    {
                        if (startTile == Point.Zero)
                        {
                            startTile = new Point(x, y);
                        }
                        else if (endTile == Point.Zero)
                        {
                            endTile = new Point(x, y);
                            path = WORLD.FindPath(startTile, endTile);

                            if (path == null)
                            {
                                // NO PATH
                                path = new LinkedList<Tile>();
                                startTile = Point.Zero;
                                endTile = Point.Zero;
                                Audio.Play("low_double_beep");
                            }
                            else
                            {
                                crew.Position = new Vector2(startTile.X * Constants.TILE_SIZE, startTile.Y * Constants.TILE_SIZE);
                                crew._waypoint = 1;
                                crew.Destination = new Vector2(path.ElementAt(crew._waypoint).Position.X * Constants.TILE_SIZE, path.ElementAt(crew._waypoint).Position.Y * Constants.TILE_SIZE);
                                Audio.Play("high_double_beep");
                            }
                        }
                    }
                }
                else if (Controls.Mouse.RightButton == ButtonState.Pressed)
                {
                    if ((x < 0 || y < 0 || x >= Constants.MAP_WIDTH || y >= Constants.MAP_HEIGHT) == false)
                    {
                        WORLD.Tiles[x, y].Type = TileType.Cliff;
                    }
                }
            }

            if (path != null)
            {
                if (path.Count > 0)
                {
                    bool arrived = crew.Update(gameTime);

                    if (arrived)
                    {
                        if (crew._waypoint+1 < path.Count)
                        {
                            crew._waypoint++;
                            crew.Destination = new Vector2(path.ElementAt(crew._waypoint).Position.X * Constants.TILE_SIZE, path.ElementAt(crew._waypoint).Position.Y * Constants.TILE_SIZE);
                        }
                    }
                }
            }

            // Check for exit.
            if (Controls.Keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.Transform);

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
                        spriteBatch.DrawRectangle(tileRectangle, Color.Black, 1 / Camera.Zoom);
                    }
                    else
                    {
                        spriteBatch.FillRectangle(tileRectangle, Color.Black);
                    }
                }
            }

            if (path.Count > 0)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    Rectangle tileRectangle = new Rectangle(
                        path.ElementAt(i).Position.X * Constants.TILE_SIZE, path.ElementAt(i).Position.Y * Constants.TILE_SIZE,
                        Constants.TILE_SIZE, Constants.TILE_SIZE);

                    //spriteBatch.FillRectangle(tileRectangle, Color.DarkBlue);

                    if (i + 1 < path.Count)
                    {
                        Tile thisTile = new Tile(path.ElementAt(i).Position.X, path.ElementAt(i).Position.Y);
                        Tile nextTile = new Tile(path.ElementAt(i + 1).Position.X, path.ElementAt(i + 1).Position.Y);

                        spriteBatch.DrawLine(
                            thisTile.Center,
                            nextTile.Center,
                            Color.White, 1 / Camera.Zoom);
                    }
                }

                

                foreach (Vector2 v in curve)
                {
                    spriteBatch.DrawCircle(v, 3, 20, Color.Red);
                }
            }

            Vector2 p = new Vector2(crew.Position.X, crew.Position.Y);
            Rectangle re = new Rectangle((int)p.X, (int)p.Y, 64, 64);

            //temporary crew member.
            spriteBatch.Draw(Sprites.Get(crew.Sprite), re, Color.White);

            //Draw every crew members sprite.
            foreach(CrewMember crew_member in crew_members)
            {
                p = new Vector2(crew_member.Position.X, crew_member.Position.Y);
                
                //Drawing the texture at the position minus the width and height to center it. This will be done in the objects class in future.
                re = new Rectangle((int)p.X - (Sprites.Get(crew_member.Sprite).Width / 2), (int)p.Y - (Sprites.Get(crew_member.Sprite).Height / 2), 64, 64);
                spriteBatch.Draw(Sprites.Get(crew_member.Sprite), re, Color.White);

                //If a crew member is selected then draw a circle around them.
                if (crew_member.Selected)
                {
                    spriteBatch.DrawCircle(crew_member.Position, 32, 20, Color.LightGreen);
                }
            }

            if (startTile != Point.Zero)
            {
                Rectangle tileRectangle = new Rectangle(
                    startTile.X * Constants.TILE_SIZE, startTile.Y * Constants.TILE_SIZE,
                    Constants.TILE_SIZE, Constants.TILE_SIZE);

                spriteBatch.FillRectangle(tileRectangle, Color.Green);
            }
            if (endTile != Point.Zero)
            {
                Rectangle tileRectangle = new Rectangle(
                    endTile.X * Constants.TILE_SIZE, endTile.Y * Constants.TILE_SIZE,
                    Constants.TILE_SIZE, Constants.TILE_SIZE);

                spriteBatch.FillRectangle(tileRectangle, Color.Red);
            }

            

            spriteBatch.End();

            frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Draw(gameTime);
        }
    }
}
