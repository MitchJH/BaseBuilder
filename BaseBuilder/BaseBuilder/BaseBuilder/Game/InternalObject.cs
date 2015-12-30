using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class InternalObject : Entity
    {
        private string _name;           //The name of the object.
        private string _description;    //Describes what the object is.
        private string _type;           //What type of object it is.

        private int _object_id;         //A number assoiciated with the object so we know what it is and how many.
        private bool _interactable;     //Can crew interact with it.

        private string _sprite;         //filename of the sprite.
        private Vector2 _dimensions;    //Dimensions in tile size.

        public InternalObject()
            : base()
        {
            _name = "Default Object";
            _description = "A default object, used indoors.";
            _type = "Default";
            _object_id = 0;
            _interactable = true;

            Position = new Vector2(0, 0);

            
        }

        public InternalObject(string name, string description, string type, int object_id, Vector2 tile_position, string sprite, Vector2 dimensions)
            : base()
        {
            _name = name;
            _description = description;
            _type = type;
            _object_id = object_id;

            //Position = position;
            TilePosition = tile_position;
            _interactable = true;
            _sprite = sprite;

            _dimensions = dimensions;

            //Overwrite the tiles it's being placed on.
            OverwriteTiles();
            //Determine the position based on the tile.
            Position = DeterminePosition();
            
        }

        public bool OverwriteTiles()
        {
            //TODO: Scenario for rotating the object. Most likely this will just change the dimensions when rotated before it even comes to this method.
            int x = (int)_dimensions.X;
            int y = (int)_dimensions.Y;

            while (y > 0)
            {
                while (x > 0)
                {
                    World.Tiles[((int)TilePosition.X - 1) + x, ((int)TilePosition.Y - 1) + y].Type = TileType.Impassable;
                    x--;
                }
                x = (int)_dimensions.X;

                World.Tiles[((int)TilePosition.X - 1) + x, ((int)TilePosition.Y - 1) + y].Type = TileType.Impassable;

                y--;
            }

            return true;

        }

        public Vector2 DeterminePosition()
        {
            if (TilePosition != null)
            {
                int x = (int)TilePosition.X * Constants.TILE_SIZE;
                int y = (int)TilePosition.Y * Constants.TILE_SIZE;

                return new Vector2(x, y);
            }
            else
            {
                return new Vector2(0, 0);
            }
        }
        public bool Update(GameTime gameTime)
        {
            base.Update();
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
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
        public string Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }
    }
}
