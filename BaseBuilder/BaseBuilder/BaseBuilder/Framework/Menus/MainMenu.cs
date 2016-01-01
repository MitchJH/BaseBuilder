using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace BaseBuilder
{
    public class MainMenu
    {
        private Texture2D _background;
        private Texture2D _background_f;
        private Rectangle _backgroundSize;
        private Random rand = new Random();
        private bool showMenu = false;

        private Form _menuForm;
        private Rectangle menuRectangle;
        private Label _quote;

        public MainMenu(Rectangle screen, ContentManager content)
        {
            _background = content.Load<Texture2D>("Textures/UI/mars_bg");
            _background_f = content.Load<Texture2D>("Textures/UI/mars_bg_f");
            _backgroundSize = screen;

            Texture2D menuTexture = content.Load<Texture2D>("Textures/UI/menu");
            Point menuPos = new Point((screen.Width / 2) - (menuTexture.Width / 2), (screen.Height / 2) - (menuTexture.Height / 2));
            menuRectangle = new Rectangle(menuPos.X, menuPos.Y - 30, menuTexture.Width, menuTexture.Height);
            _menuForm = new Form("menuForm", "", menuRectangle, menuTexture, Fonts.Standard, Color.White);

            Texture2D buttonTexture = content.Load<Texture2D>("Textures/UI/button");
            Point buttonSize = new Point(240, 40);
            Point buttonPos = new Point(40, 40);
            SpriteFont buttonFont = Fonts.Get("Menu");
            
            // NEW GAME BUTTON
            Button _newGame = new Button("new_game", "New Game",
                new Rectangle(40, 90, buttonSize.X, buttonSize.Y),
                buttonTexture, buttonFont, Color.Black);

            _newGame.onClick += new EHandler(NewGame_Click);
            _newGame.onMouseEnter += new EHandler(Beep);
            _menuForm.AddControl(_newGame);

            // LOAD GAME BUTTON
            Button _loadGame = new Button("load_game", "Load Game",
                new Rectangle(40, 150, buttonSize.X, buttonSize.Y),
                buttonTexture, buttonFont, Color.Black);

            _loadGame.onClick += new EHandler(LoadGame_Click);
            _loadGame.onMouseEnter += new EHandler(Beep);
            _menuForm.AddControl(_loadGame);

            // SETTINGS BUTTON
            Button _settings = new Button("settings", "Settings",
                new Rectangle(40, 210, buttonSize.X, buttonSize.Y),
                buttonTexture, buttonFont, Color.Black);

            _settings.onClick += new EHandler(Settings_Click);
            _settings.onMouseEnter += new EHandler(Beep);
            _menuForm.AddControl(_settings);

            // EXIT BUTTON
            Button _exit = new Button("exit", "Exit",
                new Rectangle(40, 270, buttonSize.X, buttonSize.Y),
                buttonTexture, buttonFont, Color.Black);

            _exit.onClick += new EHandler(Exit_Click);
            _exit.onMouseEnter += new EHandler(Beep);
            _menuForm.AddControl(_exit);

            // VERSION LABEL
            Vector2 stringSizeVersion = Fonts.Get("Tiny").MeasureString(Version.GetVersion());
            Vector2 labelPosVersion = new Vector2(160 - (stringSizeVersion.X / 2), menuTexture.Height - 20);
            Label _labelVersion = new Label("version", Version.GetVersion(), labelPosVersion, Fonts.Get("Tiny"), Color.White, Version.GetVersion().Length, 0);
            _menuForm.AddControl(_labelVersion);

            // Quote
            string quote = "Copyright © DAM Games " + DateTime.Now.Year;
            string font = "Quote";
            float stringSize = Fonts.Get(font).MeasureString(quote).X;
            _quote = new Label("quote", quote, new Vector2((screen.Width / 2) - (stringSize / 2), screen.Height - 20), Fonts.Get(font), Color.White * 0.5f, quote.Length, 0);
            
            // Test music
            Audio.Repeat = true;
            Audio.PlayMusicTrack("main_menu");
        }

        public void Update()
        {
            if (showMenu)
            {
                _menuForm.Update(Controls.Mouse, Controls.Keyboard);
            }

            if (fadeIn < 1.0f)
            {
                fadeIn = MathHelper.Lerp(fadeIn, 1.0f, 0.005f);

                if (fadeIn > 0.95f)
                {
                    fadeIn = 1.0f;
                }
                else if (fadeIn > 0.4f)
                {
                    showMenu = true;
                }
            }
            else
            {
                if (point == "HIGH")
                {
                    light = MathHelper.Clamp((float)rand.NextDouble(), 0.2f, 0.3f);
                    point = "MID_FROM_HIGH";
                }
                else if (point == "MID_FROM_HIGH")
                {
                    light = MathHelper.Clamp((float)rand.NextDouble(), 0.1f, 0.2f);
                    point = "LOW";
                }
                else if (point == "MID_FROM_LOW")
                {
                    light = MathHelper.Clamp((float)rand.NextDouble(), 0.1f, 0.2f);
                    point = "HIGH";
                }
                else if (point == "LOW")
                {
                    light = MathHelper.Clamp((float)rand.NextDouble(), 0.0f, 0.1f);
                    point = "MID_FROM_LOW";
                }
            }
        }

        float fadeIn = -0.01f;
        float light = 0.0f;
        string point = "HIGH";

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            if (fadeIn < 1.0f)
            {
                spriteBatch.Draw(_background_f, _backgroundSize, Color.White * fadeIn);
            }
            else
            {
                spriteBatch.Draw(_background, _backgroundSize, Color.White);
                if (rand.Next(1, 101) > (DateTime.Now.Millisecond / 10))
                {
                    spriteBatch.Draw(_background_f, _backgroundSize, Color.White * light);
                }
            }

            if (showMenu)
            {
                _menuForm.Draw(spriteBatch);
                _quote.Draw(spriteBatch);
            }
            spriteBatch.End();
        }

        private void NewGame_Click(GUIControl sender)
        {
            Audio.PlaySoundEffect("high_double_beep");
            GameStateManager.State = GameState.GameWorld;
        }

        private void LoadGame_Click(GUIControl sender)
        {
            Audio.PlaySoundEffect("low_double_beep");
        }

        private void Settings_Click(GUIControl sender)
        {
            Audio.PlaySoundEffect("low_double_beep");
        }

        private void Exit_Click(GUIControl sender)
        {
            GameStateManager.State = GameState.Exit;
        }

        private void Beep(GUIControl sender)
        {
            Audio.PlaySoundEffect("high_beep");
        }
    }
}
