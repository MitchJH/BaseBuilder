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

        Point startTile = Point.Zero;
        Point endTile = Point.Zero;
        LinkedList<Tile> path = new LinkedList<Tile>();
        List<Vector2> curve = new List<Vector2>();
        float frameRate;

        Texture2D back;
        Texture2D front;
        Texture2D left;
        Texture2D right;

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

            crew.Position = Vector2.Zero;
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Sprites.MISSING_TEXTURE = Content.Load<Texture2D>("Textures/missing");
            Sprites.PIXEL = Content.Load<Texture2D>("Textures/pixel");

            back = Content.Load<Texture2D>("Textures/p_back");
            front = Content.Load<Texture2D>("Textures/p_front");
            left = Content.Load<Texture2D>("Textures/p_left");
            right = Content.Load<Texture2D>("Textures/p_right");
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            Controls.Update();
            Camera.Update(gameTime);

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
                    "(FPS: " + frameRate + ")";
                
                // Check for a left mouse click
                if (Controls.Mouse.LeftButton == ButtonState.Pressed && Controls.MouseOld.LeftButton == ButtonState.Released)
                {
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
                    bool arrived = crew.Move(gameTime);

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

            Vector2 p = new Vector2(crew.Position.X - (Constants.TILE_SIZE / 2), crew.Position.Y - (Constants.TILE_SIZE / 2));
            //spriteBatch.DrawCircle(p, 5, 20, Color.Purple);

            Facing face = crew.Facing;

            Rectangle re = new Rectangle((int)p.X, (int)p.Y, 64, 64);

            if (face == Facing.Back)
            {
                spriteBatch.Draw(back, re, Color.White);
            }
            else if (face == Facing.Left)
            {
                spriteBatch.Draw(left, re, Color.White);
            }
            else if (face == Facing.Right)
            {
                spriteBatch.Draw(right, re, Color.White);
            }
            else
            {
                spriteBatch.Draw(front, re, Color.White);
            }

            spriteBatch.End();

            frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Draw(gameTime);
        }
    }
}
