using System;
using EzGame.Perspective.Planar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static EzGame.Perspective.Planar.Textures;

namespace EzGame.Collision
{
    public class Line
    {
        public Vector2 Start, End;

        public Line(Vector2 Start, Vector2 End)
        {
            this.Start = Start;
            this.End = End;
        }

        public void Draw(Color Color, float Thickness = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Draw(Globe.Batches[0], Color, Thickness, Effect, Layer);
        }

        public void Draw(Batch Batch, Color Color, float Thickness = 1, SpriteEffects Effect = SpriteEffects.None,
            float Layer = 0)
        {
            float Angle = (float) Math.Atan2((End.Y - Start.Y), (End.X - Start.X)),
                Length = Vector2.Distance(Start, End);
            Batch.Draw(Pixel(Color.White, true), Start, null, Color, Angle, Origin.None, new Vector2(Length, Thickness),
                Effect, Layer);
        }

        public Line Offset(Vector2 Position)
        {
            return new Line((Start + Position), (End + Position));
        }

        public bool Intersects(Line Line)
        {
            var Intersection = Vector2.Zero;
            return Intersects(Line, ref Intersection);
        }

        public bool Intersects(Line Line, ref Vector2 Intersection)
        {
            var b = End - Start;
            var d = Line.End - Line.Start;
            var bDotDPerp = b.X*d.Y - b.Y*d.X;
            if (bDotDPerp == 0) return false;
            var c = Line.Start - Start;
            var t = (c.X*d.Y - c.Y*d.X)/bDotDPerp;
            if (t < 0 || t > 1) return false;
            var u = (c.X*b.Y - c.Y*b.X)/bDotDPerp;
            if (u < 0 || u > 1) return false;
            Intersection = Start + t*b;
            return true;
        }
    }
}