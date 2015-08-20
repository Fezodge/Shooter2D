using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Shooter2D
{
    public static class Mod
    {
        public static Dictionary<ushort, Tile> Fore, Back;

        public static Dictionary<ushort, Tile> Load(string Path)
        {
            Dictionary<ushort, Tile> Array = new Dictionary<ushort, Tile>();
            if (!File.Exists(Path)) return null;
            XmlTextReader Reader = new XmlTextReader(Path);
            ushort ID = 0;
            Tile Tile = null;
            while (Reader.Read())
                switch (Reader.NodeType)
                {
                    case XmlNodeType.Element:
                        bool End = false;
                        if (Reader.Name == "Tile")
                        {
                            End = Reader.IsEmptyElement;
                            Tile = new Tile();
                            while (Reader.MoveToNextAttribute())
                                if (Reader.Name == "ID") ID = Convert.ToUInt16(Reader.Value);
                                else if (Reader.Name == "Solid") Tile.Solid = Convert.ToBoolean(Reader.Value);
                                else if (Reader.Name == "Border") Tile.Border = Convert.ToBoolean(Reader.Value);
                                else if (Reader.Name == "ClipToFore") Tile.ClipToFore = Convert.ToBoolean(Reader.Value);
                                else if (Reader.Name == "Invisible") Tile.Invisible = Convert.ToBoolean(Reader.Value);
                                else if (Reader.Name == "Waypoint") Tile.Waypoint = Convert.ToByte(Reader.Value);
                        }
                        if (End)
                        {
                            Array.Add(ID, Tile);
                            ID = 0; Tile = null;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (Reader.Name == "Tile")
                        {
                            Array.Add(ID, Tile);
                            ID = 0; Tile = null;
                        }
                        break;
                }
            Reader.Close();
            return Array;
        }

        public class Tile
        {
            public bool Solid, Border, ClipToFore, Invisible;
            public byte? Waypoint;
        }
    }
}