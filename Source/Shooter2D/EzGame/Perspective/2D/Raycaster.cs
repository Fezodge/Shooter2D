using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using EzGame.Collision;

namespace EzGame.Perspective.Planar
{
    public class Raycaster
    {
        public Ray[] Rays;

        public Raycaster(uint Rays) { this.Rays = new Ray[Math.Max(1, Rays)]; }

        public void Cast(Vector2 Position, float Length, params Polygon[] Obstacles) { Cast(Position, Length, MathHelper.ToRadians(360), 0, Obstacles); }
        public void Cast(Vector2 Position, float Length, float Field, float Angle, params Polygon[] Obstacles)
        {
            Ray[] Rays = new Ray[this.Rays.Length];
            for (uint i = 0; i < Rays.Length; i++)
            {
                float RayAngle = ((Angle - (Field / 2)) + ((i / (float)Rays.Length) * Field));
                Rays[i] = new Ray(Position, Globe.Move(Position, RayAngle, Math.Max(1, Length)));
                foreach (Polygon Obstacle in Obstacles) if (Obstacle.Intersects(Rays[i], ref Rays[i].End)) Rays[i].Obstacle = Obstacle;
            }
            this.Rays = Rays;
        }

        public void DrawLines(Color? Color = null, float Size = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0) { DrawLines(Globe.Batches[0], Color, Size, Effect, Layer); }
        public void DrawLines(Batch Batch, Color? Color = null, float Size = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            for (uint i = 0; i < Rays.Length; i++)
                if (Rays[i] != null)
                    Rays[i].Draw(Batch, (Color ?? Microsoft.Xna.Framework.Color.White), Size, Effect, Layer);
        }
        public void DrawAreaOutline(Color Color, float Size = 1) { DrawAreaOutline(Globe.Batches[0], Color, Size); }
        public void DrawAreaOutline(Batch Batch, Color Color, float Size = 1) { CreateArea.Draw(Batch, Color, Size); }

        public Polygon CreateArea
        {
            get
            {
                Polygon Area = new Polygon(new Line[Rays.Length]);
                for (uint i = 0; i < Rays.Length; i++) Area.Lines[i] = new Line(Rays[i].End, Rays[((i == (Rays.Length - 1)) ? 0 : (i + 1))].End);
                return Area;
            }
        }

        public bool Intersects(Polygon Polygon) { for (uint i = 0; i < Rays.Length; i++) if ((Rays[i] != null) && (Rays[i].Intersects(Polygon))) return true; return false; }
        public bool Intersects(Polygon Polygon, ref Vector2 Intersection) { for (uint i = 0; i < Rays.Length; i++) if ((Rays[i] != null) && (Rays[i].Intersects(Polygon, ref Intersection))) return true; return false; }

        public class Ray : Line
        {
            public Polygon Obstacle;

            public Ray(Vector2 Start, Vector2 End) : base(Start, End) { }

            public bool Intersects(Polygon Polygon) { return (Polygon == Obstacle); }
            public bool Intersects(Polygon Polygon, ref Vector2 Intersection) { Intersection = End; return (Polygon == Obstacle); }
        }
    }
}