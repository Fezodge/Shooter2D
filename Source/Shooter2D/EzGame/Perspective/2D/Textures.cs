using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EzGame.Perspective.Planar
{
    public static class Textures
    {
        private static Dictionary<string, Texture2D> Library = new Dictionary<string, Texture2D>();

        public static Texture2D Get(string Path) { Load(Path); return Library[Path]; }
        public static void Load(string Path) { if (!Library.ContainsKey(Path)) { try { Library.Add(Path, Globe.ContentManager.Load<Texture2D>(Path)); } catch (Exception e) { throw e; } } }
        public static void Unload(string Path) { if (Library.ContainsKey(Path)) { Library.Remove(Path); Globe.ContentManager.Unload(); } else throw new KeyNotFoundException("The texture at \"" + Path + "\" was not found in memory!"); }

        public class Origin
        {
            public Vector2 Value;
            public bool IsScale;

            /// <summary>
            /// Create a new Origin
            /// </summary>
            /// <param name="Value">The value of both X and Y axis.</param>
            /// <param name="IsScale">Is the value a scale of the texture size?</param>
            public Origin(float Value, bool IsScale = false) { this.Value = new Vector2(Value); this.IsScale = IsScale; }
            /// <summary>
            /// Create a new Origin
            /// </summary>
            /// <param name="X">The X axis value.</param>
            /// <param name="Y">The Y axis value.</param>
            /// <param name="IsScale">Is the value a scale of the texture size?</param>
            public Origin(float X, float Y, bool IsScale = false) { Value = new Vector2(X, Y); this.IsScale = IsScale; }

            public static Origin None { get { return Zero; } }
            public static Origin Zero { get { return new Origin(0); } }
            public static Origin Middle { get { return Center; } }
            public static Origin Center { get { return new Origin(.5f) { IsScale = true }; } }
        }

        /// <summary>
        /// Get/Make a 1x1 Colored Pixel
        /// </summary>
        /// <param name="Color">The Color of the Pixel.</param>
        /// <param name="Store">Store the Pixel in Memory, for Regular usage?</param>
        /// <returns>A 1x1 Colored Pixel.</returns>
        public static Texture2D Pixel(Color Color, bool Store = false)
        {
            string Name = (Color.R + Color.G + Color.B).ToString();
            if (Store && Library.ContainsKey(Name)) return Library[Name];
            Texture2D Pixel = new Texture2D(Globe.GraphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color });
            if (Store) Library.Add(Name, Pixel);
            return Pixel;
        }

        public static void Draw(string Path, Rectangle Bounds, Rectangle? Source, Color Color, float Angle, Origin Origin, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch Batch = Globe.Batches[0]; Batch.Draw(Get(Path), Bounds, Source, Color, Angle, Origin, Effect, Layer); }
        public static void Draw(string Path, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, float Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch Batch = Globe.Batches[0]; Batch.Draw(Get(Path), Position, Source, Color, Angle, Origin, Scale, Effect, Layer); }
        public static void Draw(string Path, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, Vector2 Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch Batch = Globe.Batches[0]; Batch.Draw(Get(Path), Position, Source, Color, Angle, Origin, Scale, Effect, Layer); }
        public static void Draw(this Texture2D Texture, Rectangle Bounds, Rectangle? Source, Color Color, float Angle, Origin Origin, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Globe.Batches[0].Draw(Texture, Bounds, Source, Color, Angle, Origin, Effect, Layer); }
        public static void Draw(this Texture2D Texture, Batch Batch, Rectangle Bounds, Rectangle? Source, Color Color, float Angle, Origin Origin, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch.Draw(Texture, Bounds, Source, Color, Angle, Origin, Effect, Layer); }
        public static void Draw(this Texture2D Texture, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, float Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Globe.Batches[0].Draw(Texture, Position, Source, Color, Angle, Origin, Scale, Effect, Layer); }
        public static void Draw(this Texture2D Texture, Batch Batch, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, float Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch.Draw(Texture, Position, Source, Color, Angle, Origin, Scale, Effect, Layer); }
        public static void Draw(this Texture2D Texture, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, Vector2 Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Globe.Batches[0].Draw(Texture, Position, Source, Color, Angle, Origin, Scale, Effect, Layer); }
        public static void Draw(this Texture2D Texture, Batch Batch, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, Vector2 Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch.Draw(Texture, Position, Source, Color, Angle, Origin, Scale, Effect, Layer); }
    }
}