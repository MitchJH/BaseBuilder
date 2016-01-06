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
        // General Settings
        private static WindowMode _windowMode;
        private static GameState _startingGameState;
        private static int _X_resolution;
        private static int _Y_resolution;
        private static int _window_width;
        private static int _window_height;
        private static int _X_windowPos;
        private static int _Y_windowPos;
        private static bool _mouseVisible;
        private static bool _fixedTimestep;
        private static bool _mouseScrolling;
        // Audio Settings
        private static float _masterVolume;
        private static float _effectVolume;
        private static float _musicVolume;

        static Settings()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APP_DATA_GAME_NAME);

            // General
            _windowMode = WindowMode.Fullscreen;
            _startingGameState = GameState.MainMenu;
            _X_resolution = screenWidth;
            _Y_resolution = screenHeight;
            _X_windowPos = 0;
            _Y_windowPos = 0;
            _mouseVisible = true;
            _fixedTimestep = false;
            _mouseScrolling = true;
            // Audio
            _masterVolume = 1.0f;
            _effectVolume = 1.0f;
            _musicVolume = 0.5f;
        }

        private static void ParseSettings(List<string> settings)
        {
            foreach (string setting in settings)
            {
                if (setting.StartsWith("#") == false && string.IsNullOrEmpty(setting) == false)
                {
                    string[] split = setting.Split(' ');
                    string value = split[1];

                    if (setting.StartsWith("window_mode"))
                    {
                        _windowMode = (WindowMode)Enum.Parse(typeof(WindowMode), value);
                    }
                    else if (setting.StartsWith("starting_game_state"))
                    {
                        _startingGameState = (GameState)Enum.Parse(typeof(GameState), value);
                    }
                    else if (setting.StartsWith("x_res"))
                    {
                        _X_resolution = int.Parse(value);
                    }
                    else if (setting.StartsWith("y_res"))
                    {
                        _Y_resolution = int.Parse(value);
                    }
                    else if (setting.StartsWith("window_width"))
                    {
                        _window_width = int.Parse(value);
                    }
                    else if (setting.StartsWith("window_height"))
                    {
                        _window_height = int.Parse(value);
                    }
                    else if (setting.StartsWith("x_pos"))
                    {
                        _X_windowPos = int.Parse(value);
                    }
                    else if (setting.StartsWith("y_pos"))
                    {
                        _Y_windowPos = int.Parse(value);
                    }
                    else if (setting.StartsWith("mouse_visible"))
                    {
                        _mouseVisible = bool.Parse(value);
                    }
                    else if (setting.StartsWith("fixed_timestep"))
                    {
                        _fixedTimestep = bool.Parse(value);
                    }
                    else if (setting.StartsWith("mouse_scrolling"))
                    {
                        _mouseScrolling = bool.Parse(value);
                    }
                    else if (setting.StartsWith("master_volume"))
                    {
                        _masterVolume = MathHelper.Clamp(float.Parse(value), 0.0f, 1.0f);
                    }
                    else if (setting.StartsWith("effect_volume"))
                    {
                        _effectVolume = MathHelper.Clamp(float.Parse(value), 0.0f, 1.0f);
                    }
                    else if (setting.StartsWith("music_volume"))
                    {
                        _musicVolume = MathHelper.Clamp(float.Parse(value), 0.0f, 1.0f);
                    }
                }
            }
        }

        public static void LoadFromFile()
        {
            if (Directory.Exists(appDataPath))
            {
                string file = Path.Combine(appDataPath, @"Settings.txt");

                if (File.Exists(file))
                {
                    string[] lines = File.ReadAllLines(file);
                    ParseSettings(lines.ToList());
                }
            }
            else
            {
                Directory.CreateDirectory(appDataPath);
            }
        }

        public static void WriteToFile()
        {
            // Collect settings text
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("# GENERAL SETTINGS");
            sb.AppendLine("window_mode " + _windowMode.ToString() + " // Windowed, Borderless, Fullscreen");
            sb.AppendLine("starting_game_state " + _startingGameState.ToString() + " // MainMenu, GameWorld, Exit");
            sb.AppendLine("x_res " + _X_resolution.ToString());
            sb.AppendLine("y_res " + _Y_resolution.ToString());
            sb.AppendLine("window_width " + _window_width.ToString());
            sb.AppendLine("window_height " + _window_height.ToString());
            sb.AppendLine("x_pos " + _X_windowPos.ToString());
            sb.AppendLine("y_pos " + _Y_windowPos.ToString());
            sb.AppendLine("mouse_visible " + _mouseVisible.ToString());
            sb.AppendLine("fixed_timestep " + _fixedTimestep.ToString());
            sb.AppendLine("mouse_scrolling " + _mouseScrolling.ToString());
            sb.AppendLine("");
            sb.AppendLine("# AUDIO SETTINGS");
            sb.AppendLine("master_volume " + _masterVolume.ToString("N2"));
            sb.AppendLine("effect_volume " + _effectVolume.ToString("N2"));
            sb.AppendLine("music_volume " + _musicVolume.ToString("N2"));            

            if (Directory.Exists(appDataPath) == false)
            {
                Directory.CreateDirectory(appDataPath);
            }
            
            string file = Path.Combine(appDataPath, @"Settings.txt");

            if (File.Exists(file))
            {
                File.Delete(file);
            }

            File.WriteAllText(file, sb.ToString());
        }

        public static void ParseCommandLineArguments(string[] args)
        {
            List<string> settings = new List<string>();

            foreach (string arg in args)
            {
                string fixed_arg = arg.Replace("=", " ");
                settings.Add(fixed_arg);
            }

            ParseSettings(settings);
        }

        public static WindowMode WindowMode
        {
            get { return Settings._windowMode; }
            set { Settings._windowMode = value; }
        }

        public static GameState StartingGameState
        {
            get { return Settings._startingGameState; }
            set { Settings._startingGameState = value; }
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

        public static bool MouseScrolling
        {
            get { return Settings._mouseScrolling; }
            set { Settings._mouseScrolling = value; }
        }

        public static float MasterVolume
        {
            get { return Settings._masterVolume; }
            set { Settings._masterVolume = value; }
        }

        public static float EffectVolume
        {
            get { return Settings._effectVolume; }
            set { Settings._effectVolume = value; }
        }

        public static float MusicVolume
        {
            get { return Settings._musicVolume; }
            set { Settings._musicVolume = value; }
        }
    }

    public enum WindowMode
    {
        Windowed,
        Borderless,
        Fullscreen
    }
}
