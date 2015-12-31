using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BaseBuilder
{
    public class ObjectType
    {
        private string _key;
        private string _name;
        private string _description;
        private string _sprite;
        private bool _isInternal;
        private bool _isInteractable;
        private int _width;
        private int _height;

        public ObjectType()
        {
        }
        public ObjectType(string key, string name, string description, string sprite, bool isInternal, bool isInteractable,  int width, int height)
        {
            _key = key;
            _name = name;
            _description = description;
            _sprite = sprite;
            _isInternal = isInternal;
            _isInteractable = isInteractable;
            _width = width;
            _height = height;
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

        public bool Internal
        {
            get { return _isInternal; }
            set { _isInternal = value; }
        }

        public bool Interactable
        {
            get { return _isInteractable; }
            set { _isInteractable = value; }
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
