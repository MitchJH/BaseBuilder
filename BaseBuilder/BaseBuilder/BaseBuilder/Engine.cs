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

        CrewMember crew = new CrewMember();

        //A test list of crew members.
        List<CrewMember> crew_members;

        Point startTile = Point.Zero;
        Point endTile = Point.Zero;
        LinkedList<Tile> path = new LinkedList<Tile>();
        List<Vector2> curve = new List<Vector2>();
        float frameRate;

        Texture2D default_texture;

        public Engine()
        {
            graphics = new GraphicsDeviceManager(this);

            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;

            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

#if DEBUG
            double windowScale = 0.8;
            this.graphics.PreferredBackBufferWidth = (int)(screenWidth * windowScale);
            this.graphics.PreferredBackBufferHeight = (int)(screenHeight * windowScale);
#else
            this.graphics.IsFullScreen = true;
            this.graphics.PreferredBackBufferWidth = screenWidth;
            this.graphics.PreferredBackBufferHeight = screenHeight;
#endif

            Content.RootDirectory = "Content";
        }
        
        protected override void Initialize()
        {
            Localization.LoadLocalization();
            Camera.Create(GraphicsDevice.Viewport);
            WORLD = new World();

            //Initialzing crew members.
            crew_members = new List<CrewMember>();
            //This should be loaded in from Save File. Hardcoded for now.
            crew_members.Add(new CrewMember("James", 23, 0, 0));
            crew_members.Add(new CrewMember("John", 25, 250, 160));
            crew_members.Add(new CrewMember("Joe", 33, 650, 300));
            crew_members.Add(new CrewMember("Jim", 27, 480, 100));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Sprites.MISSING_TEXTURE = Content.Load<Texture2D>("Textures/missing");
            Sprites.PIXEL = Content.Load<Texture2D>("Textures/pixel");

            default_texture = Content.Load<Texture2D>("Textures/p_front");
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
                    foreach (CrewMember c in crew_members)
                    {
                        
                        //If the mouseposition is within the textures bounds of a crew member...
                        //This could be done radius based instead of texture bounds based to make it simpler, but might not work then for long rectangular objects such as solar panels or something.
                        if (mousePosition.X > c.Position.X - (default_texture.Bounds.Width / 2) && mousePosition.X < c.Position.X + (default_texture.Bounds.Width / 2))
                        {
                            if (mousePosition.Y > c.Position.Y - (default_texture.Bounds.Height / 2) && mousePosition.Y < c.Position.Y + (default_texture.Bounds.Height / 2))
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
                                c.Selected = true;
                                Console.WriteLine(c.Name + " " + " has been selected");
                                
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
                            }
                            else
                            {
                                crew.Position = new Vector2(startTile.X * Constants.TILE_SIZE, startTile.Y * Constants.TILE_SIZE);
                                crew._waypoint = 1;
                                crew.Destination = new Vector2(path.ElementAt(crew._waypoint).Position.X * Constants.TILE_SIZE, path.ElementAt(crew._waypoint).Position.Y * Constants.TILE_SIZE);
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
            spriteBatch.Draw(default_texture, re, Color.White);

            //Draw every crew members sprite.
            for (int i = 0; i < crew_members.Count; i++)
            {
                p = new Vector2(crew_members[i].Position.X, crew_members[i].Position.Y);
                
                //Drawing the texture at the position minus the width and height to center it. This will be done in the objects class in future.
                re = new Rectangle((int)p.X - (default_texture.Width / 2), (int)p.Y - (default_texture.Height / 2), 64, 64);
                spriteBatch.Draw(default_texture, re, Color.White);

                //If a crew member is selected then draw a circle around them.
                if (crew_members[i].Selected)
                {
                    spriteBatch.DrawCircle(crew_members[i].Position, 32, 20, Color.LightGreen);
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
