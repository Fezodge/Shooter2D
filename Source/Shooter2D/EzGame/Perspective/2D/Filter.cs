using Microsoft.Xna.Framework;

namespace EzGame.Perspective.Planar
{
    public class Filter
    {
        protected Object Object;
        public bool Finished;

        public Filter() { }
        public Filter(Object Object) { this.Object = Object; }

        public virtual void ApplyTo(Object Object) { this.Object = Object; }
        public virtual void Update(GameTime Time) { }
    }
}