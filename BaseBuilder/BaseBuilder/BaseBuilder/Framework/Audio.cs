using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace BaseBuilder
{
    public static class Audio
    {
        private static SoundEffect _MISSING_AUDIO;

        private static Dictionary<string, SoundEffect> _sounds;
        private static Dictionary<string, Song> _music;

        static Audio()
        {
            _sounds = new Dictionary<string, SoundEffect>();
            _music = new Dictionary<string, Song>();
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
                        if (line.StartsWith("#") == false && string.IsNullOrEmpty(line) == false)
                        {
                            string[] split = line.Split(',');
                            string id = split[0];
                            string filepath = split[1];
                            string audioType = split[2];

                            if (audioType == "effect")
                            {
                                SoundEffect newSound = content.Load<SoundEffect>(filepath);
                                _sounds.Add(id, newSound);
                            }
                            else if (audioType == "music")
                            {
                                Song newSong = content.Load<Song>(filepath);
                                _music.Add(id, newSong);
                            }
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
        public static bool PlaySoundEffect(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_sounds.ContainsKey(key))
                {
                    SoundEffectInstance sei = _sounds[key].CreateInstance();
                    sei.Volume = ApplyMasterVolume(Settings.EffectVolume);
                    sei.Play();
                    return true;
                }
            }
            _MISSING_AUDIO.Play();
            return false;
        }

        public static bool PlayMusicTrack(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_music.ContainsKey(key))
                {
                    MediaPlayer.Volume = ApplyMasterVolume(Settings.MusicVolume);
                    MediaPlayer.Play(_music[key]);
                    return true;
                }
            }
            _MISSING_AUDIO.Play();
            return false;
        }

        public static SoundEffect GetSoundEffect(string key)
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

        private static float ApplyMasterVolume(float otherVolume)
        {
            return MathHelper.Clamp(otherVolume * Settings.MasterVolume, 0.0f, 1.0f);
        }

        public static SoundEffect MISSING_AUDIO
        {
            get { return _MISSING_AUDIO; }
            set { _MISSING_AUDIO = value; }
        }
    }
}
