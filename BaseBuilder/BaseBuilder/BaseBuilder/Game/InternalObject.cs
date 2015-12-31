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
        ObjectType _object_type;

        public InternalObject(Vector2 tile_position, string object_type)
            : base()
        {
            TilePosition = tile_position;

            Position = DeterminePosition();

            _object_type = new ObjectType(object_type);

            //Overwrite the tiles it's being placed on.
            OverwriteTiles();
        }


        public bool OverwriteTiles()
        {
            //TODO: Scenario for rotating the object. Most likely this will just change the dimensions when rotated before it even comes to this method.
            int x = (int)_object_type.Dimensions.X;
            int y = (int)_object_type.Dimensions.Y;

            while (y > 0)
            {
                while (x > 0)
                {
                    World.Tiles[((int)TilePosition.X - 1) + x, ((int)TilePosition.Y - 1) + y].Type = TileType.Impassable;
                    x--;
                }
                x = (int)_object_type.Dimensions.X;

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


        public bool Draw()
        {
            return true;
        }

        public ObjectType ObjectType
        {
            get { return _object_type; }
            set { _object_type = value; }
        }
    }
}
