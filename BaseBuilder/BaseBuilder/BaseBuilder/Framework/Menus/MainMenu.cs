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
        private Rectangle _backgroundSize;

        private Button _newGame;
        private Button _settings;
        private Button _exit;

        public MainMenu(Rectangle screen, ContentManager content)
        {
            _background = content.Load<Texture2D>("Textures/background");
            _backgroundSize = screen;

            Texture2D buttonTexture = content.Load<Texture2D>("Textures/button_long");
            Vector2 buttonSize = new Vector2(256, 64);
            Vector2 buttonPos = new Vector2((screen.Width / 2) - (buttonSize.X / 2), (screen.Height / 2) - (buttonSize.Y / 2));

            int shiftUp = 40;

            _newGame = new Button("new_game", "New Game",
                new Rectangle((int)buttonPos.X, ((int)buttonPos.Y - 84) - shiftUp, (int)buttonSize.X, (int)buttonSize.Y),
                buttonTexture, Fonts.Get("ButtonText"), Color.Black);
            _newGame.onClick += new EHandler(NewGame_Click);
            _newGame.onMouseEnter += new EHandler(Beep);

            _settings = new Button("settings", "Settings",
                new Rectangle((int)buttonPos.X, ((int)buttonPos.Y) - shiftUp, (int)buttonSize.X, (int)buttonSize.Y),
                buttonTexture, Fonts.Get("ButtonText"), Color.Black);
            _settings.onClick += new EHandler(Settings_Click);
            _settings.onMouseEnter += new EHandler(Beep);

            _exit = new Button("exit", "Exit",
                new Rectangle((int)buttonPos.X, ((int)buttonPos.Y + 84) - shiftUp, (int)buttonSize.X, (int)buttonSize.Y),
                buttonTexture, Fonts.Get("ButtonText"), Color.Black);
            _exit.onClick += new EHandler(Exit_Click);
            _exit.onMouseEnter += new EHandler(Beep);
        }

        public void Update()
        {
            _newGame.Update(Controls.Mouse, Controls.Keyboard);
            _settings.Update(Controls.Mouse, Controls.Keyboard);
            _exit.Update(Controls.Mouse, Controls.Keyboard);

            // Check for exit.
            if (Controls.Keyboard.IsKeyDown(Keys.Escape))
            {
                GameStateManager.State = GameState.Exit;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(_background, _backgroundSize, Color.White);
            _newGame.Draw(spriteBatch);
            _settings.Draw(spriteBatch);
            _exit.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void NewGame_Click(GUIControl sender)
        {
            Audio.PlaySoundEffect("high_double_beep");
            GameStateManager.State = GameState.Game;
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
