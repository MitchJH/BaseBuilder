using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseBuilder
{
    public static class Version
    {
        private static bool _enabled;
        private static string _version;
        private static Vector2 _position;
        private static SpriteFont _font;
        
        static Version()
        {
            _enabled = false;
            _version = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            _font = Fonts.Get("Tiny");
            _position = Vector2.Zero;
        }

        public static void Enable()
        {
            _enabled = true;
        }

        public static void Disable()
        {
            _enabled = false;
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            if (_enabled)
            {
                if (_position == Vector2.Zero)
                {
                    int vpw = spriteBatch.GraphicsDevice.Viewport.Bounds.Width;
                    int string_width = (int)_font.MeasureString(_version).X + 1;
                    _position = new Vector2(vpw - string_width, 0);
                }

                spriteBatch.Begin();
                spriteBatch.DrawString(_font, _version, _position, Color.White);
                spriteBatch.End();
            }
        }
    }
}
