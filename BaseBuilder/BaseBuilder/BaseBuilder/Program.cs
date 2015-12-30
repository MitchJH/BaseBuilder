using System;

namespace BaseBuilder
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            args = new string[5];
            args[0] = "window_mode Borderless";
            args[1] = "x_res 1680";
            args[2] = "y_res 1050";
            args[3] = "window_width 1680";
            args[4] = "window_height 1050";

            using (Engine game = new Engine(args))
            {
                game.Run();
            }
        }
    }
#endif
}

