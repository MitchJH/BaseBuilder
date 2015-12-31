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
    public static class ObjectTypes
    {
        private static int _TOTAL_OBJECTS = 3;
        private static int _TOTAL_FIELDS = 7;

        private static string[,] _objects_data = new string[_TOTAL_OBJECTS, _TOTAL_FIELDS];
        private static string[] _object_data = new string[_TOTAL_FIELDS];

        static ObjectTypes()
        {

        }


        public static void LoadObjectBank(string file, ContentManager content)
        {
            using (var stream = TitleContainer.OpenStream(file))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    int i = -1;
                    int j = 0;

                    //Navigate the 2D array to read in the data entries.
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#") == false && string.IsNullOrEmpty(line) == false)
                        {
                            string[] split = line.Split(',');

                            while (split.Length > j)
                            {
                                _objects_data[i, j] = split[j];
                                j++;
                            }
                        }
                        i++;
                        j = 0;
                    }
                }
            }
        }

        public static string[] GetData(string key)
        {
            if (string.IsNullOrEmpty(key) == false)
            {
                for (int i = 0; i < _TOTAL_OBJECTS; i++ )
                {
                    if(key == _objects_data[i, 0])
                    {
                        for (int j = 0; j < _TOTAL_FIELDS; j++)
                        {
                            //Send the relevant line of data off to the class that needs it.
                            _object_data[j] = _objects_data[i, j];
                        }
                        return _object_data;
                    }
                }
            }
            return _object_data;
        }
    }
}
