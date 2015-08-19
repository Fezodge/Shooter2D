using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using EzGame.Collision;
using EzGame.Perspective.Planar;
using static EzGame.Perspective.Planar.Textures;

namespace EzGame
{
    public static class Profiler
    {
        private static bool PrivateUpdate;
        public static bool Update { get { return PrivateUpdate; } set { PrivateUpdate = value; if (!value && (Profiles != null)) foreach (string Key in Profiles.Keys) if (Profiles[Key].Stopwatch.IsRunning) Stop(Key); } }

        public static Dictionary<string, Profile> Profiles;
        public static Profile Get(string Name, byte? Maximum)
        {
            if (Profiles == null) Profiles = new Dictionary<string, Profile>();
            if (Profiles.ContainsKey(Name)) return Profiles[Name];
            else { Profiles.Add(Name, new Profile((Maximum ?? byte.MaxValue)) { Index = 0 }); return Profiles[Name]; }
        }

        public static void Start(string Name, byte? Maximum = null)
        {
            if (Update)
            {
                Profile Profile = Get(Name, Maximum);
                if (Profile.Stopwatch == null) Profile.Stopwatch = new Stopwatch();
                if (!Profile.Stopwatch.IsRunning) Profile.Stopwatch.Start();
            }
        }
        public static void Stop(string Name)
        {
            Profile Profile = Get(Name, null);
            if (Profile.Stopwatch == null) Profile.Stopwatch = new Stopwatch();
            if (Profile.Stopwatch.IsRunning)
            {
                Profile.Stopwatch.Stop();
                Profile.TimeSpans[(Profile.Index ?? (Profile.TimeSpans.Length - 1))] = Profile.Stopwatch.Elapsed;
                if (Profile.Index == null) Profile.Index = 0;
                else
                {
                    if (Profile.Index >= (Profile.TimeSpans.Length - 1)) Profile.Index = null;
                    else Profile.Index++;
                }
                if (Profile.Recorded < Profile.TimeSpans.Length) Profile.Recorded++;
                Profile.Stopwatch.Reset();
            }
        }

        public static void Draw(int Width) { Draw(Globe.Batches[0], Width); }
        public static void Draw(Batch Batch, int Width)
        {
            Start("Profiler");
            int x = (Screen.BackBufferWidth - (Width + 5)), y = 5;
            Batch.DrawRectangle(new Rectangle(x, y, Width, (Screen.BackBufferHeight - 10)), (Color.Black * .8f), (Color.White * .8f));
            string String = string.Format("Update FPS: {0} (Avg: {1}) - Draw FPS: {2} (Avg: {3})", Math.Round(Performance.UpdateFramesPerSecond), Math.Round(Performance.AverageUpdateFramesPerSecond),
                Performance.DrawFramesPerSecond, Math.Round(Performance.AverageDrawFramesPerSecond));
            SpriteFont Font = Fonts.Get("Default/ExtraSmall");
            Batch.DrawString(String, Font, new Vector2((x + (Width / 2f)), (y + (Font.MeasureString(String).Y / 2))), Origin.Center, (Color.White * .8f), (Color.Black * .8f));
            new Line(new Vector2(x, (y + (int)Font.MeasureString(String).Y)), new Vector2((x + Width), (y + (int)Font.MeasureString(String).Y))).Draw(Batch, (Color.White * .8f));
            y += (int)Font.MeasureString(String).Y;
            new Line(new Vector2(x, (y + 20)), new Vector2((x + Width), (y + 20))).Draw(Batch, (Color.White * .8f));
            new Line(new Vector2((x + ((Width / 5) * 2)), y), new Vector2((x + ((Width / 5) * 2)), (y + (Screen.BackBufferHeight - 25)))).Draw(Batch, (Color.White * .8f));
            new Line(new Vector2((x + ((Width / 5) * 3)), y), new Vector2((x + ((Width / 5) * 3)), (y + (Screen.BackBufferHeight - 25)))).Draw(Batch, (Color.White * .8f));
            new Line(new Vector2((x + ((Width / 5) * 4)), y), new Vector2((x + ((Width / 5) * 4)), (y + (Screen.BackBufferHeight - 25)))).Draw(Batch, (Color.White * .8f));
            x += 5;
            Font = Fonts.Get("Default/Small");
            Batch.DrawString("Name", Font, new Vector2(x, y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
            Batch.DrawString("Avg Ms", Font, new Vector2((x + ((Width / 5) * 2)), y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
            Batch.DrawString("Avg Sec(s)", Font, new Vector2((x + ((Width / 5) * 3)), y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
            Batch.DrawString("Avg Ticks", Font, new Vector2((x + ((Width / 5) * 4)), y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
            if (Profiles != null)
                foreach (string Key in Profiles.Keys)
                {
                    y += 21;
                    new Line(new Vector2((x - 5), (y + 20)), new Vector2((x + Width), (y + 20))).Draw(Batch, (Color.Silver * .4f));
                    Batch.DrawString(Key, Font, new Vector2(x, y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
                    Batch.DrawString((Math.Round(AverageMilliseconds(Key), 3) + " ms"), Font, new Vector2((x + ((Width / 5) * 2)), y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
                    Batch.DrawString((Math.Round(AverageSeconds(Key), 4) + "s"), Font, new Vector2((x + ((Width / 5) * 3)), y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
                    string AverageTicksString = AverageTicks(Key).ToString("0,00"); while (AverageTicksString.StartsWith("0")) AverageTicksString = AverageTicksString.Substring(1);
                    Batch.DrawString(AverageTicksString, Font, new Vector2((x + ((Width / 5) * 4)), y), Origin.None, (Color.White * .8f), (Color.Black * .8f));
                }
            Stop("Profiler");
        }

        public static float Ticks(string Name)
        {
            Profile Profile = Get(Name, null);
            int Index = ((Profile.Index ?? Profile.TimeSpans.Length) - 1);
            if (Index == -1) Index = (Profile.TimeSpans.Length - 1);
            return (float)Profile.TimeSpans[Index].Ticks;
        }
        public static float Milliseconds(string Name)
        {
            Profile Profile = Get(Name, null);
            int Index = ((Profile.Index ?? Profile.TimeSpans.Length) - 1);
            if (Index == -1) Index = (Profile.TimeSpans.Length - 1);
            return (float)Profile.TimeSpans[Index].TotalMilliseconds;
        }
        public static float Seconds(string Name)
        {
            Profile Profile = Get(Name, null);
            int Index = ((Profile.Index ?? Profile.TimeSpans.Length) - 1);
            if (Index == -1) Index = (Profile.TimeSpans.Length - 1);
            return (float)Profile.TimeSpans[Index].TotalSeconds;
        }
        public static float Minutes(string Name)
        {
            Profile Profile = Get(Name, null);
            int Index = ((Profile.Index ?? Profile.TimeSpans.Length) - 1);
            if (Index == -1) Index = (Profile.TimeSpans.Length - 1);
            return (float)Profile.TimeSpans[Index].TotalMinutes;
        }
        public static float Hours(string Name)
        {
            Profile Profile = Get(Name, null);
            int Index = ((Profile.Index ?? Profile.TimeSpans.Length) - 1);
            if (Index == -1) Index = (Profile.TimeSpans.Length - 1);
            return (float)Profile.TimeSpans[Index].TotalHours;
        }
        public static float Days(string Name)
        {
            Profile Profile = Get(Name, null);
            int Index = ((Profile.Index ?? Profile.TimeSpans.Length) - 1);
            if (Index == -1) Index = (Profile.TimeSpans.Length - 1);
            return (float)Profile.TimeSpans[Index].TotalDays;
        }

        public static float AverageTicks(string Name)
        {
            Profile Profile = Get(Name, null);
            float Value = 0;
            for (int i = 0; i < Profile.Recorded; i++)
            {
                TimeSpan TimeSpan = Profile.TimeSpans[i];
                Value += (float)TimeSpan.Ticks;
            }
            return (Value / Profile.Recorded);
        }
        public static float AverageMilliseconds(string Name)
        {
            Profile Profile = Get(Name, null);
            float Value = 0;
            for (int i = 0; i < Profile.Recorded; i++)
            {
                TimeSpan TimeSpan = Profile.TimeSpans[i];
                Value += (float)TimeSpan.TotalMilliseconds;
            }
            return (Value / Profile.Recorded);
        }
        public static float AverageSeconds(string Name)
        {
            Profile Profile = Get(Name, null);
            float Value = 0;
            for (int i = 0; i < Profile.Recorded; i++)
            {
                TimeSpan TimeSpan = Profile.TimeSpans[i];
                Value += (float)TimeSpan.TotalSeconds;
            }
            return (Value / Profile.Recorded);
        }
        public static float AverageMinutes(string Name)
        {
            Profile Profile = Get(Name, null);
            float Value = 0;
            for (int i = 0; i < Profile.Recorded; i++)
            {
                TimeSpan TimeSpan = Profile.TimeSpans[i];
                Value += (float)TimeSpan.TotalMinutes;
            }
            return (Value / Profile.Recorded);
        }
        public static float AverageHours(string Name)
        {
            Profile Profile = Get(Name, null);
            float Value = 0;
            for (int i = 0; i < Profile.Recorded; i++)
            {
                TimeSpan TimeSpan = Profile.TimeSpans[i];
                Value += (float)TimeSpan.TotalHours;
            }
            return (Value / Profile.Recorded);
        }
        public static float AverageDays(string Name)
        {
            Profile Profile = Get(Name, null);
            float Value = 0;
            for (int i = 0; i < Profile.Recorded; i++)
            {
                TimeSpan TimeSpan = Profile.TimeSpans[i];
                Value += (float)TimeSpan.TotalDays;
            }
            return (Value / Profile.Recorded);
        }

        public class Profile
        {
            public Stopwatch Stopwatch;
            public TimeSpan[] TimeSpans;
            public byte? Index;
            public byte Recorded;

            public Profile(byte Maximum) { TimeSpans = new TimeSpan[Maximum]; }
        }
    }
}