using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class Entity
    {
        private Vector2 _position;
        private Vector2 _destination;

        public int _waypoint = 0;

        private bool _selected;
        public Entity()
        {
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }
        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }
        public Facing Facing
        {
            // DOWN = 0 / 1
            // UP = 0 / -1
            // RIGHT = 1 / 0
            // LEFT = -1 / 0

            // UP RIGHT = 1 / -1
            // UP LEFT = -1 / -1
            // DOWN RIGHT = 1 / 1
            // DOWN LEFT = -1 / 1

            get
            {
                Vector2 direction = Vector2.Normalize(this.Destination - this.Position);

                if (direction.X == 0 && direction.Y > 0) // DOWN
                {
                    return BaseBuilder.Facing.Front;
                }
                else if (direction.X == 0 && direction.Y < 0) // UP
                {
                    return BaseBuilder.Facing.Back;
                }
                else if (direction.X > 0 && direction.Y == 0) // RIGHT
                {
                    return BaseBuilder.Facing.Right;
                }
                else if (direction.X < 0 && direction.Y == 0) // LEFT
                {
                    return BaseBuilder.Facing.Left;
                }
                else if (direction.X > 0 && direction.Y < 0) // UP RIGHT
                {
                    return BaseBuilder.Facing.Back;
                }
                else if (direction.X < 0 && direction.Y < 0) // UP LEFT
                {
                    return BaseBuilder.Facing.Back;
                }
                else if (direction.X > 0 && direction.Y > 0) // DOWN RIGHT
                {
                    return BaseBuilder.Facing.Right;
                }
                else if (direction.X < 0 && direction.Y > 0) // DOWN LEFT
                {
                    return BaseBuilder.Facing.Left;
                }
                else
                {
                    return BaseBuilder.Facing.Front;
                }
            }
        }
    }

    public enum Facing
    {
        Front,
        Back,
        Left,
        Right
    }
}
