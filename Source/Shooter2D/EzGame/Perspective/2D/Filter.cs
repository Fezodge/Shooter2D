using Microsoft.Xna.Framework;

namespace EzGame.Perspective.Planar
{
    public class Filter
    {
        public bool Finished;
        protected Object Object;

        public Filter()
        {
        }

        public Filter(Object Object)
        {
            this.Object = Object;
        }

        public virtual void ApplyTo(Object Object)
        {
            this.Object = Object;
        }

        public virtual void Update(GameTime Time)
        {
        }
    }
}