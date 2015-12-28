using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseBuilder
{
    public static class Settings
    {
        private static string appDataPath;
        private static WindowMode _windowMode;
        private static int _X_resolution;
        private static int _Y_resolution;
        private static int _window_width;
        private static int _window_height;
        private static int _X_windowPos;
        private static int _Y_windowPos;
        private static bool _mouseVisible;
        private static bool _fixedTimestep;

        static Settings()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APP_DATA_GAME_NAME);

            _windowMode = WindowMode.Fullscreen;
            _X_resolution = screenWidth;
            _Y_resolution = screenHeight;
            _X_windowPos = 0;
            _Y_windowPos = 0;
            _mouseVisible = true;
            _fixedTimestep = false;
        }

        public static void Load()
        {
            if (Directory.Exists(appDataPath))
            {
                string file = Path.Combine(appDataPath, @"Settings.txt");

                if (File.Exists(file))
                {
                    string[] lines = File.ReadAllLines(file);

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("#") == false)
                        {
                            string[] split = line.Split(' ');
                            string value = split[1];

                            if (line.StartsWith("window_mode"))
                            {
                                _windowMode = (WindowMode)Enum.Parse(typeof(WindowMode), value);
                            }
                            else if (line.StartsWith("x_res"))
                            {
                                _X_resolution = int.Parse(value);
                            }
                            else if (line.StartsWith("y_res"))
                            {
                                _Y_resolution = int.Parse(value);
                            }
                            else if (line.StartsWith("window_width"))
                            {
                                _window_width = int.Parse(value);
                            }
                            else if (line.StartsWith("window_height"))
                            {
                                _window_height = int.Parse(value);
                            }
                            else if (line.StartsWith("x_pos"))
                            {
                                _X_windowPos = int.Parse(value);
                            }
                            else if (line.StartsWith("y_pos"))
                            {
                                _Y_windowPos = int.Parse(value);
                            }
                            else if (line.StartsWith("mouse_visible"))
                            {
                                _mouseVisible = bool.Parse(value);
                            }
                            else if (line.StartsWith("fixed_timestep"))
                            {
                                _fixedTimestep = bool.Parse(value);
                            }
                        }
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(appDataPath);
            }
        }

        public static WindowMode WindowMode
        {
            get { return Settings._windowMode; }
            set { Settings._windowMode = value; }
        }

        public static int X_resolution
        {
            get { return Settings._X_resolution; }
            set { Settings._X_resolution = value; }
        }

        public static int Y_resolution
        {
            get { return Settings._Y_resolution; }
            set { Settings._Y_resolution = value; }
        }

        public static int Window_Width
        {
            get { return Settings._window_width; }
            set { Settings._window_width = value; }
        }

        public static int Window_Height
        {
            get { return Settings._window_height; }
            set { Settings._window_height = value; }
        }

        public static int X_windowPos
        {
            get { return Settings._X_windowPos; }
            set { Settings._X_windowPos = value; }
        }

        public static int Y_windowPos
        {
            get { return Settings._Y_windowPos; }
            set { Settings._Y_windowPos = value; }
        }

        public static bool IsMouseVisible
        {
            get { return Settings._mouseVisible; }
            set { Settings._mouseVisible = value; }
        }

        public static bool IsFixedTimestep
        {
            get { return Settings._fixedTimestep; }
            set { Settings._fixedTimestep = value; }
        }
    }

    public enum WindowMode
    {
        Windowed,
        Borderless,
        Fullscreen
    }
}
