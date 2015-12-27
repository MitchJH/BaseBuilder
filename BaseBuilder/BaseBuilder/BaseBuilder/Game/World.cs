using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    /// <summary>
    /// Holds the current gamestate, used for savegame serialization.
    /// </summary>
    public class World
    {
        private Tile[,] _tiles;
        private Clock _clock;
        private List<CrewMember> _colonists;
        private Planet _planet;

        public World()
        {
            _tiles = new Tile[Constants.MAP_WIDTH, Constants.MAP_HEIGHT];
            _clock = new Clock();
            _colonists = new List<CrewMember>();

            EmptyMap();
        }

        /// <summary>
        /// Find a path through the world
        /// </summary>
        /// <param name="start">The tile to start the path from</param>
        /// <param name="end">The tile the path should end at</param>
        /// <param name="extraContext">Optional: Allows the passing of extra information to the IsWalkable function in the Tile object
        /// thus allowing a tile to be walkable for different entities by only changing input parameters rather than gamestate</param>
        /// <returns>A linked list of Tile objects that is the best path between the start and end tiles</returns>
        public LinkedList<Tile> FindPath(Point start, Point end, Object extraContext = null)
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

            return truePath;
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

        public RayCastingResult RayCast(Vector2 position, Vector2 direction, float rayLength)
        {
            RayCastingResult result = new RayCastingResult();
            result.DoCollide = false;

            // Exit the function now if the ray length is 0
            if (rayLength == 0)
            {
                result.DoCollide = _tiles[(int)position.X, (int)position.Y].Type == TileType.Empty;
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
                    if (_tiles[(int)rayPoint.X, (int)rayPoint.Y].Type != TileType.Empty)
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
        private void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        // Returns the list of points from p0 to p1 
        private List<Vector2> BresenhamLine(Vector2 p0, Vector2 p1)
        {
            return BresenhamLine((int)p0.X, (int)p0.Y, (int)p1.X, (int)p1.Y);
        }

        // Returns the list of points from (x0, y0) to (x1, y1)
        private List<Vector2> BresenhamLine(int x0, int y0, int x1, int y1)
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

        private void EmptyMap()
        {
            for (int x = 0; x < Constants.MAP_WIDTH; x++)
            {
                for (int y = 0; y < Constants.MAP_HEIGHT; y++)
                {
                    _tiles[x, y] = new Tile(x, y);
                    _tiles[x, y].Type = TileType.Empty;
                }
            }
        }

        public Tile[,] Tiles
        {
            get { return _tiles; }
            set { _tiles = value; }
        }

        public Clock Clock
        {
            get { return _clock; }
            set { _clock = value; }
        }

        public List<CrewMember> Colonists
        {
            get { return _colonists; }
            set { _colonists = value; }
        }

        public Planet Planet
        {
            get { return _planet; }
            set { _planet = value; }
        }
    }
}
