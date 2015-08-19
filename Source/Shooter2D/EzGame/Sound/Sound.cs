using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace EzGame
{
    public static class Sound
    {
        public static SoundEffectInstance[] Channels;
        private static Dictionary<string, ushort> SoundIDs;
        private static readonly Dictionary<string, SoundEffect> Library = new Dictionary<string, SoundEffect>();

        public static void Initialize(int Channels)
        {
            Sound.Channels = new SoundEffectInstance[Channels];
        }

        public static SoundEffect Get(string Path)
        {
            Load(Path);
            return Library[Path];
        }

        public static void Load(string Path)
        {
            if (!Library.ContainsKey(Path))
            {
                try
                {
                    Library.Add(Path, Globe.ContentManager.Load<SoundEffect>(Path));
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void Unload(string Path)
        {
            if (Library.ContainsKey(Path))
            {
                Library.Remove(Path);
                Globe.ContentManager.Unload();
            }
            else throw new KeyNotFoundException("The sound at \"" + Path + "\" was not found in memory!");
        }

        public static ushort? Play(string Path)
        {
            var Sound = Get(Path);
            if (Sound != null)
                for (ushort i = 0; i < Channels.Length; i++)
                {
                    if ((Channels[i] != null) && (Channels[i].State != SoundState.Stopped)) continue;
                    Channels[i] = Sound.CreateInstance();
                    Channels[i].Play();
                    if (SoundIDs == null)
                    {
                        SoundIDs = new Dictionary<string, ushort>();
                        SoundIDs.Add(Path, i);
                    }
                    else if (!SoundIDs.ContainsKey(Path)) SoundIDs.Add(Path, i);
                    else SoundIDs[Path] = i;
                    return i;
                }
            return null;
        }

        public static ushort? Play(string Path, bool Loop)
        {
            var Sound = Get(Path);
            if (Sound != null)
                for (ushort i = 0; i < Channels.Length; i++)
                {
                    if ((Channels[i] != null) && (Channels[i].State != SoundState.Stopped)) continue;
                    Channels[i] = Sound.CreateInstance();
                    Channels[i].IsLooped = Loop;
                    Channels[i].Play();
                    if (SoundIDs == null)
                    {
                        SoundIDs = new Dictionary<string, ushort>();
                        SoundIDs.Add(Path, i);
                    }
                    else if (!SoundIDs.ContainsKey(Path)) SoundIDs.Add(Path, i);
                    else SoundIDs[Path] = i;
                    return i;
                }
            return null;
        }

        public static ushort? Play(string Path, bool Loop, float Volume)
        {
            var Sound = Get(Path);
            if (Sound != null)
                for (ushort i = 0; i < Channels.Length; i++)
                {
                    if ((Channels[i] != null) && (Channels[i].State != SoundState.Stopped)) continue;
                    Channels[i] = Sound.CreateInstance();
                    Channels[i].IsLooped = Loop;
                    Channels[i].Volume = Volume;
                    Channels[i].Play();
                    if (SoundIDs == null)
                    {
                        SoundIDs = new Dictionary<string, ushort>();
                        SoundIDs.Add(Path, i);
                    }
                    else if (!SoundIDs.ContainsKey(Path)) SoundIDs.Add(Path, i);
                    else SoundIDs[Path] = i;
                    return i;
                }
            return null;
        }

        public static bool Pause(ushort Channel)
        {
            if ((Channels.Length > Channel) && (Channels[Channel] != null))
            {
                Channels[Channel].Pause();
                return true;
            }
            return false;
        }

        public static bool Pause(string Path)
        {
            var Channel = (((SoundIDs != null) && SoundIDs.ContainsKey(Path)) ? SoundIDs[Path] : (ushort?) null);
            if (Channel.HasValue && (Channels.Length > Channel.Value) && (Channels[Channel.Value] != null))
            {
                Channels[Channel.Value].Pause();
                return true;
            }
            return false;
        }

        public static bool Stop(ushort Channel)
        {
            if ((Channels.Length > Channel) && (Channels[Channel] != null))
            {
                Channels[Channel].Stop();
                Channels[Channel] = null;
                return true;
            }
            return false;
        }

        public static bool Stop(string Path)
        {
            var Channel = (((SoundIDs != null) && SoundIDs.ContainsKey(Path)) ? SoundIDs[Path] : (ushort?) null);
            if (Channel.HasValue && (Channels.Length > Channel.Value) && (Channels[Channel.Value] != null))
            {
                Channels[Channel.Value].Stop();
                Channels[Channel.Value] = null;
                return true;
            }
            return false;
        }

        public static bool? Looped(ushort Channel)
        {
            if ((Channels.Length > Channel) && (Channels[Channel] != null)) return Channels[Channel].IsLooped;
            return null;
        }

        public static bool Loop(ushort Channel, bool Value)
        {
            if ((Channels.Length > Channel) && (Channels[Channel] != null))
            {
                Channels[Channel].IsLooped = Value;
                return true;
            }
            return false;
        }

        public static float? Volume(ushort Channel)
        {
            if ((Channels.Length > Channel) && (Channels[Channel] != null)) return Channels[Channel].Volume;
            return null;
        }

        public static bool Volume(ushort Channel, float Value)
        {
            if ((Channels.Length > Channel) && (Channels[Channel] != null))
            {
                Channels[Channel].Volume = Value;
                return true;
            }
            return false;
        }
    }
}