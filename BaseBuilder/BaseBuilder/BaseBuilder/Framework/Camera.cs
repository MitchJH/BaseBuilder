using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public static class Camera
    {
        private static float _zoom;
        private static float _zoomTarget;
        private static Matrix _transform;
        private static Matrix _inverseTransform;
        private static Vector2 _position;
        private static Vector2 _target;
        private static Viewport _viewport;
        private static Int32 _scroll;
        private static Vector2 _origin;

        static Camera()
        {
            _zoom = 1.0f;
            _zoomTarget = _zoom;
            _scroll = 1;
            _position = Vector2.Zero;
            _target = _position;
        }

        public static void Create(Viewport viewport)
        {
            _viewport = viewport;
            _origin = new Vector2(_viewport.Width / 2.0f, _viewport.Height / 2.0f);
        }

        /// <summary>
        /// Update the camera view
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Call Camera Input
            Input(elapsedTime);
            //Create view matrix
            _transform = Matrix.CreateTranslation(new Vector3(-_position, 0f)) *
                   Matrix.CreateTranslation(new Vector3(-_origin, 0f)) *
                   Matrix.CreateScale(_zoom, _zoom, 1f) *
                   Matrix.CreateTranslation(new Vector3(_origin, 0f));
            //Update inverse matrix
            _inverseTransform = Matrix.Invert(_transform);
        }

        private static void Input(float elapsedTime)
        {
            // Shift key doubles scroll speed and zoom speed
            float zoomSpeed = Constants.CAMERA_ZOOM_SPEED * elapsedTime;
            float scrollSpeed = Constants.CAMERA_SCROLL_SPEED * elapsedTime;
            if (Controls.Keyboard.IsKeyDown(Keys.LeftShift))
            {
                zoomSpeed = (Constants.CAMERA_ZOOM_SPEED * 2) * elapsedTime;
                scrollSpeed = (Constants.CAMERA_SCROLL_SPEED * 2) * elapsedTime;
            }

            //Check zoom
            if (Controls.Mouse.ScrollWheelValue != 0)
            {
                if (Controls.Mouse.ScrollWheelValue > _scroll)
                {
                    _zoomTarget += zoomSpeed;
                    _scroll = Controls.Mouse.ScrollWheelValue;
                }
                else if (Controls.Mouse.ScrollWheelValue < _scroll)
                {
                    _zoomTarget -= zoomSpeed;
                    _scroll = Controls.Mouse.ScrollWheelValue;
                }

                //Clamp zoom value
                _zoomTarget = MathHelper.Clamp(_zoomTarget, Constants.CAMERA_MAX_ZOOM_OUT, Constants.CAMERA_MAX_ZOOM_IN);
            }

            _zoom = MathHelper.Lerp(_zoom, _zoomTarget, Constants.CAMERA_ZOOM_INERTIA);

            // Check for mouse dragging scrolling
            if (Controls.Mouse.MiddleButton == ButtonState.Pressed && Controls.MouseOld.MiddleButton == ButtonState.Pressed)
            {
                // TODO: Add intertia to mouse dragging camera movement
                float x_dif = Controls.Mouse.X - Controls.MouseOld.X;
                float y_dif = Controls.Mouse.Y - Controls.MouseOld.Y;
                _position += new Vector2(-(x_dif), -(y_dif));
                _target = _position;
                // We return here as we don't want other scrolling getting in the way
                return;
            }

            // Check the mouse is against the edge of the screen
            if (Settings.MouseScrolling)
            {
                if (Controls.Mouse.X <= Constants.CAMERA_EDGE_SCROLL_SIZE)
                {
                    _target.X -= scrollSpeed;
                }
                if (Controls.Mouse.X >= (_viewport.Width - Constants.CAMERA_EDGE_SCROLL_SIZE))
                {
                    _target.X += scrollSpeed;
                }
                if (Controls.Mouse.Y <= Constants.CAMERA_EDGE_SCROLL_SIZE)
                {
                    _target.Y -= scrollSpeed;
                }
                if (Controls.Mouse.Y >= (_viewport.Height - Constants.CAMERA_EDGE_SCROLL_SIZE))
                {
                    _target.Y += scrollSpeed;
                }
            }
                        
            // Check for keyboard scrolling
            if (Controls.Keyboard.IsKeyDown(Keys.A) || Controls.Keyboard.IsKeyDown(Keys.Left) || Controls.Keyboard.IsKeyDown(Keys.NumPad4))
            {
                _target.X -= scrollSpeed;
            }
            if (Controls.Keyboard.IsKeyDown(Keys.D) || Controls.Keyboard.IsKeyDown(Keys.Right) || Controls.Keyboard.IsKeyDown(Keys.NumPad6))
            {
                _target.X += scrollSpeed;
            }
            if (Controls.Keyboard.IsKeyDown(Keys.W) || Controls.Keyboard.IsKeyDown(Keys.Up) || Controls.Keyboard.IsKeyDown(Keys.NumPad8))
            {
                _target.Y -= scrollSpeed;
            }
            if (Controls.Keyboard.IsKeyDown(Keys.S) || Controls.Keyboard.IsKeyDown(Keys.Down) || Controls.Keyboard.IsKeyDown(Keys.NumPad2))
            {
                _target.Y += scrollSpeed;
            }

            _target.X = MathHelper.Clamp(_target.X, Constants.CAMERA_BOUNDS_MIN_X, Constants.CAMERA_BOUNDS_MAX_X);
            _target.Y = MathHelper.Clamp(_target.Y, Constants.CAMERA_BOUNDS_MIN_Y, Constants.CAMERA_BOUNDS_MAX_Y);

            _position.X = MathHelper.Lerp(_position.X, _target.X, Constants.CAMERA_SCROLL_INERTIA);
            _position.Y = MathHelper.Lerp(_position.Y, _target.Y, Constants.CAMERA_SCROLL_INERTIA);
        }

        public static float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; }
        }

        /// <summary>
        /// Camera View Matrix Property
        /// </summary>
        public static Matrix Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        /// <summary>
        /// Inverse of the view matrix, can be used to get objects screen coordinates
        /// from its object coordinates
        /// </summary>
        public static Matrix InverseTransform
        {
            get { return _inverseTransform; }
        }

        public static Vector2 Position
        {
            get { return _target; }
            set { _target = value; }
        }


        // EXTENSION METHODS
        public static bool IsInCameraView(this MouseState state)
        {
            Point pos = new Point(state.X, state.Y);
            return _viewport.Bounds.Contains(pos);
        }
    }
}