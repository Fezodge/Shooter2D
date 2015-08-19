using System;
using EzGame.Perspective.Planar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static EzGame.Perspective.Planar.Textures;

namespace EzGame
{
    public static class Performance
    {
        /* (Update) Frames Per Second Counter */
        public static float[] UpdateFramesPerSecondBuffer;
        private static ushort? UpdateFramesPerSecondBufferIndex;
        private static ushort UpdateFramesPerSecondBufferRecorded;
        public static float UpdateFramesPerSecond;
        /* (Draw) Frames Per Second Counter */
        private static float SecondTimer;
        public static float[] DrawFramesPerSecondBuffer;
        private static byte? DrawFramesPerSecondBufferIndex;
        private static byte DrawFramesPerSecondBufferRecorded;
        private static ushort PrivateDrawFramesPerSecond;
        public static ushort DrawFramesPerSecond;

        public static float AverageUpdateFramesPerSecond
        {
            get
            {
                float Value = 0;
                for (byte i = 0; i < UpdateFramesPerSecondBufferRecorded; i++) Value += UpdateFramesPerSecondBuffer[i];
                return (Value/UpdateFramesPerSecondBufferRecorded);
            }
        }

        public static float AverageDrawFramesPerSecond
        {
            get
            {
                float Value = 0;
                for (byte i = 0; i < DrawFramesPerSecondBufferRecorded; i++) Value += DrawFramesPerSecondBuffer[i];
                return (Value/DrawFramesPerSecondBufferRecorded);
            }
        }

        public static void Update(GameTime Time)
        {
            UpdateFramesPerSecond = (1/(float) Time.ElapsedGameTime.TotalSeconds);
            if (UpdateFramesPerSecondBuffer != null)
            {
                UpdateFramesPerSecondBuffer[
                    (UpdateFramesPerSecondBufferIndex ?? (UpdateFramesPerSecondBuffer.Length - 1))] =
                    UpdateFramesPerSecond;
                if (UpdateFramesPerSecondBufferIndex == null) UpdateFramesPerSecondBufferIndex = 0;
                else
                {
                    if (UpdateFramesPerSecondBufferIndex >= (UpdateFramesPerSecondBuffer.Length - 1))
                        UpdateFramesPerSecondBufferIndex = null;
                    else UpdateFramesPerSecondBufferIndex++;
                }
                if (UpdateFramesPerSecondBufferRecorded < UpdateFramesPerSecondBuffer.Length)
                    UpdateFramesPerSecondBufferRecorded++;
            }
        }

        public static void Draw(GameTime Time)
        {
            PrivateDrawFramesPerSecond++;
            SecondTimer += (float) Time.ElapsedGameTime.TotalSeconds;
            if (SecondTimer >= 1)
            {
                SecondTimer--;
                DrawFramesPerSecond = PrivateDrawFramesPerSecond;
                PrivateDrawFramesPerSecond = 0;
                if (DrawFramesPerSecondBuffer != null)
                {
                    DrawFramesPerSecondBuffer[(DrawFramesPerSecondBufferIndex ?? (DrawFramesPerSecondBuffer.Length - 1))
                        ] = DrawFramesPerSecond;
                    if (DrawFramesPerSecondBufferIndex == null) DrawFramesPerSecondBufferIndex = 0;
                    else
                    {
                        if (DrawFramesPerSecondBufferIndex >= (DrawFramesPerSecondBuffer.Length - 1))
                            DrawFramesPerSecondBufferIndex = null;
                        else DrawFramesPerSecondBufferIndex++;
                    }
                    if (DrawFramesPerSecondBufferRecorded < DrawFramesPerSecondBuffer.Length)
                        DrawFramesPerSecondBufferRecorded++;
                }
            }
        }

        public static void Draw(SpriteFont Font, Vector2 Position, Origin Origin, Color Fore, Color? Back = null,
            float Angle = 0, float Scale = 1, SpriteEffects Effect = SpriteEffects.None, float Layer = 0)
        {
            Draw(Globe.Batches[0], Font, Position, Origin, Fore, Back, Angle, Scale, Effect, Layer);
        }

        public static void Draw(Batch Batch, SpriteFont Font, Vector2 Position, Origin Origin, Color Fore,
            Color? Back = null, float Angle = 0, float Scale = 1, SpriteEffects Effect = SpriteEffects.None,
            float Layer = 0)
        {
            Batch.DrawString(
                string.Format("Update FPS: {0} (Avg. {1})\nDraw FPS: {2} (Avg. {3})", Math.Round(UpdateFramesPerSecond),
                    Math.Round(AverageUpdateFramesPerSecond),
                    DrawFramesPerSecond, Math.Round(AverageDrawFramesPerSecond)), Font, Position, Origin, Fore, Back,
                Angle, Scale, Effect, Layer);
        }
    }
}