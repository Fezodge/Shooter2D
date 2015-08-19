namespace EzGame
{
    public static class Screen
    {
        public static int BackBufferWidth { get { return Globe.GraphicsDevice.PresentationParameters.BackBufferWidth; } set { Globe.GraphicsDeviceManager.PreferredBackBufferWidth = value; Globe.GraphicsDeviceManager.ApplyChanges(); } }
        public static int BackBufferHeight { get { return Globe.GraphicsDevice.PresentationParameters.BackBufferHeight; } set { Globe.GraphicsDeviceManager.PreferredBackBufferHeight = value; Globe.GraphicsDeviceManager.ApplyChanges(); } }

        public static bool Fullscreen { get { return Globe.GraphicsDeviceManager.IsFullScreen; } set { Globe.GraphicsDeviceManager.IsFullScreen = value; Globe.GraphicsDeviceManager.ApplyChanges(); } }
        public static bool VSync { get { return Globe.GraphicsDeviceManager.SynchronizeWithVerticalRetrace; } set { Globe.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = value; Globe.GraphicsDeviceManager.ApplyChanges(); } }

        public static int ViewportWidth { get { return Globe.Viewport.Width; } }
        public static int ViewportHeight { get { return Globe.Viewport.Height; } }

        public static int WindowWidth { get { return Globe.GameWindow.ClientBounds.Width; } }
        public static int WindowHeight { get { return Globe.GameWindow.ClientBounds.Height; } }
        public static void Expand(bool HideControlBar)
        {
            if (HideControlBar) Globe.Form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Globe.Form.Location = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Location;
            Globe.Form.Size = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Size;
        }
    }
}