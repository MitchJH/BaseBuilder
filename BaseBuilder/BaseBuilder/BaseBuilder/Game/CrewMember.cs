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

        private Texture2D _sprite;


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

            
            this.Destination = this.Position;
        }

        public CrewMember(string name, int age, float posX, float posY)
            : base()
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


            _traits = new List<Trait>();

            Position = new Vector2(posX, posY);
            this.Destination = this.Position;
        }

        public bool Move(GameTime gameTime)
        {
            if (this.Destination != this.Position)
            {
                Vector2 direction = Vector2.Normalize(this.Destination - this.Position);
                this.Position += direction * (float)gameTime.ElapsedGameTime.TotalSeconds * 60;

                if (Vector2.Distance(this.Position, this.Destination) < 1)
                {
                    //direction = Vector2.Zero;
                    this.Destination = this.Position;
                    return true;
                }
            }
            return false;
        }

        public bool Update(GameTime gameTime)
        {
            Move(gameTime);



            return true;
        }

        public bool Draw()
        {
            return true;
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

        public Texture2D Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
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
