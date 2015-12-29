using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public static class Constants
    {
        // GAME
        public const string APP_DATA_GAME_NAME = "Mars Game";

        // TILE MAP
        public const int TILE_SIZE = 32;
        public const int MAP_WIDTH = 60;
        public const int MAP_HEIGHT = 34;

        // CAMERA
        // Zoom
        public const float CAMERA_ZOOM_SPEED = 5.0f;
        public const float CAMERA_ZOOM_INERTIA = 0.1f;
        public const float CAMERA_MAX_ZOOM_IN = 1.5f;
        public const float CAMERA_MAX_ZOOM_OUT = 0.2f;
        // Scroll
        public const float CAMERA_SCROLL_SPEED = 400.0f;
        public const float CAMERA_SCROLL_INERTIA = 0.1f;
        public const int CAMERA_EDGE_SCROLL_SIZE = 2;
        public const int CAMERA_BOUNDS_MIN_X = -(TILE_SIZE * 10);
        public const int CAMERA_BOUNDS_MIN_Y = -(TILE_SIZE * 10);
        public const int CAMERA_BOUNDS_MAX_X = (TILE_SIZE * MAP_WIDTH) - (TILE_SIZE * 10);
        public const int CAMERA_BOUNDS_MAX_Y = (TILE_SIZE * MAP_HEIGHT) - (TILE_SIZE * 10);

        static Constants()
        {
        }
    }
}
