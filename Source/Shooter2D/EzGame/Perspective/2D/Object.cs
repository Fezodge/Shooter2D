using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using static EzGame.Perspective.Planar.Textures;

namespace EzGame.Perspective.Planar
{
    public class Object
    {
        public Animation Animation;
        public Texture2D Texture { get { return ((Animation != null) ? Animation.Texture() : null); } set { Animation = new Animation(value); } }
        public Origin Origin { get { return ((Animation != null) ? Animation.Origin() : Origin.None); } set { if (Animation != null) Animation.Override(value); } }

        public Object() { }
        public Object(Animation Animation) { this.Animation = Animation; }
        public Object(Texture2D Texture, Origin Origin = null) { this.Texture = Texture; this.Origin = Origin; }

        public Vector2 Position;
        public float Angle, Scale = 1;
        public double Opacity = 1;

        public List<Filter> Filters;

        public virtual void Update(GameTime Time)
        {
            if (Animation != null) Animation.Update(Time);
            if (Filters != null)
            {
                for (int i = 0; i < Filters.Count; i++)
                {
                    Filters[i].Update(Time);
                    if (Filters[i].Finished) { Filters.RemoveAt(i); i--; continue; }
                }
                if (Filters.Count == 0) Filters = null;
            }
        }

        public virtual void Draw(Color Color, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Draw(Globe.Batches[0], Position, Color, (float)Opacity, Angle, Origin, Scale, Effect, Angle); }
        public virtual void Draw(Batch Batch, Vector2 Position, Color Color, float Opacity, float Angle, Origin Origin, float Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch.Draw(Texture, Position, null, (Color * Opacity), Angle, Origin, Scale, Effect, Angle); }
        public virtual void Draw(Batch Batch, Vector2 Position, Color Color, float Opacity, float Angle, Origin Origin, Vector2 Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        { Batch.Draw(Texture, Position, null, (Color * Opacity), Angle, Origin, Scale, Effect, Angle); }

        /// <summary>
        /// Get the count of how many filters of the provided filter type(s) (combined) there are
        /// </summary>
        /// <param name="Filters">The filter type(s) (separated by comma).</param>
        /// <returns>The amount of filter(s) of the provided type(s) (combined).</returns>
        public int Applied(params Type[] Filters)
        {
            int Count = 0;
            if (this.Filters != null) { for (int i = 0; i < this.Filters.Count; i++) if (Filters.Contains(this.Filters[i].GetType())) Count++; }
            return Count;
        }
        /// <summary>
        /// Apply filter(s) to this object
        /// </summary>
        /// <param name="Filters">The filter(s) (separated by comma).</param>
        public void Implement(params Filter[] Filters)
        {
            if (this.Filters == null) this.Filters = new List<Filter>();
            foreach (Filter Filter in Filters) { Filter.ApplyTo(this); this.Filters.Add(Filter); }
        }
        /// <summary>
        /// Cease/delete filter(s) from this object
        /// </summary>
        /// <param name="Filters">The filter type(s) (separated by comma).</param>
        public void Cease(params Type[] Filters)
        {
            if (this.Filters != null)
            {
                for (int i = 0; i < this.Filters.Count; i++) if (Filters.Contains(this.Filters[i].GetType())) { this.Filters.RemoveAt(i); i--; continue; }
                if (this.Filters.Count == 0) this.Filters = null;
            }
        }
    }
}