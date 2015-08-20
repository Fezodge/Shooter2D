using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static EzGame.Perspective.Planar.Textures;

namespace EzGame.Perspective.Planar
{
    public class Animation
    {
        public bool Finished
        {
            get
            {
                return ((Index == (Frames.Length - 1)) &&
                        (Timer >= (Frames[Index].Speed.HasValue ? Frames[Index].Speed.Value : Speed)));
            }
        }

        public Frame[] Frames;
        public uint Index;
        public bool Loop, Paused;
        public Origin Origins = Textures.Origin.None;
        public float Speed;
        private double Timer;

        public Animation(Texture2D Texture, Origin Origin = null)
        {
            Frames = new Frame[1];
            Frames[0] = new Frame(Texture, Origin);
        }

        public Animation(string Path, int Frames, bool Loop = false, float? Speed = null)
        {
            this.Frames = new Frame[Frames];
            for (var i = 0; i < Frames; i++)
            {
                var Texture = Get(Path + ((Path.EndsWith("/") || Path.EndsWith("-")) ? string.Empty : "/") + i);
                this.Frames[i] = new Frame(Texture, Textures.Origin.Center);
            }
            this.Loop = Loop;
            if (Speed.HasValue) this.Speed = Speed.Value;
        }

        public Animation(string Path, int Frames, bool Loop = false, float? Speed = null, params Origin[] Origins)
        {
            this.Frames = new Frame[Frames];
            for (var i = 0; i < Frames; i++)
            {
                var Texture = Get(Path + ((Path.EndsWith("/") || Path.EndsWith("-")) ? string.Empty : "/") + i);
                var Origin = ((Origins.Length > i) ? EzGame.Perspective.Planar.Textures.Origin.Center : Origins[i]);
                this.Frames[i] = new Frame(Texture, Origin);
            }
            this.Loop = Loop;
            if (Speed.HasValue) this.Speed = Speed.Value;
        }

        public void Override(Origin Origin)
        {
            for (var i = 0; i < Frames.Length; i++) Frames[i].Origin = null;
            Origins = Origin;
        }

        public void Update(GameTime Time)
        {
            if (!Paused)
                if (Frames[Index].Speed.HasValue)
                {
                    if (Timer < Frames[Index].Speed) Timer += Time.ElapsedGameTime.TotalSeconds;
                    else
                    {
                        if (Index < (Frames.Length - 1))
                        {
                            Index++;
                            Timer -= Frames[Index].Speed.Value;
                        }
                        else if (Loop)
                        {
                            Timer -= Frames[Index].Speed.Value;
                            Index = 0;
                        }
                    }
                }
                else
                {
                    if (Timer < Speed) Timer += Time.ElapsedGameTime.TotalSeconds;
                    else
                    {
                        if (Index < (Frames.Length - 1))
                        {
                            Index++;
                            Timer -= Speed;
                        }
                        else if (Loop)
                        {
                            Index = 0;
                            Timer -= Speed;
                        }
                    }
                }
        }

        public void Draw(Vector2 Position, Color Color, float Angle, float Scale,
            SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Draw(Globe.Batches[0], Position, Color, Angle, Scale, Effect, Layer);
        }

        public void Draw(Vector2 Position, Color Color, float Angle, Vector2 Scale,
            SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Draw(Globe.Batches[0], Position, Color, Angle, Scale, Effect, Layer);
        }

        public void Draw(Batch Batch, Vector2 Position, Color Color, float Angle, float Scale,
            SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Batch.Draw(Texture(), Position, null, Color, Angle, Origin(), Scale, Effect, Layer);
        }

        public void Draw(Batch Batch, Vector2 Position, Color Color, float Angle, Vector2 Scale,
            SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Batch.Draw(Texture(), Position, null, Color, Angle, Origin(), Scale, Effect, Layer);
        }

        public Texture2D Texture(uint? Index = null)
        {
            if (Frames == null) return null;
            if (Index == null)
            {
                if (Frames.Length > this.Index) return Frames[this.Index].Texture;
            }
            else if (Frames.Length > Index) return Frames[(uint) Index].Texture;
            return null;
        }

        public Origin Origin(uint? Index = null)
        {
            if (Frames == null) return Textures.Origin.None;
            if (Index == null)
            {
                if (Frames.Length > this.Index)
                    return (Frames[this.Index].Origin ?? (Origins ?? Textures.Origin.Center));
            }
            else if (Frames.Length > Index) return (Frames[Index.Value].Origin ?? (Origins ?? Textures.Origin.Center));
            return null;
        }

        public class Frame
        {
            public Origin Origin = Origin.None;
            public float? Speed;
            public Texture2D Texture;

            public Frame(Origin Origin)
            {
                this.Origin = Origin;
            }

            public Frame(Texture2D Texture)
            {
                this.Texture = Texture;
            }

            public Frame(Texture2D Texture, float Speed)
            {
                this.Texture = Texture;
                this.Speed = Speed;
            }

            public Frame(float Speed, Origin Origin = null)
            {
                this.Speed = Speed;
                this.Origin = (Origin ?? Origin.Center);
            }

            public Frame(Texture2D Texture, Origin Origin = null)
            {
                this.Texture = Texture;
                this.Origin = Origin;
            }

            public Frame(Texture2D Texture, float Speed, Origin Origin = null)
            {
                this.Texture = Texture;
                this.Speed = Speed;
                this.Origin = Origin;
            }
        }
    }
}