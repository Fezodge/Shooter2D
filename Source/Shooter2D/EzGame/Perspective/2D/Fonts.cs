using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using static EzGame.Perspective.Planar.Textures;

namespace EzGame.Perspective.Planar
{
    public static class Fonts
    {
        private static Dictionary<string, SpriteFont> Library = new Dictionary<string, SpriteFont>();

        public static SpriteFont Get(string Path) { Load(Path); return Library[Path]; }
        public static void Load(string Path) { if (!Library.ContainsKey(Path)) { try { Library.Add(Path, Globe.ContentManager.Load<SpriteFont>("Fonts/" + Path)); } catch (Exception e) { throw e; } } }
        public static void Unload(string Path) { if (Library.ContainsKey(Path)) { Library.Remove(Path); Globe.ContentManager.Unload(); } else throw new KeyNotFoundException("The font at \"" + Path + "\" was not found in memory!"); }

        public static void DrawString(string String, SpriteFont Font, Vector2 Position, Origin Origin, Color Fore, Color? Back = null, float Angle = 0, float Scale = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { DrawString(Globe.Batches[0], String, Font, Position, Origin, Fore, Back, Angle, Scale, Effect, Layer); }
        public static void DrawString(Batch Batch, string String, SpriteFont Font, Vector2 Position, Origin Origin, Color Fore, Color? Back = null, float Angle = 0, float Scale = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch.DrawString(String, Font, Position, Origin, Fore, Back, Angle, Scale, Effect, Layer); }

        public static void Draw(this string String, SpriteFont Font, Vector2 Position, Origin Origin, Color Fore, Color? Back = null, float Angle = 0, float Scale = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { DrawString(Globe.Batches[0], String, Font, Position, Origin, Fore, Back, Angle, Scale, Effect, Layer); }
        public static void Draw(this string String, Batch Batch, SpriteFont Font, Vector2 Position, Origin Origin, Color Fore, Color? Back = null, float Angle = 0, float Scale = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { DrawString(Batch, String, Font, Position, Origin, Fore, Back, Angle, Scale, Effect, Layer); }
    }
}