using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public static class Controls
    {
        private static MouseState _currentMouseState;
        private static KeyboardState _currentKeyboardState;

        private static MouseState _oldMouseState;
        private static KeyboardState _oldKeyboardState;

        static Controls()
        {
        }

        public static void Update()
        {
            _oldMouseState = _currentMouseState;
            _oldKeyboardState = _currentKeyboardState;

            _currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            _currentKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public static MouseState Mouse
        {
            get
            {
                return _currentMouseState;
            }
        }

        public static KeyboardState Keyboard
        {
            get
            {
                return _currentKeyboardState;
            }
        }

        public static MouseState MouseOld
        {
            get
            {
                return _oldMouseState;
            }
        }

        public static KeyboardState KeyboardOld
        {
            get
            {
                return _oldKeyboardState;
            }
        }
    }
}
