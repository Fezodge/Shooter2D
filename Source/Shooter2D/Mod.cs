using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using EzGame.Perspective.Planar;
using static EzGame.Perspective.Planar.Textures;

namespace Shooter2D
{
    public static class Mod
    {
        public static Dictionary<ushort, Tile> Fore, Back;
        public static Dictionary<ushort, Weapon> Weapons;

        public static Dictionary<ushort, Tile> LoadTiles(string Path)
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
                                else if (Reader.Name == "Type") Tile.Type = (Tile.Types)Enum.Parse(typeof(Tile.Types), Reader.Value);
                                else if (Reader.Name == "Border") Tile.Border = Convert.ToBoolean(Reader.Value);
                                else if (Reader.Name == "ClipToFore") Tile.ClipToFore = Convert.ToBoolean(Reader.Value);
                                else if (Reader.Name == "Invisible") Tile.Invisible = Convert.ToBoolean(Reader.Value);
                                else if (Reader.Name == "Waypoint") Tile.Waypoint = Convert.ToByte(Reader.Value);
                                else if (Reader.Name == "Frames") Tile.Frames = Convert.ToByte(Reader.Value);
                                else if (Reader.Name == "Speed") Tile.Speed = Convert.ToSingle(Reader.Value);
                        }
                        if (Reader.Name == "Animation")
                        {
                            while (Reader.MoveToNextAttribute())
                                if (Reader.Name == "Frames") Tile.Frames = Convert.ToByte(Reader.Value);
                                else if (Reader.Name == "Speed") Tile.Speed = Convert.ToSingle(Reader.Value);
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
        public static Dictionary<ushort, Weapon> LoadWeapons(string Path)
        {
            Dictionary<ushort, Weapon> Array = new Dictionary<ushort, Weapon>();
            if (!File.Exists(Path)) return null;
            XmlTextReader Reader = new XmlTextReader(Path);
            ushort ID = 0;
            Weapon Weapon = null;
            while (Reader.Read())
                switch (Reader.NodeType)
                {
                    case XmlNodeType.Element:
                        bool End = false;
                        if (Reader.Name == "Weapon")
                        {
                            End = Reader.IsEmptyElement;
                            Weapon = new Weapon();
                            while (Reader.MoveToNextAttribute())
                                if (Reader.Name == "Name") Weapon.Name = Reader.Value;
                                else if (Reader.Name == "Texture") Weapon.Texture = Reader.Value;
                                else if (Reader.Name == "RoundsPerSecond") Weapon.RoundsPerSecond = Convert.ToDouble(Reader.Value);
                                else if (Reader.Name == "Damage") Weapon.Damage = Convert.ToSingle(Reader.Value);
                        }
                        if (Reader.Name == "Origin")
                        {
                            while (Reader.MoveToNextAttribute())
                                if (Reader.Name == "Player") Weapon.Player = new Vector2(Convert.ToSingle(Reader.Value.Split(',')[0]), Convert.ToSingle(Reader.Value.Split(',')[1]));
                                else if (Reader.Name == "Bullet") Weapon.Bullet = new Vector2(Convert.ToSingle(Reader.Value.Split(',')[0]), Convert.ToSingle(Reader.Value.Split(',')[1]));
                        }
                        if (End)
                        {
                            Array.Add(ID, Weapon);
                            ID++; Weapon = null;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (Reader.Name == "Weapon")
                        {
                            Array.Add(ID, Weapon);
                            ID++; Weapon = null;
                        }
                        break;
                }
            Reader.Close();
            return Array;
        }

        public class Tile
        {
            public enum Types { Floor, Platform, Wall }
            public Types Type = Types.Floor;

            public bool Border, ClipToFore, Invisible;
            public byte? Waypoint;

            public Animation Animation;
            public byte Frames;
            public float Speed;
        }

        public class Weapon
        {
            public string Name, Texture;
            public double RoundsPerSecond;
            public float Damage;
            public Vector2 Player, Bullet;
        }
    }
}