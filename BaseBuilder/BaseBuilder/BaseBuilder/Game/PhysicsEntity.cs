using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class PhysicsEntity : Entity
    {
        private string _type;               //What type of entity is it.

        private Vector2 _velocity;          //The velocity, directly impacts position every frame.
        private Vector2 _acceleration;      //The acceleration. The rate of change applied to velocity every frame. Can be decceleration too.
        private Vector2 _direction;         //The direction vector for the entity.

        private float _resistance;          //The modifier to the force applied in the opposite direction to which the entity is moving.

        private bool _in_motion;            //Whether or not the entity is moving.

        public PhysicsEntity()
        {
            _velocity = new Vector2(0, 0);
            _acceleration = new Vector2(0, 0);
            _direction = new Vector2(0, 0);

            _resistance = 5;

            _in_motion = false;
        }

        public PhysicsEntity(Vector2 position, string type, float width, float height)
        {
            _type = type;

            _velocity = new Vector2(0, 0);
            _acceleration = new Vector2(0, 0);
            _direction = new Vector2(0, 0);

            _resistance = 5;

            _in_motion = false;

            Position = position;
            Width = width;
            Height = height;

        }

        public override void CollideFrom(PhysicsEntity entity)
        {
            /*Vector2 collide = new Vector2(0,0);

            collide = new Vector2((_direction.X * 10), (_direction.Y * 10));

            collide = _acceleration * -1;

            ApplyForce(_acceleration);*/

            _velocity = entity.Velocity;
        }

        public override void CollideTo(PhysicsEntity entity)
        {
            _velocity = entity.Velocity;
        }

        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //If in motion, add acceleration to velocity and update position.
            if (_in_motion)
            {
                _velocity += _acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

                Position += _velocity;

                //If the velocity has not reached 0, deccelerate in the direciton it's moving. Else, stop it.
                if (_velocity != Vector2.Zero)
                {
                    _direction = Vector2.Normalize(_velocity);

                    _acceleration = new Vector2((_direction.X * _resistance), (_direction.Y * _resistance));

                    _acceleration = _acceleration * -1;
                }
                else
                {
                    _in_motion = false;
                }

                //Cleanup the small velocity and decceleration values.
                if (System.Math.Abs(_velocity.X) < 0.1f)
                {
                    _velocity = new Vector2(0.0f, Velocity.Y);
                    _acceleration.X = 0;
                }
                if (System.Math.Abs(_velocity.Y) < 0.1f)
                {
                    _velocity = new Vector2(_velocity.X, 0.0f);
                    _acceleration.Y = 0;
                }
            }
        }

        public void ApplyForce(Vector2 force)
        {
            _acceleration += force;
            _in_motion = true;
        }


        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public Vector2 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public float Resistance
        {
            get { return _resistance; }
            set { _resistance = value; }
        } 
    }
}
