using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BaseBuilder
{
    public static class Audio
    {
        private static SoundEffect _MISSING_AUDIO;

        private static Dictionary<string, SoundEffect> _sounds;

        static Audio()
        {
            _sounds = new Dictionary<string, SoundEffect>();
        }

        public static void LoadSoundBank(string file, ContentManager content)
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

                            SoundEffect newSound = content.Load<SoundEffect>(filepath);
                            _sounds.Add(id, newSound);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="key">The key of the sound effect</param>
        /// <returns>True if sound was found and played</returns>
        public static bool Play(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_sounds.ContainsKey(key))
                {
                    _sounds[key].Play();
                    return true;
                }
            }
            _MISSING_AUDIO.Play();
            return false;
        }

        public static SoundEffect Get(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_sounds.ContainsKey(key))
                {
                    return _sounds[key];
                }
            }
            return _MISSING_AUDIO;
        }

        public static SoundEffect MISSING_AUDIO
        {
            get { return _MISSING_AUDIO; }
            set { _MISSING_AUDIO = value; }
        }
    }
}
