using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static EzGame.Perspective.Planar.Textures;

namespace EzGame.Perspective.Planar
{
    public class Batch
    {
        private SpriteBatch SpriteBatch;
        public bool Begun = false;

        public Batch(GraphicsDevice GraphicsDevice) { SpriteBatch = new SpriteBatch(GraphicsDevice); }

        private SpriteSortMode SortMode = SpriteSortMode.Deferred;
        private BlendState BlendState;
        private SamplerState SamplerState;
        private DepthStencilState DepthStencilState;
        private RasterizerState RasterizerState;
        private Effect Effect;
        private Matrix? Matrix;
        /// <summary>
        /// Begin the SpriteBatch
        /// </summary>
        /// <param name="Matrix">Self-explanatory.</param>
        public void Begin(Matrix Matrix, bool Backup = false) { Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix, Backup); }
        /// <summary>
        /// Begin the SpriteBatch
        /// </summary>
        /// <param name="SortMode">Self-explanatory.</param>
        /// <param name="Matrix">Self-explanatory.</param>
        public void Begin(SpriteSortMode SortMode, Matrix Matrix, bool Backup = false) { Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix, Backup); }
        /// <summary>
        /// Begin the SpriteBatch
        /// </summary>
        /// <param name="SortMode">Self-explanatory.</param>
        /// <param name="BlendState">Self-explanatory.</param>
        /// <param name="Matrix">Self-explanatory.</param>
        public void Begin(SpriteSortMode SortMode, BlendState BlendState, Matrix Matrix, bool Backup = false) { Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix, Backup); }
        /// <summary>
        /// Begin the SpriteBatch
        /// </summary>
        /// <param name="SortMode">Self-explanatory.</param>
        /// <param name="BlendState">Self-explanatory.</param>
        /// <param name="SamplerState">Self-explanatory.</param>
        /// <param name="DepthStencilState">Self-explanatory.</param>
        /// <param name="RasterizerState">Self-explanatory.</param>
        /// <param name="Effect">Self-explanatory.</param>
        /// <param name="Matrix">Self-explanatory.</param>
        public void Begin(SpriteSortMode SortMode = SpriteSortMode.Deferred, BlendState BlendState = null, SamplerState SamplerState = null, DepthStencilState DepthStencilState = null, RasterizerState RasterizerState = null, Effect Effect = null, Matrix? Matrix = null, bool Backup = false)
        {
            if (Begun) End();
            Begun = true;
            if (Backup) this.Backup(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix);
            SpriteBatch.Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix);
        }
        public void Backup(SpriteSortMode SortMode = SpriteSortMode.Deferred, BlendState BlendState = null, SamplerState SamplerState = null, DepthStencilState DepthStencilState = null, RasterizerState RasterizerState = null, Effect Effect = null, Matrix? Matrix = null)
        {
            this.SortMode = SortMode;
            this.BlendState = BlendState;
            this.SamplerState = SamplerState;
            this.DepthStencilState = DepthStencilState;
            this.RasterizerState = RasterizerState;
            this.Effect = Effect;
            this.Matrix = Matrix;
        }
        public void Restore() { if (Begun) { End(); Begin(SortMode, BlendState, SamplerState, DepthStencilState, RasterizerState, Effect, Matrix); } else Begin(); }
        public void End() { if (Begun) { Begun = false; SpriteBatch.End(); } }

        public static void EndAll() { if (Globe.Batches != null) for (int i = 0; i < Globe.Batches.Length; i++) Globe.Batches[i].End(); }

        public void Draw(Texture2D Texture, Rectangle Bounds, Rectangle? Source, Color Color, float Angle, Origin Origin, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            if (Origin.IsScale) Origin.Value = new Vector2(((Source.HasValue ? Source.Value.Width : Texture.Width) * Origin.Value.X), ((Source.HasValue ? Source.Value.Height : Texture.Height) * Origin.Value.Y));
            if (!Begun) Begin(); SpriteBatch.Draw(Texture, Bounds, Source, Color, Angle, Origin.Value, Effect, Layer);
        }
        public void Draw(Texture2D Texture, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, float Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            if (Origin.IsScale) Origin.Value = new Vector2(((Source.HasValue ? Source.Value.Width : Texture.Width) * Origin.Value.X), ((Source.HasValue ? Source.Value.Height : Texture.Height) * Origin.Value.Y));
            if (!Begun) Begin(); SpriteBatch.Draw(Texture, Position, Source, Color, Angle, Origin.Value, Scale, Effect, Layer);
        }
        public void Draw(Texture2D Texture, Vector2 Position, Rectangle? Source, Color Color, float Angle, Origin Origin, Vector2 Scale, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            if (Origin.IsScale) Origin.Value = new Vector2(((Source.HasValue ? Source.Value.Width : Texture.Width) * Origin.Value.X), ((Source.HasValue ? Source.Value.Height : Texture.Height) * Origin.Value.Y));
            if (!Begun) Begin(); SpriteBatch.Draw(Texture, Position, Source, Color, Angle, Origin.Value, Scale, Effect, Layer);
        }
        public void DrawString(string String, SpriteFont Font, Vector2 Position, Origin Origin, Color Fore, Color? Back = null, float Angle = 0, float Scale = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Vector2 Size = Font.MeasureString(String);
            if (Origin.IsScale) Origin.Value = new Vector2((Size.X * Origin.Value.X), (Size.Y * Origin.Value.Y));
            if (!Begun) Begin();
            if (Back != null) SpriteBatch.DrawString(Font, String, (Position + Vector2.One), (Color)Back, Angle, Origin.Value, Scale, Effect, ((Layer > 0) ? (Layer - .0000001f) : Layer));
            SpriteBatch.DrawString(Font, String, Position, Fore, Angle, Origin.Value, Scale, Effect, Layer);
        }
        public void DrawRectangle(Rectangle Bounds, Color? Fill = null, Color? Stroke = null, float Angle = 0, int Size = 1, float Layer = 0)
        {
            if (!Begun) Begin();
            if (Fill != null) Draw(Pixel(Color.White, true), Bounds, null, (Color)Fill, Angle, Origin.None, SpriteEffects.None, Layer);
            if ((Stroke == null) || (Size < 1)) return;
            else
            {
                Draw(Pixel(Color.White, true), new Rectangle(Bounds.X, (Bounds.Y - Size), Bounds.Width, Size), null, (Color)Stroke, Angle, Origin.None, SpriteEffects.None, Layer);
                Draw(Pixel(Color.White, true), new Rectangle((Bounds.X - Size), Bounds.Y, Size, Bounds.Height), null, (Color)Stroke, Angle, Origin.None, SpriteEffects.None, Layer);
                Draw(Pixel(Color.White, true), new Rectangle((Bounds.X + Bounds.Width), Bounds.Y, Size, Bounds.Height), null, (Color)Stroke, Angle, Origin.None, SpriteEffects.None, Layer);
                Draw(Pixel(Color.White, true), new Rectangle(Bounds.X, (Bounds.Y + Bounds.Height), Bounds.Width, Size), null, (Color)Stroke, Angle, Origin.None, SpriteEffects.None, Layer);
            }
        }
    }
}