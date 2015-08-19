using EzGame.Input;
using Microsoft.Xna.Framework;

namespace EzGame.Perspective.Planar
{
    public class Camera
    {
        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        public Matrix View
        {
            get
            {
                Matrix Matrix = (Matrix.CreateTranslation(new Vector3(-Position, 0))*Matrix.CreateRotationZ(Angle)*
                                 Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))*
                                 Matrix.CreateTranslation((Screen.BackBufferWidth/2f), (Screen.BackBufferHeight/2f), 0)),
                    Invert = Matrix.Invert(Matrix);
                Mouse.CameraPosition = new Vector2(Mouse.Position.X, Mouse.Position.Y);
                Vector2.Transform(ref Mouse.CameraPosition, ref Invert, out Mouse.CameraPosition);
                return Matrix;
            }
        }

        public float Angle, Zoom = 1;
        public Vector2 Position;
    }
}