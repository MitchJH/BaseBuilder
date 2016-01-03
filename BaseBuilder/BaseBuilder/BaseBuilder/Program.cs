using System;

namespace BaseBuilder
{
#if WINDOWS
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Engine game = new Engine(args))
            {
                game.Run();
            }
        }
    }
#endif
}

