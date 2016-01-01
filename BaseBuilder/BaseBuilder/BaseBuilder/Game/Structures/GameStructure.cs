using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BaseBuilder
{
    public class GameStructure : Entity
    {
        private string _ID; // The unique identifier of this object
        private StructureType _structureType; // What type of object this is

        public GameStructure(string ID, string structureType, Vector2 tilePosition)
            : base()
        {
            _ID = ID;
            _structureType = StructureManager.GetType(structureType);
            base.TilePosition = tilePosition; // Set the tile position of the entity
            base.Position = new Vector2(tilePosition.X * Constants.TILE_SIZE, tilePosition.Y * Constants.TILE_SIZE);
        }

        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public StructureType StructureType
        {
            get { return _structureType; }
            set { _structureType = value; }
        }
    }
}
