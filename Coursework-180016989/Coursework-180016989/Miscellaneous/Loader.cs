using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Coursework_180016989
{
    class Loader
    {
        public Loader() { }

        public void ReadLinesFromText(Stream stream, ref List<string> strings)
        {
            string line = "";

            try
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    // Reads a complete line in the text file
                    // Ends when it can't read any line
                    while ((line = sr.ReadLine()) != null)
                    {
                        strings.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: XML File could not be read!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
        }

        public void LoadGame(string fileName)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    // Creating a new serializer of type GameInfo while will 
                    // deserialize the XML File i.e translate to machine readable state.
                    SaveInfo.Instance = (SaveInfo)new XmlSerializer(typeof(SaveInfo)).Deserialize(reader.BaseStream);
                    reader.Close();
                }

            }
            catch (Exception e)
            {
                // If we've caught an exception, output an error message
                // describing the error
                Console.WriteLine("ERROR: XML File could not be deserialized!");
                Console.WriteLine("Exception Message: " + e.Message);
            }
            
        }

        public void SaveGame(string fileName, SaveInfo info)
        {
            // Creating a settings variable
            // to apply indentation to the XML File
            XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };
            XmlSerializer xmlSerializer = new XmlSerializer(info.GetType());

            // XmlWrites creates an instance of the name
            // if one doesn't exist.
            using (XmlWriter xmlWriter = XmlWriter.Create(fileName, settings))
            {
                xmlSerializer.Serialize(xmlWriter, info);
            }

        }

    }
}
