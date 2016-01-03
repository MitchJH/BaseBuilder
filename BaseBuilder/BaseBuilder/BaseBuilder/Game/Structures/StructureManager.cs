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
    public static class StructureManager
    {
        private static Dictionary<string, StructureType> _structureTypes;

        static StructureManager()
        {
            _structureTypes = new Dictionary<string, StructureType>();
        }

        /// <summary>
        /// This will return an structure type based on its key
        /// </summary>
        /// <param name="key">The key of the structure type</param>
        /// <returns>An structure type</returns>
        public static StructureType GetType(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_structureTypes.ContainsKey(key))
                {
                    return _structureTypes[key];
                }
            }
            return null;
        }

        public static void LoadStructureBank(string file, ContentManager content)
        {
            using (var reader = new StreamReader(TitleContainer.OpenStream(file)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#") == false && string.IsNullOrEmpty(line) == false)
                    {
                        string[] split = line.Split(',');
                        string id = split[0];
                        string name = split[1];
                        string description = split[2];
                        string sprite = split[3];
                        int width = int.Parse(split[4]);
                        int height = int.Parse(split[5]);

                        StructureType newStructure = new StructureType(id, name, description, sprite, width, height);
                        _structureTypes.Add(id, newStructure);
                    }
                }
            }
        }

        public static Dictionary<string, StructureType> StructureTypes
        {
            get { return _structureTypes; }
            set { _structureTypes = value; }
        }
    }
}
