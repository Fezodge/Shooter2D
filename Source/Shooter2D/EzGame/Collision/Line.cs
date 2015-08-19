using EzGame.Perspective.Planar;
using static EzGame.Perspective.Planar.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EzGame.Collision
{
    public class Line
    {
        public Vector2 Start, End;

        public Line(Vector2 Start, Vector2 End) { this.Start = Start; this.End = End; }

        public void Draw(Color Color, float Thickness = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Draw(Globe.Batches[0], Color, Thickness, Effect, Layer); }
        public void Draw(Batch Batch, Color Color, float Thickness = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            float Angle = (float)Math.Atan2((End.Y - Start.Y), (End.X - Start.X)), Length = Vector2.Distance(Start, End);
            Batch.Draw(Pixel(Color.White, true), Start, null, Color, Angle, Origin.None, new Vector2(Length, Thickness), Effect, Layer);
        }

        public Line Offset(Vector2 Position) { return new Line((Start + Position), (End + Position)); }

        public bool Intersects(Line Line) { Vector2 Intersection = Vector2.Zero; return Intersects(Line, ref Intersection); }
        public bool Intersects(Line Line, ref Vector2 Intersection)
        {
            Vector2 b = End - Start;
            Vector2 d = Line.End - Line.Start;
            float bDotDPerp = b.X * d.Y - b.Y * d.X;
            if (bDotDPerp == 0) return false;
            Vector2 c = Line.Start - Start;
            float t = (c.X * d.Y - c.Y * d.X) / bDotDPerp;
            if (t < 0 || t > 1) return false;
            float u = (c.X * b.Y - c.Y * b.X) / bDotDPerp;
            if (u < 0 || u > 1) return false;
            Intersection = Start + t * b;
            return true;
        }
    }
}