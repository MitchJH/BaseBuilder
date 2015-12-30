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
    public static class ObjectDatabase
    {
        private static Dictionary<string, InternalObject> _internalObjects;

        static ObjectDatabase()
        {
        }

        public static void LoadInternalObjects(string file, ContentManager content)
        {
            using (var stream = TitleContainer.OpenStream(file))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#") == false)
                        {
                            string[] split = line.Split(',');
                            string id = split[0];
                            string filepath = split[1];

                            //InternalObject newObject = new InternalObject();
                            //_internalObjects.Add(id, newObject);
                        }
                    }
                }
            }
        }

        public static InternalObject InternalObject(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                if (_internalObjects.ContainsKey(key))
                {
                    return _internalObjects[key];
                }
            }
            return null;
        }
    }
}
