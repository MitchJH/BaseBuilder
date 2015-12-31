using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class Entity : Node
    {
        public int _waypoint;
        private bool _selected;

        private float _width;
        private float _height;
        private float _radius;

        public Entity()
        {
            _width = 0;
            _height = 0;
            _radius = 0;
        }

        public virtual void CollideFrom(PhysicsEntity entity)
        {
        }

        public virtual void CollideTo(PhysicsEntity entity)
        {
        }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /*Get the radius of the entity. If the height and width are different the radius isn't determined and just the (width / 2) is returned.
         */
        public float GetRadius()
        {
            float w = _width / 2;
            float h = _height / 2;

            if (w == h)
            {
                return w;
            }
            else
            {
                return w;
                Console.WriteLine("The width and height of this Entity are not the same, so it doesn't have a set radius. Value set to (width/2)");
            }
        }

        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public float Height
        {
            get { return _height; }
            set { _height = value; }
        } 

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }



        /*
         * public Facing Facing
       
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
        }*/
    }
}
