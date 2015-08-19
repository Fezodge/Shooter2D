using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EzGame.Input
{
    public static class Mouse
    {
        private static MouseState M, LastM;
        public static Vector2 CameraPosition;

        public static int X
        {
            get { return (int) ((M.X/(float) Screen.WindowWidth)*Screen.ViewportWidth); }
        }

        public static int Y
        {
            get { return (int) ((M.Y/(float) Screen.WindowHeight)*Screen.ViewportHeight); }
        }

        public static Point Position
        {
            get { return new Point(X, Y); }
            set { Microsoft.Xna.Framework.Input.Mouse.SetPosition(value.X, value.Y); }
        }

        public static void Update()
        {
            LastM = M;
            M = Microsoft.Xna.Framework.Input.Mouse.GetState();
        }

        /// <summary>
        /// Check if a mouse button has been pressed
        /// </summary>
        /// <param name="Key">The mouse button to check.</param>
        /// <returns>A True/False statement.</returns>
        public static bool Pressed(Buttons Key)
        {
            if ((Key == Buttons.Left) && (M.LeftButton == ButtonState.Pressed) &&
                ((LastM == null) || (LastM.LeftButton == ButtonState.Released))) return true;
            if ((Key == Buttons.Middle) && (M.MiddleButton == ButtonState.Pressed) &&
                ((LastM == null) || (LastM.MiddleButton == ButtonState.Released))) return true;
            if ((Key == Buttons.Right) && (M.RightButton == ButtonState.Pressed) &&
                ((LastM == null) || (LastM.RightButton == ButtonState.Released))) return true;
            return false;
        }

        /// <summary>
        /// Check if a mouse button has been released
        /// </summary>
        /// <param name="Key">The mouse button to check.</param>
        /// <returns>A True/False statement.</returns>
        public static bool Released(Buttons Key)
        {
            if ((Key == Buttons.Left) && (M.LeftButton == ButtonState.Released) &&
                ((LastM != null) && (LastM.LeftButton == ButtonState.Pressed))) return true;
            if ((Key == Buttons.Middle) && (M.MiddleButton == ButtonState.Released) &&
                ((LastM != null) && (LastM.MiddleButton == ButtonState.Pressed))) return true;
            if ((Key == Buttons.Right) && (M.RightButton == ButtonState.Released) &&
                ((LastM != null) && (LastM.RightButton == ButtonState.Pressed))) return true;
            return false;
        }

        /// <summary>
        /// Check if a mouse button is being held
        /// </summary>
        /// <param name="Key">The mouse button to check.</param>
        /// <returns>A True/False statement.</returns>
        public static bool Holding(Buttons Key)
        {
            if ((Key == Buttons.Left) && (M.LeftButton == ButtonState.Pressed)) return true;
            if ((Key == Buttons.Middle) && (M.MiddleButton == ButtonState.Pressed)) return true;
            if ((Key == Buttons.Right) && (M.RightButton == ButtonState.Pressed)) return true;
            return false;
        }

        public static bool ScrolledUp()
        {
            return (M.ScrollWheelValue > LastM.ScrollWheelValue);
        }

        public static bool ScrolledDown()
        {
            return (M.ScrollWheelValue < LastM.ScrollWheelValue);
        }

        public enum Buttons
        {
            Left,
            Middle,
            Right
        }
    }
}