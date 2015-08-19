using System;
using Microsoft.Xna.Framework;

namespace EzGame.Perspective.Planar.Filters
{
    public class FadeOut : Filter
    {
        private readonly double Time;

        /// <summary>
        /// Create a new 'Fade Out' Filter
        /// </summary>
        /// <param name="Time">The time (in seconds) to completely fade out.</param>
        public FadeOut(double Time)
        {
            this.Time = Time;
        }

        /// <summary>
        /// Create a new 'Fade Out' Filter
        /// </summary>
        /// <param name="Object">The object this filter is applied to.</param>
        /// <param name="Time">The time (in seconds) to completely fade out.</param>
        public FadeOut(Object Object, double Time) : base(Object)
        {
            Object.Opacity = 0;
            this.Time = Time;
        }

        public override void ApplyTo(Object Object)
        {
            Object.Opacity = 0;
            base.ApplyTo(Object);
        }

        public override void Update(GameTime Time)
        {
            Object.Opacity = Math.Max(0, (Object.Opacity - (Time.ElapsedGameTime.TotalSeconds/this.Time)));
            if (Object.Opacity == 0) Finished = true;
        }
    }
}