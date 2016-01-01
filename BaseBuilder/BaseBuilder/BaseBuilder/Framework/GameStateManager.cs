using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseBuilder
{
    public static class GameStateManager
    {
        private static GameState _state;

        static GameStateManager()
        {
            _state = GameState.MainMenu;
        }

        public static GameState State
        {
            get { return _state; }
            set { _state = value; }
        }
    }

    public enum GameState
    {
        GameWorld,
        MainMenu,
        Exit
    }
}
