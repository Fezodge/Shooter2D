using System;
using System.Collections.Generic;
using EzGame.Perspective.Planar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EzGame.Collision
{
    public class Polygon
    {
        public float Angle
        {
            get { return PrivateAngle; }
            set
            {
                for (uint i = 0; i < Lines.Length; i++)
                {
                    Lines[i].Start = Rotate(Lines[i].Start, -PrivateAngle);
                    Lines[i].End = Rotate(Lines[i].End, -PrivateAngle);
                }
                PrivateAngle = value;
                while (MathHelper.ToDegrees(PrivateAngle) >= 359) PrivateAngle -= 359;
                for (uint i = 0; i < Lines.Length; i++)
                {
                    Lines[i].Start = Rotate(Lines[i].Start, PrivateAngle);
                    Lines[i].End = Rotate(Lines[i].End, PrivateAngle);
                }
            }
        }

        public Vector2 Min
        {
            get
            {
                var Min = Position;
                for (uint i = 0; i < Lines.Length; i++)
                    Min = new Vector2(Math.Min(Min.X, Math.Min(Lines[i].Start.X, Lines[i].End.X)),
                        Math.Min(Min.Y, Math.Min(Lines[i].Start.Y, Lines[i].End.Y)));
                return Min;
            }
        }

        public Vector2 Max
        {
            get
            {
                var Max = Position;
                for (uint i = 0; i < Lines.Length; i++)
                    Max = new Vector2(Math.Max(Max.X, Math.Max(Lines[i].Start.X, Lines[i].End.X)),
                        Math.Max(Max.Y, Math.Max(Lines[i].Start.Y, Lines[i].End.Y)));
                return Max;
            }
        }

        public Vector2 Size
        {
            get { return new Vector2((Max.X - Min.X), (Max.Y - Min.Y)); }
        }

        public float Width
        {
            get { return Size.X; }
        }

        public float Height
        {
            get { return Size.Y; }
        }

        public Polygon Clone
        {
            get
            {
                var Clone = new Polygon(new Line[Lines.Length]);
                for (uint i = 0; i < Lines.Length; i++)
                    Clone.Lines[i] = new Line(Rotate(Lines[i].Start, -Angle), Rotate(Lines[i].End, -Angle));
                return Clone;
            }
        }

        public Line[] Lines;
        public Vector2 Position, Origin;
        private float PrivateAngle;

        public Polygon()
        {
        }

        public Polygon(Line[] Lines)
        {
            this.Lines = Lines;
        }

        public static Vector2 Rotate(Vector2 Position, float Angle)
        {
            return Vector2.Transform(Position, Matrix.CreateRotationZ(Angle));
        }

        public void Draw(Color Color, float Thickness, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Draw(Globe.Batches[0], Color, Thickness, Effect, Layer);
        }

        public void Draw(Batch Batch, Color Color, float Thickness, SpriteEffects Effect = SpriteEffects.None,
            float Layer = 0)
        {
            for (var i = 0; i < Lines.Length; i++)
                Lines[i].Offset(Position).Draw(Batch, Color, Thickness, Effect, Layer);
        }

        public bool Intersects(Line Line)
        {
            for (uint i = 0; i < Lines.Length; i++) if (Lines[i].Offset(Position).Intersects(Line)) return true;
            return false;
        }

        public bool Intersects(Line Line, ref Vector2 Intersection)
        {
            var Intersects = false;
            for (uint i = 0; i < Lines.Length; i++)
                if (Lines[i].Offset(Position).Intersects(Line, ref Intersection))
                    Intersects = true;
            return Intersects;
        }

        public bool Intersects(Polygon Polygon)
        {
            for (uint i = 0; i < Lines.Length; i++)
                for (uint j = 0; j < Polygon.Lines.Length; j++)
                    if (Lines[i].Offset(Position).Intersects(Polygon.Lines[j].Offset(Polygon.Position)))
                        return true;
            return false;
        }

        public bool Intersects(Polygon Polygon, ref Vector2 Intersection)
        {
            for (uint i = 0; i < Lines.Length; i++)
                for (uint j = 0; j < Polygon.Lines.Length; j++)
                    if (Lines[i].Offset(Position)
                        .Intersects(Polygon.Lines[j].Offset(Polygon.Position), ref Intersection))
                        return true;
            return false;
        }

        public static Polygon CreateLine(Line Line)
        {
            return new Polygon(new[] {Line});
        }

        public static Polygon CreateLines(Line[] Lines)
        {
            return new Polygon(Lines);
        }

        public static Polygon CreateSquare(float Radius, Vector2 Origin)
        {
            return CreateRectangle(new Vector2(Radius), Origin);
        }

        public static Polygon CreateRectangle(Vector2 Size, Vector2 Origin)
        {
            Line[] Lines =
            {
                new Line((new Vector2(-(Size.X/2), -(Size.Y/2)) - Origin),
                    (new Vector2((Size.X/2), -(Size.Y/2)) - Origin)),
                new Line((new Vector2((Size.X/2), -(Size.Y/2)) - Origin), (new Vector2((Size.X/2), (Size.Y/2)) - Origin)),
                new Line((new Vector2(-(Size.X/2), -(Size.Y/2)) - Origin),
                    (new Vector2(-(Size.X/2), (Size.Y/2)) - Origin)),
                new Line((new Vector2(-(Size.X/2), (Size.Y/2)) - Origin), (new Vector2((Size.X/2), (Size.Y/2)) - Origin))
            };
            return new Polygon(Lines) {Origin = Origin};
        }

        public static Polygon CreateCircle(float Radius, Vector2 Origin, byte Sides = 8)
        {
            return CreateEllipse(new Vector2(Radius), Origin, Sides);
        }

        public static Polygon CreateEllipse(Vector2 Radius, Vector2 Origin, byte Sides = 8)
        {
            if (Sides < 5) return CreateRectangle(Radius, Origin);
            var Start = (new Vector2(-((Radius.X/Sides*4)/2), (-(Radius.Y/2) - ((Radius.Y/Sides*4)/2))) - Origin);
            var Lines = new List<Line>();
            for (var Side = 0; Side < Sides; Side++)
            {
                var Angle = MathHelper.ToRadians((Side/(float) Sides)*360);
                var End = (Start +
                           new Vector2(((float) (Math.Cos(Angle)*(Radius.X/Sides*4))),
                               ((float) (Math.Sin(Angle)*(Radius.Y/Sides*4)))));
                Lines.Add(new Line(Start, End));
                Start = End;
            }
            return new Polygon(Lines.ToArray()) {Origin = Origin};
        }

        public static Polygon CreateCross(float Radius, Vector2 Origin)
        {
            return CreateCross(new Vector2(Radius), Origin);
        }

        public static Polygon CreateCross(Vector2 Radius, Vector2 Origin)
        {
            Line[] Lines =
            {
                new Line((new Vector2(-(Radius.X/2f), -(Radius.X/2f)) - Origin),
                    (new Vector2((Radius.Y/2f), (Radius.Y/2f)) - Origin)),
                new Line((new Vector2((Radius.X/2f), -(Radius.X/2f)) - Origin),
                    (new Vector2(-(Radius.Y/2f), (Radius.Y/2f)) - Origin))
            };
            return new Polygon(Lines);
        }

        public static Polygon CreateSquareWithCross(float Radius, Vector2 Origin)
        {
            return CreateRectangleWithCross(new Vector2(Radius), Origin);
        }

        public static Polygon CreateRectangleWithCross(Vector2 Size, Vector2 Origin)
        {
            Line[] Lines =
            {
                new Line((new Vector2(-(Size.X/2), -(Size.Y/2)) - Origin),
                    (new Vector2((Size.X/2), -(Size.Y/2)) - Origin)),
                new Line((new Vector2((Size.X/2), -(Size.Y/2)) - Origin), (new Vector2((Size.X/2), (Size.Y/2)) - Origin)),
                new Line((new Vector2(-(Size.X/2), -(Size.Y/2)) - Origin),
                    (new Vector2(-(Size.X/2), (Size.Y/2)) - Origin)),
                new Line((new Vector2(-(Size.X/2), (Size.Y/2)) - Origin), (new Vector2((Size.X/2), (Size.Y/2)) - Origin)),
                new Line((new Vector2(-(Size.X/2f), -(Size.Y/2f)) - Origin),
                    (new Vector2((Size.X/2f), (Size.Y/2f)) - Origin)),
                new Line((new Vector2((Size.X/2f), -(Size.Y/2f)) - Origin),
                    (new Vector2(-(Size.X/2f), (Size.Y/2f)) - Origin))
            };
            return new Polygon(Lines) {Origin = Origin};
        }
    }
}