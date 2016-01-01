using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseBuilder
{
    public class StructureType
    {
        private string _key;
        private string _name;
        private string _description;
        private string _sprite;
        private int _width;
        private int _height;

        public StructureType()
        {
        }
        public StructureType(string key, string name, string description, string sprite, int width, int height)
        {
        }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
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

        public string Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
