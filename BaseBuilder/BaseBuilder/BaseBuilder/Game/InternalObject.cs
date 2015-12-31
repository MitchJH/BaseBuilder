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
        private int _object_id;         //A number assoiciated with the object should be unique to each object.

        private string[] _object_data;  //Holds all the data needed for this type of object.

        private string _name;           //The name of the object displayed. "Chair, Bed"
        private string _description;    //Describes what the object is. "Something you sit on"
        private string _type;           //What type of object it is. 

        private bool _interactable;     //Can crew interact with it.

        private string _sprite;         //filename of the sprite.
        private Vector2 _dimensions;    //Dimensions in tile size, could be done on the fly I suppose? Pretty easy calculation. Up to you to remove.

        public InternalObject(Vector2 tile_position, string object_type)
            : base()
        {
            TilePosition = tile_position;

            Position = DeterminePosition();

            //Get the relevant data for this object.
            _object_data = ObjectTypes.GetData(object_type);

            //Break down the data and assign to variables.
            ApplyData();

            //Overwrite the tiles it's being placed on.
            OverwriteTiles();
        }

        public bool ApplyData()
        {
            _name = _object_data[0];
            _description = _object_data[1];
            _type = _object_data[2];
            _interactable = bool.Parse(_object_data[3]);
            _sprite = _object_data[4];
            _dimensions.X = int.Parse(_object_data[5]);
            _dimensions.Y = int.Parse(_object_data[6]);

            return true;
        }

        public bool OverwriteTiles()
        {
            //TODO: Scenario for rotating the object. Most likely this will just change the dimensions when rotated before it even comes to this method.
            int x = (int)Dimensions.X;
            int y = (int)Dimensions.Y;

            while (y > 0)
            {
                while (x > 0)
                {
                    World.Tiles[((int)TilePosition.X - 1) + x, ((int)TilePosition.Y - 1) + y].Type = TileType.Impassable;
                    x--;
                }
                x = (int)Dimensions.X;

                World.Tiles[((int)TilePosition.X - 1) + x, ((int)TilePosition.Y - 1) + y].Type = TileType.Impassable;

                y--;
            }
            
            return true;

        }

        /*Determine the position based on the tile.
         */
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

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public Vector2 Dimensions
        {
            get { return _dimensions; }
            set { _dimensions = value; }
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

        public bool Draw()
        {
            return true;
        }

    }
}
