using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public enum TileType
    {
        Walkable,  // Walkable
        Impassable   // Impassable
    }

    public class Tile : IPathNode<Object>
    {
        private Point _position;
        private Vector2 _center;
        private TileType _type;
        private bool _hovered;
        private string _texture;

        public Tile(int x, int y)
        {
            _position = new Point(x, y);
            _center = new Vector2((x * Constants.TILE_SIZE) + (Constants.TILE_SIZE / 2), (y * Constants.TILE_SIZE) + (Constants.TILE_SIZE / 2));
            _hovered = false;
        }
        
        public bool IsWalkable(Object unused)
        {
            if (_type == TileType.Walkable)
            {
                return true;
            }            

            return false;
        }

        public Point Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 Center
        {
            get { return _center; }
            set { _center = value; }
        }

        public TileType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public bool Hovered
        {
            get { return _hovered; }
            set { _hovered = value; }
        }

        public string Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
    }
}
