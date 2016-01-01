using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class CrewMember : Entity
    {
        private string _name;
        private int _age;
        private float _walk_speed;
        private float _run_speed;

        private Country _country;

        private Needs _needs;
        private Stats _stats;
        private List<Trait> _traits;

        private string _sprite;
        private Vector2 _destination;

        private enum State : byte { Idle = 0, Walking = 1, Running = 2, Sleeping = 3, Building = 4};

        

        Point _startTile = Point.Zero;
        Point _endTile = Point.Zero;

        LinkedList<Tile> _path = new LinkedList<Tile>();
        List<Vector2> _path_waypoints = new List<Vector2>();


        byte _current_state;            //The physical state which the crew member is currently in.
        float _current_exertion_rate;   //The current exertion rate the crew member has.
        float _current_damage_rate;     //The current rate at which damage is applied. This is WIP. Needs design check.
        float _current_hunger_rate;     //The current rate at which hunger is applied. This is WIP. Needs design check for what affects hunger.
        float _current_thirst_rate;     //The current rate at which hunger is applied. This is WIP. Needs design check for what affects thirst.
        float _current_stress_rate;     //The current rate at which hunger is applied. This is WIP. Needs design check for what affects stress.
        
        private string _activity;       //A description of what the crew member is doing.

        public CrewMember()
            : base()
        {
            _name = "NO_NAME";
            _age = 0;
            _country = new Country();
            _country.Name = "Earth";
            _country.Demonym = "Human";
            _country.Flag = "EarthFlag";

            _needs.Health = 100;
            _needs.Energy = 100;
            _needs.Hunger = 100;
            _needs.Thirst = 100;
            _needs.Stress = 100;

            _stats.Fitness = 0;
            _stats.Engineering = 0;
            _stats.Agriculture = 0;
            _stats.Medicine = 0;

            _walk_speed = 60;
            _run_speed = 100;

            _traits = new List<Trait>();
            Selected = false;

            _startTile = Point.Zero;
            _endTile = Point.Zero;

            _activity = "Idle";
            this.Destination = this.Position;
        }

        public CrewMember(string name, int age, float posX, float posY, string sprite)
            : base(new Vector2(posX, posY), 64, 64, 64, true, name)
        {
            _name = name;
            _age = age;
            _country = new Country();
            _country.Name = "England";
            _country.Demonym = "English";
            _country.Flag = "England_Flag";

            _needs.Health = 100;
            _needs.Energy = 100;
            _needs.Hunger = 100;
            _needs.Thirst = 100;
            _needs.Stress = 100;

            _stats.Fitness = 0;
            _stats.Engineering = 0;
            _stats.Agriculture = 0;
            _stats.Medicine = 0;

            _walk_speed = 500;
            _run_speed = 1000;

            _traits = new List<Trait>();

            _sprite = sprite;

            Position = new Vector2(posX, posY);

            _startTile = new Point((int)posX, (int)posY);
            _endTile = Point.Zero;
            
            _current_state = 0;
            _current_exertion_rate = GetExertionRate(State.Idle);
            _current_damage_rate = -1;
            _current_hunger_rate = -0.2f;
            _current_thirst_rate = -0.25f;
            _current_stress_rate = -0.001f;

            Width = 64;
            Height = 64;

            _activity = "Idle";


            this.Destination = this.Position;
        }

        public override void Collide(Entity entity)
        {
            base.Collide(entity);
            _activity = "Collision detected.";
        }

        public Vector2 Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }

        public bool Move(GameTime gameTime)
        {
            
            //If there are currently waypoints determined, the crew member will start moving to the next waypoint.
            //The waypoint they move to is always at index 0 in the list.
            if (_path_waypoints.Count > 0)
            {
                Direction = Vector2.Normalize(_path_waypoints[0] - this.Position);
                Vector2 force = new Vector2(0, 0);
                force += Direction * (float)gameTime.ElapsedGameTime.TotalSeconds;

                ApplyForce(force * _walk_speed);
                
                if (_current_state != (byte)State.Walking)
                {
                    StateChange(State.Walking);
                }

                //Once a waypoint has been reached, remove it from the list.
                //The RemoveAt function removes the index specified, and bumps all other idexes up.
                if (Vector2.Distance(this.Position, _path_waypoints[0]) < 1)
                {
                    _path_waypoints[0] = this.Position;
                    _path_waypoints.RemoveAt(0);
                    
                    return true;
                }
            }
            else
            {
                if (_current_state != (byte)State.Idle)
                {
                    StateChange(State.Idle);
                    Console.WriteLine(Name + ": ' I have arrived! My energy is now " + _needs.Energy + "'");
                }
            }

            if (_path.Count > 0)
            {
                if (_waypoint + 1 < _path.Count)
                {
                    _waypoint++;
                    
                    Destination = new Vector2(_path.ElementAt(_waypoint).Position.X * Constants.TILE_SIZE, _path.ElementAt(_waypoint).Position.Y * Constants.TILE_SIZE);

                    _path_waypoints.Add(new Vector2(Destination.X + (Constants.TILE_SIZE / 2), Destination.Y + (Constants.TILE_SIZE / 2)));

                    //Debug
                    //Console.WriteLine("added ("+ Destination.X + "," + Destination.Y + ") to the route.");
                }
            }

            _endTile = new Point((int)Destination.X / Constants.TILE_SIZE, (int)Destination.Y / Constants.TILE_SIZE);

            return false;
        }

        public bool DeterminePath(LinkedList<Tile> path)
        {
            Point start_location = new Point((int)Position.X / Constants.TILE_SIZE, (int)Position.Y / Constants.TILE_SIZE);
            _path = path;

            _path_waypoints.Clear();
            if (_path == null)
            {
                Console.WriteLine("No path found, resetting tiles.");
                Audio.PlaySoundEffect("low_double_beep");

                _path = new LinkedList<Tile>();
                _startTile = Point.Zero;
                _endTile = Point.Zero;
            }
            else
            {
                Console.WriteLine(" 'On my way!' ");
                Audio.PlaySoundEffect("high_double_beep");
               
                _waypoint = 0;
                Destination = new Vector2(path.ElementAt(_waypoint).Position.X * Constants.TILE_SIZE, path.ElementAt(_waypoint).Position.Y * Constants.TILE_SIZE);
                
                _startTile = start_location;

            }

            return true;

        }

        
        public bool UpdateNeeds(GameTime gameTime)
        {
            CalculateHealth(gameTime);

            CalculateEnergy(gameTime);

            CalculateHunger(gameTime);
            
            CalculateThirst(gameTime);
            
            CalculateStress(gameTime);

            return true;
        }

        /*This method handles the different states a crew member can be in. More to be added over time.
         * A seperate method for emotional states and other things will be used. This is their physical state.
         */
        private bool StateChange(State newState)
        {
            
            _current_state = (byte)newState;
            _current_exertion_rate = GetExertionRate(newState);

            if (newState == State.Idle)
            {
                Console.WriteLine(Name + ": ' I am now idle. '");
                _activity = "Idle.";
            }
            else if (newState == State.Walking)
            {
                Console.WriteLine(Name + ": ' I am now walking! '");
                _activity = "Walking.";
                //_activity = "Walking from (" + _startTile.X + "," + _startTile.Y + ") to (" + _endTile.X + "," + _endTile.Y + ")";
            }
            else if (newState == State.Running)
            {
                Console.WriteLine(Name + ": ' I am now running! '");
                _activity = "Running from (" + _startTile.X + "," + _startTile.Y + ") to (" + _endTile.X + "," + _endTile.Y + ")";
            }
            else if (newState == State.Sleeping)
            {
                Console.WriteLine(Name + ": ' Time for bed! Zz. '");
                _activity = "Sleeping.";
            }
            else if (newState == State.Building)
            {
                Console.WriteLine(Name + ": ' Building the structure sir! '");
                _activity = "Constructing...";
            }

            return true;

        }

        private float CalculateHealth(GameTime gameTime)
        {
            float rate_of_change = 0.0f;

            //If hunger or thirst is below 0.
            if(_needs.Hunger <= 0 || _needs.Thirst <= 0)
            {
                rate_of_change = _current_damage_rate * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            _needs.Health += rate_of_change;

            if (_needs.Health <= 0)
            {
                _needs.Health = 0;
            }

            return rate_of_change;
        }

        private void CalculateEnergy(GameTime gameTime)
        {
            float rate_of_change = 0;

            //When hungry, energy is burned faster.
            float hunger_modifier = 1.0f;

            if (_needs.Hunger <= 0)
            {
                hunger_modifier = 1.2f;
            }

            _needs.Energy += rate_of_change = (_current_exertion_rate * hunger_modifier) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //If the need has gone below 0, set to 0.
            if (_needs.Energy < 0)
            {
                _needs.Energy = 0;
            }
        }

        private void CalculateHunger(GameTime gameTime)
        {
            float rate_of_change = _current_hunger_rate * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _needs.Hunger += rate_of_change;

            if (_needs.Hunger < 0)
            {
                _needs.Hunger = 0;
            }

        }

        private void CalculateThirst(GameTime gameTime)
        {
            float rate_of_change = _current_thirst_rate * (float)gameTime.ElapsedGameTime.TotalSeconds;

            rate_of_change += (_current_exertion_rate / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _needs.Thirst += rate_of_change;

            if (_needs.Thirst < 0)
            {
                _needs.Thirst = 0;
            }
        }

        private void CalculateStress(GameTime gameTime)
        {
            float rate_of_change = _current_stress_rate * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _needs.Stress += rate_of_change;

            if (_needs.Stress < 0)
            {
                _needs.Stress = 0;
            }

        }

        public bool Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Move(gameTime);
            
            UpdateNeeds(gameTime);

            return true;
        }

        /*Assigns the exertion rate for each different state. That is the rate at which the crew member get's tired.
         * Perhaps we could read these values in from a database eventually.
         */
        private float GetExertionRate(State newState)
        {
            switch (newState)
            {
                case State.Idle:
                    return -0.1f;
                case State.Walking:
                    return -0.3f;
                case State.Running:
                    return -0.8f;
                case State.Sleeping:
                    return 1.0f;
                case State.Building:
                    return -0.5f;
            }

            Console.WriteLine(Name + ": ' ERROR! I'm not sure what my exertion rate is... Have you added a new state but not set an exertion rate? '");
            
            return 0;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Age
        {
            get { return _age; }
            set { _age = value; }
        }

        public Country Nationality
        {
            get { return _country; }
            set { _country = value; }
        }

        public Needs Needs
        {
            get { return _needs; }
            set { _needs = value; }
        }

        public Stats Stats
        {
            get { return _stats; }
            set { _stats = value; }
        }

        public List<Trait> Traits
        {
            get { return _traits; }
            set { _traits = value; }
        }

        public string Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public Point StartTile
        {
            get { return _startTile;  }
            set { _startTile = value;  }
        }

        public Point EndTile
        {
            get { return _endTile; }
            set { _endTile = value; }
        }

        public LinkedList<Tile> Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public string Activity
        {
            get { return _activity; }
            set { _activity = value; }
        }
    }

    public struct Needs
    {
        public double Health;
        public double Energy;
        public double Hunger;
        public double Thirst;
        public double Stress;
    }

    public struct Stats
    {
        public double Fitness;
        public double Engineering;
        public double Agriculture;
        public double Medicine;
    }

    public class Country
    {
        private string _name;
        private string _demonym;
        private string _flag;

        public Country()
        {
        }
        public Country(string name, string demonym, string flag)
        {
            _name = name;
            _demonym = demonym;
            _flag = flag;
        }

        /// <summary>
        /// The name of the country.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The identifier of the countries residents.
        /// Example: The demonym of France is French
        /// </summary>
        public string Demonym
        {
            get { return _demonym; }
            set { _demonym = value; }
        }

        /// <summary>
        /// The texture ID for the flag of the country.
        /// </summary>
        public string Flag
        {
            get { return _flag; }
            set { _flag = value; }
        }
    }

    public class Trait
    {
        private string _ID;
        private string _name;
        private string _description;
        private Texture2D _icon;

        public Trait()
        {
        }

        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Texture2D Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
    }
}
