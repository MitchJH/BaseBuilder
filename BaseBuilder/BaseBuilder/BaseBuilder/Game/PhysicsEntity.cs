using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class PhysicsEntity : Node
    {
        public int _waypoint;
        private bool _selected;

        private float _width;
        private float _height;
        private float _radius;
        private float _force;

        private string _type;

        Vector2 _velocity;
        Vector2 _acceleration;
        Vector2 _velocityi;
        public PhysicsEntity(Vector2 position)
        {
            _width = 0;
            _height = 0;
            _radius = 0;
            _type = "No type defined. Make sure to define the type when creating the Entity.";

            _velocity = new Vector2(0, 0);
            _velocityi = new Vector2(0, 0);
            _acceleration = new Vector2(0f, -9.81f);
            Position = position;
            _force = 1;

        }

        public virtual void CollideFrom(Entity entity)
        {
        }

        public virtual void CollideTo(Entity entity)
        {
        }

        public void Update(GameTime gameTime)
        {
            _velocity += _acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            Position += _velocity;

            if (_velocity.X != 0)
            {
                if (_velocity.X > 0)
                {
                    _acceleration.X = _force * -1;
                    if (_velocity.X < 0.03f)
                    {
                        _velocity = new Vector2(0.0f, _velocity.Y);
                        _acceleration.X = 0;
                    }
                }
                else
                {
                    _acceleration.X = _force;
                    if (_velocity.X > 0.03f)
                    {
                        _velocity = new Vector2(0.0f, _velocity.Y);
                        _acceleration.X = 0;
                    }
                }
            }
            if (_velocity.Y != 0)
            {

                if (_velocity.Y > 0)
                {
                    _acceleration.Y = _force * -1;
                    if (_velocity.Y < 0.03f)
                    {
                        _velocity = new Vector2(_velocity.X, 0.0f);
                        _acceleration.Y = 0;
                    }
                }
                else
                {
                    _acceleration.Y = _force;
                    if (_velocity.Y > 0.03f)
                    {
                        _velocity = new Vector2(_velocity.X, 0.0f);
                        _acceleration.Y = 0;
                    }
                }
            }

            
            

            base.Update();


        }

        public void ApplyForce(Vector2 force)
        {
            _acceleration = force;
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
                Console.WriteLine("The width and height of this PhysicsEntity (" + _type + ") are not the same, so it doesn't have a set radius. Value set to (width/2)");
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
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }   

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
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
