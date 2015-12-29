using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    /// <summary>
    /// Basic TextBox class supports char sets,supports events : onClick, onMouseDown, onMouseMove, onMouseEnter, onMouseLeave, onChange
    /// </summary>
    public class TextBox : GUIControl
    {

        #region Fields

        public EHandler onChange;

        // Max chars the control can contain
        protected int maxLength = 9;

        protected KeyboardState prevKeyboardState;

        protected string charSet;

        protected bool readOnly = false; // If true the user won't be able to type inside the textbox

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a new TextBox
        /// </summary>
        /// <param name="name">Control name</param>
        /// <param name="text">Text displayed on the control</param>
        /// <param name="maxLength">Max text length the control can hold</param>
        /// <param name="PositionWidthHeight">A rectangle that specifies the position, width , height relative to the control parent</param>
        /// <param name="texture">Texture to be drawn on the control, pass null if not needed</param>
        /// <param name="font">Font used for displaying text, pass null if there's no text</param>
        /// <param name="foreColor">Color of the displayed text</param>
        /// <param name="viewport">Optional : Viewport used to render the gui, if your game contains only one viewport pass null or don't pass anything at all.</param>
        public TextBox(String name, String text, int maxLength, Rectangle PositionWidthHeight, Texture2D texture, SpriteFont font, Color foreColor, Viewport? viewport = null)
            : this(name, text,maxLength,"", PositionWidthHeight, texture, font, foreColor, viewport)
        {
            if (texture!=null) sourceRectangle = new Rectangle(0, texture.Height / 2, texture.Width, texture.Height / 2);
        }


        /// <summary>
        /// Creates a new TextBox
        /// </summary>
        /// <param name="name">Control name</param>
        /// <param name="text">Text displayed on the control</param>
        /// <param name="maxLength">Max text length the control can hold</param>
        /// <param name="charSet">A string containing all allowed chars, other chars will be omitted when typed</param>
        /// <param name="PositionWidthHeight">A rectangle that specifies the position, width , height relative to the control parent</param>
        /// <param name="texture">Texture to be drawn on the control, pass null if not needed</param>
        /// <param name="font">Font used for displaying text, pass null if there's no text</param>
        /// <param name="foreColor">Color of the displayed text</param>
        /// <param name="viewport">Optional : Viewport used to render the gui, if your game contains only one viewport pass null or don't pass anything at all.</param>
        public TextBox(String name, String text, int maxLength, string charSet, Rectangle PositionWidthHeight, Texture2D texture, SpriteFont font, Color foreColor, Viewport? viewport = null)
            : base(name, text, PositionWidthHeight, texture, font, foreColor,viewport)
        {
            this.charSet = charSet;
            this.maxLength = maxLength;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the gui
        /// </summary>
        /// <param name="mouseState">Current mouse state</param>
        /// <param name="keyboardState">Current keyboard state</param>
        public override void Update(MouseState mouseState, KeyboardState keyboardState)
        {
            if (!enabled || !visible) return;

            // if both control and its parent has focus process keyboard commands
            if (!readOnly && Focus && ParentHasFocus)
            {
                // Get all pressed keys
                Keys[] pressed = keyboardState.GetPressedKeys();

                // Is shift key pressed?
                bool shiftKeyPressed = false;

                if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
                    shiftKeyPressed = true;


                foreach (Keys k in pressed)
                {
                    if (prevKeyboardState.IsKeyUp(k))
                    {
                        if (k == Keys.Back && text.Length >= 1)
                        {
                            text = text.Substring(0, text.Length - 1);
                            if (onChange != null) onChange(this);
                        }
                        else if (text.Length < maxLength)
                        {
                            char ch;
                            bool charFound = false;

                            charFound = KeyboardUtils.KeyToString(k, shiftKeyPressed, out ch);

                            if (charSet != null & charSet != "")
                            {
                                if (!charSet.Contains(ch)) charFound = false;
                            }

                            if (charFound)
                            {
                                text = text + ch;
                                if (onChange != null) onChange(this);
                            }
                        }
                    }
                }
            }

            prevKeyboardState = keyboardState;

            // TextBox has two sprites, one when it has focus, the other when it doesn't
            if (texture != null) sourceRectangle = new Rectangle(0, (hasFocus ? texture.Height / 2 : 0), texture.Width, texture.Height / 2);

            base.Update(mouseState, keyboardState);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Sets, gets the ReadOnly state of the text box, if true the user won't be able to type inside the textbox
        /// </summary>
        public bool ReadOnly
        {
            get
            {
                return readOnly;
            }
            set
            {
                readOnly = value;
            }
        }

        /// <summary>
        /// Gets or sets the text displayed on the control
        /// </summary>
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                String prevText = text;

                base.Text = value;

                if (onChange != null && prevText != text) onChange(this);
            }
        }

        #endregion
    }
}
