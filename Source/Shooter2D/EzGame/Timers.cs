﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace EzGame
{
    public static class Timers
    {
        private static Dictionary<string, Timer> Array;
        public static Timer Add(string Name, double Interval)
        {
            if (Array == null) Array = new Dictionary<string, Timer>();
            if (!Array.ContainsKey(Name)) Array.Add(Name, new Timer(Interval));
            else Array[Name].Interval = Interval;
            return Array[Name];
        }
        public static void Remove(string Name) { if ((Array != null) && Array.ContainsKey(Name)) Array.Remove(Name); }
        public static void Clear() { Array.Clear(); Array = null; }

        public static void Update(GameTime GameTime)
        {
            if (Array != null)
                foreach (Timer Timer in Array.Values)
                {
                    if (Timer.Time >= Timer.Interval) Timer.Time -= Timer.Interval;
                    Timer.Time += GameTime.ElapsedGameTime.TotalSeconds;
                }
        }
        public static bool Tick(string Name) { return ((Array != null) && Array.ContainsKey(Name) && (Array[Name].Time >= Array[Name].Interval)); }

        public class Timer
        {
            public double Interval, Time;
            public bool Tick { get { return (Time >= Interval); } }

            public Timer(double Interval) { this.Interval = Interval; }
        }
    }
}