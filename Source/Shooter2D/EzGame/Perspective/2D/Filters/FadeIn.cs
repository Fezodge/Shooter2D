using Microsoft.Xna.Framework;
using System;

namespace EzGame.Perspective.Planar.Filters
{
    public class FadeIn : Filter
    {
        private double Time;

        /// <summary>
        /// Create a new 'Fade In' Filter
        /// </summary>
        /// <param name="Time">The time (in seconds) to completely fade in.</param>
        public FadeIn(double Time) { this.Time = Time; }
        /// <summary>
        /// Create a new 'Fade In' Filter
        /// </summary>
        /// <param name="Object">The object this filter is applied to.</param>
        /// <param name="Time">The time (in seconds) to completely fade in.</param>
        public FadeIn(Object Object, double Time) : base(Object) { Object.Opacity = 0; this.Time = Time; }

        public override void ApplyTo(Object Object) { Object.Opacity = 0; base.ApplyTo(Object); }
        public override void Update(GameTime Time)
        {
            Object.Opacity = Math.Min(1, (Object.Opacity + (Time.ElapsedGameTime.TotalSeconds / this.Time)));
            if (Object.Opacity == 1) Finished = true;
        }
    }
}