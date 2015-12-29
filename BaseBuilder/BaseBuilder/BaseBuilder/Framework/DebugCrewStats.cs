using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BaseBuilder
{
    public static class DebugCrewStats
    {
        private static bool _enabled;
        private static SpriteFont _font;

        static DebugCrewStats()
        {
            _enabled = false;
            _font = Fonts.Get("Tiny");
        }

        public static void Enable()
        {
            _enabled = true;
        }

        public static void Disable()
        {
            _enabled = false;
        }

        public static void Update(GameTime gameTime)
        {
            
        }

        public static void Draw(SpriteBatch spriteBatch, CrewMember crew_member)
        {
            if (_enabled)
            {

                string crew_name = crew_member.Name + ", " + crew_member.Activity;
                string crew_health = "Health: " + crew_member.Needs.Health.ToString("0.00");
                string crew_energy = "Energy: " + crew_member.Needs.Energy.ToString("0.00");
                string crew_thirst = "Thirst: " + crew_member.Needs.Thirst.ToString("0.00");
                string crew_stress = "Stress: " + crew_member.Needs.Stress.ToString("0.00");
                string crew_hunger = "Hunger: " + crew_member.Needs.Hunger.ToString("0.00");


                spriteBatch.Begin();


                spriteBatch.DrawString(_font, crew_name, new Vector2(2, 30), Color.Black);
                spriteBatch.DrawString(_font, crew_name, new Vector2(1, 30), Color.Yellow);

                spriteBatch.DrawString(_font, crew_health, new Vector2(2, 50), Color.Black);
                spriteBatch.DrawString(_font, crew_health, new Vector2(1, 50), Color.Yellow);

                spriteBatch.DrawString(_font, crew_energy, new Vector2(2, 70), Color.Black);
                spriteBatch.DrawString(_font, crew_energy, new Vector2(1, 70), Color.Yellow);

                spriteBatch.DrawString(_font, crew_thirst, new Vector2(2, 90), Color.Black);
                spriteBatch.DrawString(_font, crew_thirst, new Vector2(1, 90), Color.Yellow);

                spriteBatch.DrawString(_font, crew_stress, new Vector2(2,110), Color.Black);
                spriteBatch.DrawString(_font, crew_stress, new Vector2(1, 110), Color.Yellow);

                spriteBatch.DrawString(_font, crew_hunger, new Vector2(2, 130), Color.Black);
                spriteBatch.DrawString(_font, crew_hunger, new Vector2(1, 130), Color.Yellow);

                spriteBatch.End();
            }
        }

        public static bool Enabled
        {
            get { return _enabled; }
        }
    }
}
