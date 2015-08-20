using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EzGame.Perspective.Planar;
using static EzGame.Perspective.Planar.Textures;

namespace Shooter2D
{
    public struct Tile
    {
        private static Map Map { get { return Game.Map; } }

        public const byte Width = 16, Height = 16;

        public ushort Fore, Back;
        public byte Angle;

        public bool Empty { get { return !((Fore > 0) || (Back > 0)); } }
        public bool ForeOnly { get { return ((Fore > 0) && (Back == 0)); } }
        public bool BackOnly { get { return ((Fore == 0) && (Back > 0)); } }
        public bool HasFore { get { return (Fore > 0); } }
        public bool HasBack { get { return (Back > 0); } }

        public void Draw(Batch Batch, Vector2 Position)
        {
            if (Back > 0) Batch.Draw(Textures.Get("Tiles.Back." + Back), Position, null, Color.White, 0, Origin.Center, 1, SpriteEffects.None, .6f);
            if (Fore > 0) Batch.Draw(Textures.Get("Tiles.Fore." + Fore), Position, null, Color.White, MathHelper.ToRadians((Angle / 128f) * 360), Origin.Center, 1, SpriteEffects.None, .4f);
            //Batch.Draw(Pixel(Color.Black, true), new Rectangle((int)(Position.X - (Width / 2f)), (int)(Position.Y - (Height / 2f)), Width, Height), null, (Color.White * (1 - (Light / 255f))), 0, Origin.None);
            //Batch.DrawString(Light.ToString(), Fonts.Get("Default/Tiny"), (Position - new Vector2(0, 5)), Origin.Center, Color.White, null, 0, .85f);
            //Batch.DrawString(Max.ToString(), Fonts.Get("Default/Tiny"), (Position + new Vector2(0, 5)), Origin.Center, Color.White, null, 0, .85f);
            //Batch.DrawString(State.ToString(), Fonts.Get("Default/Tiny"), Position, Origin.Center, Color.White, null, 0, .85f);
        }
    }
}