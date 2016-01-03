using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BaseBuilder
{
    /// <summary>
    /// Holds the current gamestate, used for savegame serialization.
    /// </summary>
    public class World
    {
        private static Tile[,] _tiles;
        private static Clock _clock;
        private static List<CrewMember> _crewMembers;
        private static List<GameObject> _objects;
        private static List<GameStructure> _structures;
        private static Planet _planet;


        // ############################################# //
        // ######## EVERYTHING HERE NEEDS TO GO ######## //
        // ############################################# //

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

        public World(Rectangle screen, ContentManager content)
        {
            _tiles = new Tile[Constants.MAP_WIDTH, Constants.MAP_HEIGHT];
            _clock = new Clock();
            _crewMembers = new List<CrewMember>();
            _objects = new List<GameObject>();
            _structures = new List<GameStructure>();
            EmptyMap();

            // #### TESTING BELOW ####
            sand = content.Load<Texture2D>("Textures/sand");
            Texture2D tex = content.Load<Texture2D>("Textures/button");
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

            //This should be loaded in from Save File. Hardcoded for now.
            World.CrewMembers.Add(new CrewMember("James", 23, 90, 60, "crew"));
            World.CrewMembers.Add(new CrewMember("John", 25, 250, 160, "crew"));
            //World.CrewMembers.Add(new CrewMember("Joe", 33, 650, 300, "crew"));
            //World.CrewMembers.Add(new CrewMember("Jim", 27, 480, 100, "crew"));
            //World.CrewMembers.Add(new CrewMember("Jack", 21, 790, 333, "crew"));

            //Add all objects that can collide to the entity collider. Do this somewhere else later, and in real time as objects get added/removed from the world.
            foreach (Entity e in World.CrewMembers)
            {
                if (e.Dynamic)
                {
                    EntityCollider.Add(e);
                }
            }

            // Add dummy test objects
            _objects.Add(new GameObject("ID_1", "bed1", new Vector2(10, 10)));
        }

        public void Update(GameTime gameTime)
        {
            Camera.Update(gameTime);
            World.Clock.Update(gameTime);
            clock.Text = World.Clock.Time;
            EntityCollider.Collide(gameTime);

            if (Controls.Mouse.IsInCameraView()) // Don't do anything with the mouse if it's not in our cameras viewport.
            {
                Vector2 mousePosition = Controls.GameMousePosition;

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
                    if ((Controls.MouseTilePosition.X < 0 || Controls.MouseTilePosition.Y < 0 || Controls.MouseTilePosition.X >= Constants.MAP_WIDTH || Controls.MouseTilePosition.Y >= Constants.MAP_HEIGHT) == false)
                    {
                        World.Tiles[Controls.MouseTilePosition.X, Controls.MouseTilePosition.Y].Type = TileType.Impassable;
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
                                Point destination = Controls.MouseTilePosition;
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
                GameStateManager.State = GameState.MainMenu;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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



            //Draw every crew members sprite.
            foreach (CrewMember crew_member in World.CrewMembers)
            {
                //Drawing the texture at the position minus the width and height to center it. This will be done in the objects class in future.
                Rectangle re = new Rectangle((int)crew_member.Position.X - (Sprites.Get(crew_member.Sprite).Width / 2), (int)crew_member.Position.Y - (Sprites.Get(crew_member.Sprite).Height / 2), 64, 64);
                spriteBatch.Draw(Sprites.Get(crew_member.Sprite), re, World.Clock.AmbientLightFromTime);

                //If a crew member is selected then draw a circle around them.
                if (crew_member.Selected)
                {
                    spriteBatch.DrawCircle(crew_member.Position, 32, 20, Color.LightGreen, 2);


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

                    crew_member.Draw(spriteBatch);

                }
            }

            foreach (GameObject io in World.Objects)
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
        }

        public class RayCastingResult
        {
            // Does the ray collide with the environment?
            private bool doCollide;
            // And if so, at which position?
            private Vector2 position;

            public bool DoCollide
            {
                get { return doCollide; }
                set { doCollide = value; }
            }

            public Vector2 Position
            {
                get { return position; }
                set { position = value; }
            }
        }

        /// <summary>
        /// Find a path through the world
        /// </summary>
        /// <param name="start">The tile to start the path from</param>
        /// <param name="end">The tile the path should end at</param>
        /// <param name="extraContext">Optional: Allows the passing of extra information to the IsWalkable function in the Tile object
        /// thus allowing a tile to be walkable for different entities by only changing input parameters rather than gamestate</param>
        /// <returns>A linked list of Tile objects that is the best path between the start and end tiles</returns>
        public static LinkedList<Tile> FindPath(Point start, Point end, Object extraContext = null)
        {
            if (start.X < 0 || start.Y < 0 || end.X < 0 || end.Y < 0)
            {
                return null;
            }
            else if (start.X > Constants.MAP_WIDTH || start.Y > Constants.MAP_HEIGHT || end.X > Constants.MAP_WIDTH || end.Y > Constants.MAP_HEIGHT)
            {
                return null;
            }

            // Generate a search map from our worlds tiles
            SpatialAStar<Tile, Object> aStar = new SpatialAStar<Tile, Object>(_tiles);
            // Begin the search and return the result
            LinkedList<Tile> tempPath = aStar.Search(start, end, extraContext);
            return tempPath;
            LinkedList<Tile> truePath = new LinkedList<Tile>();

            

            for (int i = 0; i < tempPath.Count; i++)
            {
                Vector2 startPos = new Vector2(start.X, start.Y);
                Vector2 endPos = new Vector2(tempPath.ElementAt(i).Position.X, tempPath.ElementAt(i).Position.Y);
                Vector2 dir = endPos - startPos;
                float length = dir.Length();
                dir.Normalize();

                RayCastingResult rcr = RayCast(startPos, dir, length);

                Ray r = new Ray(new Vector3(new Vector2(start.X, start.Y), 0.0f), new Vector3(dir, 0.0f));

                if (rcr.DoCollide == true)
                {
                    truePath.AddLast(tempPath.ElementAt(i));
                }
            }
            Console.WriteLine("Path generation successful");
            return truePath;
        }

        public static RayCastingResult RayCast(Vector2 position, Vector2 direction, float rayLength)
        {
            RayCastingResult result = new RayCastingResult();
            result.DoCollide = false;

            // Exit the function now if the ray length is 0
            if (rayLength == 0)
            {
                result.DoCollide = _tiles[(int)position.X, (int)position.Y].Type == TileType.Walkable;
                result.Position = position;

                return result;
            }

            // Get the list of points from the Bresenham algorithm
            direction.Normalize();
            List<Vector2> rayLine = BresenhamLine(position, position + (direction * rayLength));

            if (rayLine.Count > 0)
            {
                int rayPointIndex = 0;

                if (rayLine[0] != position) rayPointIndex = rayLine.Count - 1;

                // Loop through all the points starting from "position"
                while (true)
                {
                    Vector2 rayPoint = rayLine[rayPointIndex];
                    if (_tiles[(int)rayPoint.X, (int)rayPoint.Y].Type != TileType.Walkable)
                    {
                        result.Position = rayPoint;
                        result.DoCollide = true;
                        break;
                    }
                    if (rayLine[0] != position)
                    {
                        rayPointIndex--;
                        if (rayPointIndex < 0) break;
                    }
                    else
                    {
                        rayPointIndex++;
                        if (rayPointIndex >= rayLine.Count) break;
                    }
                }
            }

            return result;
        }

        // Swap the values of A and B
        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        // Returns the list of points from p0 to p1 
        private static List<Vector2> BresenhamLine(Vector2 p0, Vector2 p1)
        {
            return BresenhamLine((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y);
        }

        // Returns the list of points from (x0, y0) to (x1, y1)
        private static List<Vector2> BresenhamLine(int x0, int y0, int x1, int y1)
        {
            // Optimization: it would be preferable to calculate in
            // advance the size of "result" and to use a fixed-size array
            // instead of a list.
            List<Vector2> result = new List<Vector2>();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int ystep;
            int y = y0;
            if (y0 < y1) ystep = 1; else ystep = -1;
            for (int x = x0; x <= x1; x++)
            {
                if (steep) result.Add(new Vector2(y, x)); else result.Add(new Vector2(x, y));
                error += deltay;
                if (2 * error >= deltax)
                {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }

        private static void EmptyMap()
        {
            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    _tiles[x, y] = new Tile(x, y);
                    _tiles[x, y].Type = TileType.Walkable;
                }
            }
        }

        public static Tile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }

        public static Clock Clock
        {
            get { return _clock; }
            set { _clock = value; }
        }

        public static List<CrewMember> CrewMembers
        {
            get { return _crewMembers; }
            set { _crewMembers = value; }
        }

        public static List<GameObject> Objects
        {
            get { return _objects; }
            set { _objects = value; }
        }

        public static List<GameStructure> Structures
        {
            get { return World._structures; }
            set { World._structures = value; }
        }

        public static Planet Planet
        {
            get { return _planet; }
            set { _planet = value; }
        }
    }
}
