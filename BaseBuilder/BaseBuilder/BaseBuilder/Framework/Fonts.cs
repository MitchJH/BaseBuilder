using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BaseBuilder
{
    public static class Fonts
    {
        private static Dictionary<string, SpriteFont> _fonts;

        static Fonts()
        {
            _fonts = new Dictionary<string, SpriteFont>();
        }

        public static void LoadFontBank(string file, ContentManager content)
        {
            using (var stream = TitleContainer.OpenStream(file))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#") == false)
                        {
                            string[] split = line.Split(',');
                            string id = split[0];
                            string filepath = split[1];

                            SpriteFont newFont = content.Load<SpriteFont>(filepath);
                            _fonts.Add(id, newFont);
                        }
                    }
                }
            }
        }

        public static SpriteFont Get(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_fonts.ContainsKey(key))
                {
                    return _fonts[key];
                }
            }
            return null;
        }
    }
}
