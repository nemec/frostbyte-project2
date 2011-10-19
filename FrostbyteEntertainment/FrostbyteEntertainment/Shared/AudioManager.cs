using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Frostbyte
{
    class AudioManager
    {
        internal Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        internal Dictionary<string, Song> backgroundMusic = new Dictionary<string, Song>();

        internal void AddBackgroundMusic(string name)
        {
            try{
                backgroundMusic[name] = This.Game.Content.Load<Song>("Audio/" + name);
            }
            catch (NoAudioHardwareException)
            {
                // Ignore it...
            }
        }

        internal void PlayBackgroundMusic(string name)
        {
            Song s; 
            if (backgroundMusic.TryGetValue(name,out s))
            {
                try
                {
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Play(s);
                }
                catch (InvalidOperationException)
                {
                    MediaPlayer.Stop();
                }
            }
        }

        internal void BackgroundMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        internal void AddSoundEffect(string name)
        {
            try
            {
                soundEffects[name] = This.Game.Content.Load<SoundEffect>("Audio/" + name);
            }
            catch (NoAudioHardwareException)
            {
                // Ignore it...
            }
        }

        internal void PlaySoundEffect(string name)
        {
            PlaySoundEffect(name, 1f);
        }

        internal void PlaySoundEffect(string name, float volume)
        {
            SoundEffect se;
            if (soundEffects.TryGetValue(name,out se))
            {
                var sound = se.CreateInstance();
                sound.Volume = volume;
                sound.Play();
            }
        }

        internal void Pause()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }
            else if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
        }

        internal void Stop()
        {
            MediaPlayer.Stop();
        }
    }
}
