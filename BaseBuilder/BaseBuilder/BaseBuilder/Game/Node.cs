using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class Node
    {
        private Vector2 _position;
        private Vector2 _tile_position;

        public Node()
        {
            
        }

        public bool Update()
        {
            return true;
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 TilePosition
        {
            get { return _tile_position; }
            set { _tile_position = value; }
        }
    }
}
