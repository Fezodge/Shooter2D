using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using EzGame.Perspective.Planar;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EzGame
{
    public static class Globe
    {
        public static ulong Version;
        public static bool Active;
        private static GameServiceContainer Container;
        private static Random SystemRandom;
        private static RandomNumberGenerator SecureRandom;
        public static string ExecutableDirectory =
            Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath);

        public static Batch[] Batches
        {
            get
            {
                try
                {
                    return Get<Batch[]>();
                }
                catch
                {
                    throw new MissingFieldException("The Batch Array has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static Form Form
        {
            get
            {
                try
                {
                    return Get<Form>();
                }
                catch
                {
                    throw new MissingFieldException("The Form has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static Viewport Viewport
        {
            get
            {
                try
                {
                    return Get<Viewport>();
                }
                catch
                {
                    throw new MissingFieldException("The Viewport has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static GameWindow GameWindow
        {
            get
            {
                try
                {
                    return Get<GameWindow>();
                }
                catch
                {
                    throw new MissingFieldException("The GameWindow has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static ContentManager ContentManager
        {
            get
            {
                try
                {
                    return Get<ContentManager>();
                }
                catch
                {
                    throw new MissingFieldException("The ContentManager has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static GraphicsDevice GraphicsDevice
        {
            get
            {
                try
                {
                    return Get<GraphicsDevice>();
                }
                catch
                {
                    throw new MissingFieldException("The GraphicsDevice has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static GraphicsAdapter GraphicsAdapter
        {
            get
            {
                try
                {
                    return Get<GraphicsAdapter>();
                }
                catch
                {
                    throw new MissingFieldException("The GraphicsAdapter has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static GraphicsDeviceManager GraphicsDeviceManager
        {
            get
            {
                try
                {
                    return Get<GraphicsDeviceManager>();
                }
                catch
                {
                    throw new MissingFieldException("The GraphicsDeviceManager has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static TextureLoader TextureLoader
        {
            get
            {
                try
                {
                    return Get<TextureLoader>();
                }
                catch
                {
                    throw new MissingFieldException("The TextureLoader has not (yet) been Globalized!");
                }
            }
            set { Add(value); }
        }

        public static T Get<T>()
        {
            return (T) Container.GetService(typeof (T));
        }

        public static void Add<T>(T Service)
        {
            if (Container == null) Container = new GameServiceContainer();
            Container.AddService(typeof (T), Service);
        }

        public static void Remove<T>()
        {
            Container.RemoveService(typeof (T));
        }

        public static Vector2 Rotate(Vector2 Position, float Angle)
        {
            return Vector2.Transform(Position, Matrix.CreateRotationZ(Angle));
        }

        public static Vector2 Rotate(Vector2 Position, Vector2 Origin, float Angle)
        {
            return (Vector2.Transform((Position - Origin), Matrix.CreateRotationZ(Angle)) + Origin);
        }

        public static float Lerp(float Original, float New, float Velocity)
        {
            float c, d;
            if (New < Original)
            {
                c = New + MathHelper.TwoPi;
                d = c - Original > Original - New
                    ? MathHelper.Lerp(Original, New, Velocity)
                    : MathHelper.Lerp(Original, c, Velocity);
            }
            else if (New > Original)
            {
                c = New - MathHelper.TwoPi;
                d = New - Original > Original - c
                    ? MathHelper.Lerp(Original, c, Velocity)
                    : MathHelper.Lerp(Original, New, Velocity);
            }
            else return Original;
            return MathHelper.WrapAngle(d);
        }

        public static float Angle(Vector2 From, Vector2 To)
        {
            return (float) Math.Atan2((To.Y - From.Y), (To.X - From.X));
        }

        public static double Wrap(double value, double max, double min = 0)
        {
            value -= min;
            max -= min;
            if (max == 0) return min;
            value = value%max;
            value += min;
            while (value < min) value += max;
            return value;
        }

        public static Vector2 Move(ref Vector2 Position, float Angle, float Velocity)
        {
            return (Position += new Vector2(((float) Math.Cos(Angle)*Velocity), ((float) Math.Sin(Angle)*Velocity)));
        }

        public static Vector2 Move(ref Vector2 Position, Vector2 Other, float Velocity)
        {
            return Move(ref Position, Angle(Position, Other), Velocity);
        }

        public static Vector2 Move(Vector2 Position, float Angle, float Velocity)
        {
            return (Position + new Vector2(((float) Math.Cos(Angle)*Velocity), ((float) Math.Sin(Angle)*Velocity)));
        }

        public static Vector2 Move(Vector2 Position, Vector2 Other, float Velocity)
        {
            return Move(Position, Angle(Position, Other), Velocity);
        }

        public static int Difference(int Value1, int Value2)
        {
            return (Math.Max(Value1, Value2) - Math.Min(Value1, Value2));
        }

        public static float Difference(float Value1, float Value2)
        {
            return (Math.Max(Value1, Value2) - Math.Min(Value1, Value2));
        }

        public static double Difference(double Value1, double Value2)
        {
            return (Math.Max(Value1, Value2) - Math.Min(Value1, Value2));
        }

        public static float AngleDifference(float A, float B)
        {
            float AD = MathHelper.ToDegrees(A), BD = MathHelper.ToDegrees(B), Difference = Math.Abs(AD - BD)%360;
            if (Difference > 180) Difference = 360 - Difference;
            return MathHelper.ToRadians(Difference);
        }

        public static double RandomDouble(bool Secure = false)
        {
            if (!Secure)
            {
                if (SystemRandom == null) SystemRandom = new Random();
                return SystemRandom.NextDouble();
            }
            if (SecureRandom == null) SecureRandom = RandomNumberGenerator.Create();
            var Array = new byte[4];
            SecureRandom.GetBytes(Array);
            return ((double) BitConverter.ToUInt32(Array, 0)/uint.MaxValue);
        }

        public static int Random(int Min, int Max, bool Secure = false)
        {
            if (!Secure)
            {
                if (SystemRandom == null) SystemRandom = new Random();
                return SystemRandom.Next(Min, (Max + 1));
            }
            if (SecureRandom == null) SecureRandom = RandomNumberGenerator.Create();
            return ((int) Math.Round(RandomDouble(true)*((Max + 1) - Min - 1)) + Min);
        }

        public static float Random(float Min, float Max, bool Secure = false)
        {
            return (float) Random(Min, (double) Max, Secure);
        }

        public static double Random(double Min, double Max, bool Secure = false)
        {
            if (!Secure)
            {
                if (SystemRandom == null) SystemRandom = new Random();
                return (Min + (RandomDouble(false)*Difference(Min, Max)));
            }
            if (SecureRandom == null) SecureRandom = RandomNumberGenerator.Create();
            return (Min + (RandomDouble(true)*Difference(Min, Max)));
        }

        public static int Random(int Max, bool Secure = false)
        {
            if (!Secure)
            {
                if (SystemRandom == null) SystemRandom = new Random();
                return SystemRandom.Next(Max + 1);
            }
            return Random(0, Max, true);
        }

        public static float Random(float Max, bool Secure = false)
        {
            return (float) Random((double) Max, Secure);
        }

        public static double Random(double Max, bool Secure = false)
        {
            if (!Secure)
            {
                if (SystemRandom == null) SystemRandom = new Random();
                return (RandomDouble(false)*Max);
            }
            if (SecureRandom == null) SecureRandom = RandomNumberGenerator.Create();
            return (RandomDouble(true)*Max);
        }

        public static int Pick(params int[] Values)
        {
            return Values[Random(Values.Length - 1)];
        }

        public static float Pick(params float[] Values)
        {
            return Values[Random(Values.Length - 1)];
        }

        public static double Pick(params double[] Values)
        {
            return Values[Random(Values.Length - 1)];
        }

        public static string Pick(params string[] Values)
        {
            return Values[Random(Values.Length - 1)];
        }

        public static bool Chance(uint Value, uint Max = 100, bool Secure = false)
        {
            return (Random((Max - 1), Secure) <= (Value - 1));
        }
    }
}