using Microsoft.Xna.Framework;
using EzGame;
using EzGame.Perspective.Planar;

namespace Shooter2D
{
    public class Map
    {
        public Tile[,] Tiles;
        public Camera Camera;

        public Map(int Width, int Height)
        {
            Tiles = new Tile[Width, Height];
            Camera = new Camera();
        }

        public void Update(GameTime Time)
        {
        }

        public void Draw() { Draw(Globe.Batches[0]); }
        public void Draw(Batch Batch)
        {
            for (int x = (int)((Camera.X - (Screen.ViewportWidth / 2f)) / Tile.Width); x <= (int)((Camera.X + (Screen.ViewportWidth / 2f)) / Tile.Width); x++)
                for (int y = (int)((Camera.Y - (Screen.ViewportHeight / 2f)) / Tile.Height); y <= (int)((Camera.Y + (Screen.ViewportHeight / 2f)) / Tile.Height); y++)
                    if (InBounds(x, y))
                    {
                        Tile Tile = Tiles[x, y];
                        Vector2 Position = new Vector2(((x * Tile.Width) + (Tile.Width / 2f)), ((y * Tile.Height) + (Tile.Height / 2f)));
                        Tile.Draw(Batch, Position);
                    }
        }

        public bool InBounds(int x, int y) { return !((x < 0) || (y < 0) || (x >= Tiles.GetLength(0)) || (y >= Tiles.GetLength(1))); }
        public bool OffCamera(int x, int y, sbyte Offset)
        {
            return ((x < (int)((Camera.X - (Screen.ViewportWidth / 2f)) / Tile.Width - Offset)) || (y < (int)((Camera.Y - (Screen.ViewportHeight / 2f)) / Tile.Height - Offset)) ||
                (x >= (int)((Camera.X + (Screen.ViewportWidth / 2f)) / Tile.Width + Offset)) || (y >= (int)((Camera.Y + (Screen.ViewportHeight / 2f)) / Tile.Height + Offset)));
        }
    }
}