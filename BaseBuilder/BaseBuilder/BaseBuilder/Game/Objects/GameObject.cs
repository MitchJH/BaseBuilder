using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class GameObject : Entity
    {
        private string _ID; // The unique identifier of this object
        private ObjectType _objectType; // What type of object this is

        public GameObject(string ID, string objectType, Vector2 tilePosition)
            : base()
        {
            _ID = ID;
            _objectType = ObjectManager.GetType(objectType);
            base.TilePosition = tilePosition; // Set the tile position of the entity
            base.Position = new Vector2(tilePosition.X * Constants.TILE_SIZE, tilePosition.Y * Constants.TILE_SIZE);

            OverwriteTiles();
        }

        /// <summary>
        /// Determine the position based on the tile.
        /// </summary>
        /// <returns>The X/Y position of the object</returns>
        public Vector2 DeterminePosition()
        {
            if (this.TilePosition != null)
            {
                int x = (int)this.TilePosition.X * Constants.TILE_SIZE;
                int y = (int)this.TilePosition.Y * Constants.TILE_SIZE;

                return new Vector2(x, y);
            }
            else
            {
                return new Vector2(0, 0);
            }
        }

        
        public bool OverwriteTiles()
        {
            //TODO: Scenario for rotating the object. Most likely this will just change the dimensions when rotated before it even comes to this method.
            int x = (int)_objectType.Width;
            int y = (int)_objectType.Height;

            while (y > 0)
            {
                while (x > 0)
                {
                    World.Tiles[((int)this.TilePosition.X - 1) + x, ((int)this.TilePosition.Y - 1) + y].Type = TileType.Impassable;
                    x--;
                }
                x = (int)_objectType.Width;

                World.Tiles[((int)this.TilePosition.X - 1) + x, ((int)this.TilePosition.Y - 1) + y].Type = TileType.Impassable;

                y--;
            }
            return true;
        }
        

        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public ObjectType Type
        {
            get { return _objectType; }
            set { _objectType = value; }
        }
        public ObjectType ObjectType
        {
            get { return _objectType; }
            set { _objectType = value; }
        }
    }
}
