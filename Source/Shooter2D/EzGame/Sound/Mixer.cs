using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EzGame;

namespace EzGame
{
    public class Mixer
    {
        public Dictionary<string, Sound> Sounds;
        private float _Volume;

        public Mixer(params Sound[] Sounds)
        {
            if (Sounds.Length > 0) this.Sounds = new Dictionary<string, Sound>();
            foreach (Sound Sound in Sounds) this.Sounds.Add(Sound.Path, Sound);
        }

        public float Volume { get { return _Volume; } set { if (Sounds != null) foreach (Sound Sound in Sounds.Values) Sound.Volume = value; _Volume = value; } }

        public void Update(GameTime Time)
        {
            if (Sounds != null) foreach (Sound Sound in Sounds.Values) Sound.Update(Time);
        }

        public void Play(float Volume) { if (Sounds != null) foreach (Sound Sound in Sounds.Values) Sound.Play(Volume); _Volume = Volume; }
        public void Stop() { if (Sounds != null) foreach (Sound Sound in Sounds.Values) Sound.Stop(); }

        public class Sound
        {
            public ushort? Channel;
            public string Path;
            public enum Types { Loop, Random }
            public Types Type;
            public byte MaxVolume;
            public double Time, RandomPerSec;
            private float CDTime;
            public float Volume { get { return (Channel.HasValue ? EzGame.Sound.Channels[Channel.Value].Volume : 0); } set { if (Channel.HasValue) EzGame.Sound.Channels[Channel.Value].Volume = (value * ((MaxVolume > 0) ? (MaxVolume / 100f) : 1)); } }

            public Sound(string Path, Types Type) { this.Path = Path; this.Type = Type; }

            public void Update(GameTime Time)
            {
                this.Time += Time.ElapsedGameTime.TotalSeconds;
                if (Channel.HasValue) { if (EzGame.Sound.Channels[Channel.Value].State == Microsoft.Xna.Framework.Audio.SoundState.Stopped) { Channel = null; } }
                else if (Type == Types.Loop) Play(true, 1);
                else if ((Type == Types.Random) && (this.Time >= RandomPerSec))
                {
                    if (CDTime <= 0) { if (Globe.Chance(20)) { Play(1); this.Time -= RandomPerSec; } CDTime = 2; }
                    else CDTime = Math.Max(0, (CDTime - (float)Time.ElapsedGameTime.TotalSeconds));
                }
            }

            public void Play(float Volume) { if (Channel.HasValue) Stop(); Channel = EzGame.Sound.Play(Path, (Type == Types.Loop), (Volume * ((MaxVolume > 0) ? (MaxVolume / 100f) : 1))); }
            public void Play(bool Loop, float Volume) { if (Channel.HasValue) Stop(); Channel = EzGame.Sound.Play(Path, Loop, (Volume * ((MaxVolume > 0) ? (MaxVolume / 100f) : 1))); }
            public void Stop() { if (Channel.HasValue) { EzGame.Sound.Stop(Channel.Value); Channel = null; } }
        }
    }
}